using System;
using System.Data;
using System.Globalization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// Pending Requests 화면의 파생 컬럼·필터·집계 모음 — 화면(폼)과 분리된
    /// 순수 DataTable 로직이다 (HistoryTablePresenter와 같은 역할 분담).
    ///
    /// 데이터 개념 (2026-07-17 FAC_SEND_MAS 기반 재정의):
    /// 인터페이스 흐름은 원래 전부 자동이다 — 발송 통보(FAC_SEND_MAS) → 물류
    /// 도착 → Receive 처리(전산 ITEM Created/Released 생성) → 의뢰서 연결 →
    /// Create 처리. 이 화면은 자동이 일부/전부 실패한 건을 사람이 이어서
    /// 처리하고(수동 Receive/Create), 도착했지만 의뢰서가 없는 목록과 전체
    /// 현황을 현업에게 보여주기 위한 것이다.
    ///
    /// 행 상태(STATUS)는 "아직 안 된 첫 단계"로 정한다 — 자동이 어느 조합까지
    /// 되고 멈췄든 다음에 할 일이 하나로 정해진다:
    ///   Sent             : 발송 통보(FAC_SEND_MAS)만 있고 도착 전    (액션 없음)
    ///   Arrived          : 도착했지만 Receive 미처리(RECV_YN='N')    (→ Receive)
    ///   Request Unlinked : Receive 완료, 의뢰서 미연결(REQ_NO 없음)  (대기 — 경과일 강조)
    ///   Request Linked   : 의뢰서 연결됨, Create 미처리(PROC_YN='N') (→ Create)
    ///   Completed        : Create 처리 완료(PROC_YN='Y')
    ///
    /// 발송 통보 여부(SEND_YN)는 상태와 **직교**한다 — 미확인 물류(SEND_YN='N')도
    /// 의뢰 인터페이스는 타므로 동일 파이프라인으로 진행하고, FAC_SEND_MAS에
    /// 있냐 없냐만 SEND_YN 배지(Y 초록 / N 빨강 틴트)로 구분해 보여준다.
    ///
    /// 경과일(ELAPSED_DAYS)은 두 구간에 표기한다 — 도착 전(운송 중)은 발송
    /// 통보 시각 기준(운송 지연 감지), 도착 후에는 "의뢰서와 연결이 안 된"
    /// 행만 도착 시각 기준(방치 기간). 연결된 행은 의뢰번호가 상태를 말해
    /// 주므로 비운다. 배지색: 0-2일 파랑 · 3-6일 호박 · 7-13일 주황 ·
    /// 14일+ 빨강 틴트. KPI의 Avg/Oldest는 도착 후 미연결 행만 집계한다.
    /// </summary>
    internal static class PendingTablePresenter
    {
        // ===== 상태 정의 =====

        /// <summary>상태 인덱스: 발송 통보(FAC_SEND_MAS)만 있고 도착 전(Sent).</summary>
        internal const int StatusTransit = 0;

        /// <summary>상태 인덱스: 도착했지만 Receive 미처리 — 수동 Receive 대상.</summary>
        internal const int StatusArrived = 1;

        /// <summary>상태 인덱스: Receive 완료, 의뢰서 미연결 — 경과일 강조 대상.</summary>
        internal const int StatusNoRequest = 2;

        /// <summary>상태 인덱스: 의뢰서 연결됨, Create 미처리 — 수동 Create 대상.</summary>
        internal const int StatusLinked = 3;

        /// <summary>상태 인덱스: Create 처리 완료.</summary>
        internal const int StatusCompleted = 4;

        /// <summary>상태 표시명 — 필터 콤보 값·STATUS 컬럼 값·KPI 집계가 같은 문자열을 쓴다.</summary>
        internal static readonly string[] StatusNames =
        {
            "Sent", "Arrived", "Request Unlinked", "Request Linked", "Completed"
        };

        // 상태 배지 배경색 (StatusNames와 같은 순서) —
        // 회색(운송 중)·파랑(도착)·호박(의뢰 대기)·남색 틴트(연결)·초록(완료).
        private static readonly string[] statusColors =
        {
            "#E5E7EB", "#DBEAFE", "#FEF3C7", "#E0E7FF", "#DCFCE7"
        };

        // 경과일 구간별 배지 배경색 (0-2 / 3-6 / 7-13 / 14+ 일 — 구간이
        // 심해질수록 파랑 → 호박 → 주황 → 빨강 틴트).
        private static readonly string[] agingBadgeColors = { "#DBEAFE", "#FEF3C7", "#FFE0CC", "#FEE2E2" };

        // 발송 통보 여부(SEND_YN) 배지색 — Y(FAC_SEND_MAS 있음) 초록 / N(미확인) 빨강 틴트.
        private const string sendNotifiedColor = "#DCFCE7";
        private const string sendUnmatchedColor = "#FEE2E2";

        /// <summary>집계 결과 — 폼은 이 값을 KPI 배지에 그대로 표기만 한다.</summary>
        internal sealed class PendingSummary
        {
            /// <summary>상태별 건수 — StatusNames와 같은 순서.</summary>
            internal readonly int[] StatusCounts = new int[5];

            /// <summary>발송 통보 없이 도착한 미확인 물류 건수(SEND_YN='N').</summary>
            internal int UnmatchedCount;

            /// <summary>도착 &amp; 의뢰서 미연결 건수(Arrived + Unlinked) — 경과 통계의 대상.</summary>
            internal int NoLinkCount;

            /// <summary>미연결 건 경과일 평균 (NoLinkCount가 0이면 0).</summary>
            internal double DaysAverage;

            /// <summary>미연결 건 경과일 최대 (NoLinkCount가 0이면 0).</summary>
            internal int DaysMax;
        }

        // ===== 파생 컬럼 =====

        /// <summary>
        /// 현황판에 워크리스트 파생 컬럼을 보장하고 전 행에 채운다:
        /// STATUS/STATUS_COLOR(파이프라인 상태 배지), SEND_YN/SEND_COLOR
        /// (발송 통보 여부 배지), ELAPSED_DAYS/DAYS_COLOR(미연결 행의 경과 강조),
        /// CHK(벌크 체크박스), RECV_CAN/CRT_CAN(행 단위 Receive/Create 버튼 활성).
        /// </summary>
        internal static void ApplyWorkflowColumns(DataTable board)
        {
            if (board == null)
            {
                return;
            }

            EnsureColumn(board, "STATUS", typeof(string));
            EnsureColumn(board, "STATUS_COLOR", typeof(string));
            EnsureColumn(board, "SEND_YN", typeof(string));
            EnsureColumn(board, "SEND_COLOR", typeof(string));
            EnsureColumn(board, "ELAPSED_DAYS", typeof(int));
            EnsureColumn(board, "DAYS_COLOR", typeof(string));
            EnsureColumn(board, "CHK", typeof(bool));
            EnsureColumn(board, "RECV_CAN", typeof(bool));
            EnsureColumn(board, "CRT_CAN", typeof(bool));

            foreach (DataRow row in board.Rows)
            {
                ApplyStatus(row);

                if (row.IsNull("CHK"))
                {
                    row["CHK"] = false;
                }
            }
        }

        /// <summary>
        /// 한 행의 파이프라인 상태와 강조(상태 배지·경과일·버튼 활성)를 계산한다.
        /// </summary>
        internal static void ApplyStatus(DataRow row)
        {
            bool arrived = CellText(row, "ARRIVE_TM").Trim().Length > 0;
            bool received = CellText(row, "RECV_YN").Trim() == "Y";
            bool linked = CellText(row, "REQ_NO").Trim().Length > 0;
            bool created = CellText(row, "PROC_YN").Trim() == "Y";

            int status;

            if (!arrived)
            {
                status = StatusTransit;
            }
            else if (!received)
            {
                status = StatusArrived;
            }
            else if (!linked)
            {
                status = StatusNoRequest;
            }
            else if (!created)
            {
                status = StatusLinked;
            }
            else
            {
                status = StatusCompleted;
            }

            row["STATUS"] = StatusNames[status];
            row["STATUS_COLOR"] = statusColors[status];
            row["RECV_CAN"] = status == StatusArrived;
            row["CRT_CAN"] = status == StatusLinked;

            // 발송 통보 여부 — 'Y'가 아니면 미확인('N')으로 정규화한다
            // (회사 조인 쿼리에서 FAC_SEND_MAS 쪽이 NULL인 행 포함).
            bool notified = CellText(row, "SEND_YN").Trim() == "Y";
            row["SEND_YN"] = notified ? "Y" : "N";
            row["SEND_COLOR"] = notified ? sendNotifiedColor : sendUnmatchedColor;

            // 경과일: 도착 전(운송 중)은 발송 통보 시각 기준 — 운송 지연을
            // 그대로 보여준다. 도착 후에는 "의뢰서 미연결" 행만 — Receive
            // 여부와 무관하게 도착시각 기준으로 방치 기간을 보여준다.
            string agingBasis = string.Empty;

            if (!arrived)
            {
                agingBasis = CellText(row, "SEND_TM").Trim();
            }
            else if (!linked)
            {
                agingBasis = CellText(row, "ARRIVE_TM").Trim();
            }

            if (agingBasis.Length > 0)
            {
                int days = ElapsedDays(agingBasis);
                row["ELAPSED_DAYS"] = days;
                row["DAYS_COLOR"] = agingBadgeColors[AgingBand(days)];
            }
            else
            {
                row["ELAPSED_DAYS"] = DBNull.Value;
                row["DAYS_COLOR"] = string.Empty;
            }
        }

        /// <summary>bool 파생 컬럼(RECV_CAN/CRT_CAN/CHK)을 관용적으로 읽는다.</summary>
        internal static bool FlagSet(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return false;
            }

            object value = row[columnName];
            return value is bool && (bool)value;
        }

        // ===== 필터 + 집계 =====

        /// <summary>
        /// 상태 필터(StatusNames 값, "" = 전체) · 발송 통보 필터(""/Y/N) ·
        /// 경과일 최소값(0 = 전체)으로 현황판을 잘라낸다. 전부 클라이언트에서
        /// 처리한다 — 서버 조회는 조건 없이 원본만 준다. 경과일은 미연결 행만
        /// 값이 있으므로, 경과일 필터를 걸면 연결/미도착 행은 자연히 제외된다.
        /// </summary>
        internal static DataTable Filter(DataTable board, string statusFilter, string sendFilter, int minDays)
        {
            if (statusFilter.Length == 0 && sendFilter.Length == 0 && minDays <= 0)
            {
                return board;
            }

            DataTable filtered = board.Clone();

            foreach (DataRow row in board.Rows)
            {
                if (statusFilter.Length > 0 && CellText(row, "STATUS") != statusFilter)
                {
                    continue;
                }

                if (sendFilter.Length > 0 && CellText(row, "SEND_YN") != sendFilter)
                {
                    continue;
                }

                if (minDays > 0)
                {
                    if (row.IsNull("ELAPSED_DAYS")
                            || ParseDays(CellText(row, "ELAPSED_DAYS")) < minDays)
                    {
                        continue;
                    }
                }

                filtered.ImportRow(row);
            }

            return filtered;
        }

        /// <summary>
        /// KPI를 집계한다 — 상태별 건수(전체 현황), 미확인 물류 건수(SEND_YN='N'),
        /// 미연결 건의 경과 통계(Avg/Oldest).
        /// </summary>
        internal static PendingSummary Aggregate(DataTable result)
        {
            PendingSummary summary = new PendingSummary();

            if (result == null)
            {
                return summary;
            }

            int daysSum = 0;

            foreach (DataRow row in result.Rows)
            {
                int status = Array.IndexOf(StatusNames, CellText(row, "STATUS"));

                if (status >= 0)
                {
                    summary.StatusCounts[status] += 1;
                }

                if (CellText(row, "SEND_YN") == "N")
                {
                    summary.UnmatchedCount += 1;
                }

                // 운송 중 행의 경과일(발송 기준)은 미연결 통계에서 제외한다 —
                // Avg/Oldest는 "도착했는데 의뢰서가 없는" 건의 방치 기간 지표다.
                if (status != StatusTransit
                        && row.Table.Columns.Contains("ELAPSED_DAYS") && !row.IsNull("ELAPSED_DAYS"))
                {
                    summary.NoLinkCount += 1;

                    int days = ParseDays(CellText(row, "ELAPSED_DAYS"));
                    daysSum += days;

                    if (days > summary.DaysMax)
                    {
                        summary.DaysMax = days;
                    }
                }
            }

            if (summary.NoLinkCount > 0)
            {
                summary.DaysAverage = (double)daysSum / summary.NoLinkCount;
            }

            return summary;
        }

        // ===== 공용 헬퍼 =====

        /// <summary>서버 숫자 컬럼(JSON number)을 관용적으로 파싱한다 — 빈 값/소수 표기 모두 허용.</summary>
        internal static int ParseDays(string text)
        {
            double value;

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return (int)value;
            }

            return 0;
        }

        /// <summary>컬럼이 없거나(null 키 생략) DBNull인 경우를 빈 문자열로 읽는다.</summary>
        internal static string CellText(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            object value = row[columnName];
            return value == DBNull.Value || value == null ? string.Empty : value.ToString();
        }

        // 파생 컬럼이 없으면 만든다 — 서버가 이미 내려준 컬럼은 그대로 둔다.
        private static void EnsureColumn(DataTable table, string columnName, Type columnType)
        {
            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName, columnType);
            }
        }

        // 도착시각 → 오늘까지의 경과일 (해석 불가/미래 도착은 0일).
        private static int ElapsedDays(string arriveText)
        {
            DateTime arrived;

            if (!DateTime.TryParse(arriveText, out arrived))
            {
                return 0;
            }

            int days = (int)(DateTime.Today - arrived.Date).TotalDays;
            return days > 0 ? days : 0;
        }

        // 경과일 → 구간 인덱스 (0-2 / 3-6 / 7-13 / 14+).
        private static int AgingBand(int days)
        {
            if (days >= 14)
            {
                return 3;
            }

            if (days >= 7)
            {
                return 2;
            }

            if (days >= 3)
            {
                return 1;
            }

            return 0;
        }
    }
}
