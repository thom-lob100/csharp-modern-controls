using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// ★ 회사 환경 교체 지점 — Pending Requests 화면의 데모 데이터 빌더.
    ///
    /// 회사 환경에는 두 인터페이스 테이블이 실재한다:
    /// - FAC_SEND_MAS(ITEM_ID, BOX_ID, SEND_TM, SEND_FAC, RECV_YN(기본 'N'),
    ///   RECV_TM, RECV_DESC): 보내는 쪽 공장이 "아이템을 보낸다"는 신호를
    ///   먼저 적재하는 발송 통보 테이블.
    /// - 의뢰 인터페이스(가제 IF_REQ_MAS): 물류가 실제 도착하면 ITEM_ID와
    ///   물류도착시간(ARRIVE_TM)이 적재되고, 의뢰서와 연결되면 의뢰내용
    ///   (REQ_NO, SAMPLE_NM …)과 처리여부/처리시간(PROC_YN/PROC_TM)이 채워진다.
    ///
    /// 화면 기준은 두 테이블을 합친 **단일 현황판**(FULL OUTER JOIN 상당)이다 —
    /// 발송 통보 없이 도착한 미확인 물류도 의뢰 인터페이스는 타므로 동일 레벨의
    /// 행으로 취급하고, FAC_SEND_MAS 존재 여부만 SEND_YN('Y'/'N')으로 구분한다.
    ///
    /// 홈 환경에는 이 테이블들이 없으므로, 홈 API의 도착 목록(EVENT_TM)을 원료로
    /// 현황판을 ITEM_ID 해시 기반으로 **결정적으로** 시뮬레이션한다 — 조회할
    /// 때마다 같은 행은 같은 상태가 나와 화면 검증이 가능하다. 자동 처리가
    /// 일부만 되고 멈춘 경우(수신 실패·의뢰서 미작성·Create 실패)와 발송 통보
    /// 없는 미확인 물류(SEND_YN='N')를 골고루 섞는다.
    ///
    /// 수동 Receive/Create 처리는 서버(DB) 역할의 정적 저장소에 처리 시각과
    /// 함께 기록한다 — 회사 환경에서 인터페이스가 RECV_TM/PROC_TM에 SYSDATE를
    /// 적재하는 것에 해당한다. 화면은 처리 성공 후 **재조회**로 이 값을 받아
    /// 보여준다 (화면이 시각을 만들지 않는다).
    ///
    /// 매뉴얼 Receive(ManualReceive)는 **단일 전문**이다 — 조회해도 현황판에
    /// 나오지 않는(어떤 인터페이스에도 올라오지 않은) 아이템을 Item ID + 발송
    /// 공장 입력으로 **강제 수신 처리**한다. 서버가 체크와 수신 처리를 한 번에
    /// 수행하고 실패면 사유를 돌려준다 — 회사 체크 로직 중 데모는 중복 수신
    /// 거부만 흉내 낸다. 강제 수신된 아이템은 물류 도착 시간(ARRIVE_TM)도 수신
    /// 처리 시각으로 함께 적재되고, 재조회 시 현황판에 행으로 나타난다.
    ///
    /// 회사 적용 시 이 클래스를 통째로 지우고, 폼의 서버 조회 메서드가
    /// FAC_SEND_MAS FULL OUTER JOIN IF_REQ_MAS 결과(SEND_YN 포함)를 직접
    /// 내려받게 바꾸며, ProcessReceive/ProcessCreate/ManualReceive 호출은 회사
    /// 인터페이스(ITEM 생성 전문, 의뢰 처리 갱신, 수신 전문) 호출로 바꾼다.
    /// </summary>
    internal static class PendingInterfaceSimulator
    {
        // ===== 서버측 수동 처리 저장소 (데모의 DB 역할) =====

        // 수동 Receive 처리된 ITEM_ID → 서버가 찍은 처리 시각. 시뮬레이터는
        // 조회 때마다 해시로 상태를 재생성하므로, 처리 결과가 재조회에도
        // 유지되도록 서버측 상태를 따로 보관한다 — 회사 환경의 FAC_SEND_MAS
        // UPDATE(RECV_YN='Y', RECV_TM=SYSDATE)에 해당한다.
        private static readonly Dictionary<string, DateTime> manualReceives =
                new Dictionary<string, DateTime>(StringComparer.Ordinal);

        // 수동 Create 처리된 ITEM_ID → 서버가 찍은 처리 시각 — 의뢰 인터페이스
        // UPDATE(PROC_YN='Y', PROC_TM=SYSDATE)에 해당한다.
        private static readonly Dictionary<string, DateTime> manualCreates =
                new Dictionary<string, DateTime>(StringComparer.Ordinal);

        // 매뉴얼 Receive(다이얼로그 직접 입력)로 강제 수신된 ITEM_ID → 입력한
        // 발송 공장. 인터페이스에 없던 아이템이므로 재조회 시 이 저장소로
        // 현황판 행을 합성한다 (처리 시각은 manualReceives가 든다).
        private static readonly Dictionary<string, string> manualArrivals =
                new Dictionary<string, string>(StringComparer.Ordinal);

        // 마지막 조회(Build)에서 수신 완료(RECV_YN='Y')로 내려간 ITEM_ID —
        // 매뉴얼 Receive의 중복 수신 체크(서버 역할)에 쓴다.
        private static readonly HashSet<string> receivedBoardIds =
                new HashSet<string>(StringComparer.Ordinal);

        // 발송 공장 데모 값 — 공개 저장소라 실제 사내 명칭을 쓰지 않는다.
        private static readonly string[] sendFacilities = { "FAC-A", "FAC-B", "FAC-C" };

        // 의뢰서에 적힌 시료명 데모 값.
        private static readonly string[] sampleNames = { "Panel-A", "Panel-B", "Chem-7", "Special-X" };

        /// <summary>
        /// 발송 공장 데모 코드 목록 — 매뉴얼 Receive 다이얼로그의 콤보 원천.
        /// ★ 회사 적용 시 공장 코드 조회로 교체한다.
        /// </summary>
        internal static string[] SendFacilityCodes
        {
            get { return (string[])sendFacilities.Clone(); }
        }

        /// <summary>매뉴얼 Receive 단일 전문의 응답 — 성공 여부와 실패 사유.</summary>
        internal sealed class ManualReceiveResult
        {
            /// <summary>수신 처리 성공 여부.</summary>
            internal bool Success;

            /// <summary>실패 사유 (성공이면 빈 문자열) — 화면 표기용 영어 문구.</summary>
            internal string Message;
        }

        /// <summary>
        /// 수동 Receive 처리 — 회사 환경의 ITEM 생성 인터페이스 호출에 해당한다.
        /// 처리 시각(RECV_TM)은 화면이 아니라 여기(서버 역할)가 찍는다.
        /// </summary>
        internal static void ProcessReceive(string itemId)
        {
            manualReceives[itemId] = DateTime.Now;
        }

        /// <summary>
        /// 수동 Create 처리 — 회사 환경의 의뢰 처리 인터페이스 호출에 해당한다.
        /// 처리 시각(PROC_TM)은 화면이 아니라 여기(서버 역할)가 찍는다.
        /// </summary>
        internal static void ProcessCreate(string itemId)
        {
            manualCreates[itemId] = DateTime.Now;
        }

        /// <summary>
        /// 매뉴얼 Receive 단일 전문 — 회사 환경의 수신 전문 호출에 해당한다.
        /// 조회해도 현황판에 나오지 않는(어떤 인터페이스에도 올라오지 않은)
        /// 아이템을 Item ID + 발송 공장 입력으로 **강제 수신 처리**한다.
        /// 서버 체크(회사 로직)는 데모에서 중복 수신 거부만 흉내 낸다.
        /// </summary>
        internal static ManualReceiveResult ManualReceive(string itemId, string sendFac)
        {
            string id = (itemId ?? string.Empty).Trim().ToUpperInvariant();
            string fac = (sendFac ?? string.Empty).Trim();

            if (id.Length == 0 || fac.Length == 0)
            {
                return Fail("Enter an item ID and a send facility.");
            }

            if (manualReceives.ContainsKey(id) || receivedBoardIds.Contains(id))
            {
                return Fail("Item " + id + " is already received.");
            }

            return Succeed(id, fac);
        }

        // 수신 성공 — 서버가 처리 시각을 찍고(RECV_TM/ARRIVE_TM 공용), 재조회 시
        // 현황판 행 합성을 위해 발송 공장도 기록한다.
        private static ManualReceiveResult Succeed(string itemId, string sendFac)
        {
            manualReceives[itemId] = DateTime.Now;
            manualArrivals[itemId] = sendFac;

            ManualReceiveResult result = new ManualReceiveResult();
            result.Success = true;
            result.Message = string.Empty;
            return result;
        }

        private static ManualReceiveResult Fail(string message)
        {
            ManualReceiveResult result = new ManualReceiveResult();
            result.Success = false;
            result.Message = message;
            return result;
        }

        /// <summary>
        /// 도착 목록(홈 API 응답 — ITEM_ID/EVENT_TM/BOX_ID)을 원료로 단일
        /// 현황판을 만든다. 도착 전(운송 중) 발송 통보 행도 몇 건 합성해
        /// 파이프라인 전 단계가 화면에 나타나게 한다.
        /// </summary>
        internal static DataTable Build(DataTable arrivals)
        {
            DataTable board = CreateBoardTable();
            HashSet<string> boardIds = new HashSet<string>(StringComparer.Ordinal);

            // 수신 완료 ID 스냅샷을 이번 조회 기준으로 다시 만든다
            // (매뉴얼 Receive의 중복 수신 체크 원천).
            receivedBoardIds.Clear();

            if (arrivals != null)
            {
                foreach (DataRow arrival in arrivals.Rows)
                {
                    string itemId = PendingTablePresenter.CellText(arrival, "ITEM_ID").Trim();

                    if (itemId.Length == 0)
                    {
                        continue;
                    }

                    int hash = StableHash(itemId);
                    DateTime arrived;

                    if (!DateTime.TryParse(PendingTablePresenter.CellText(arrival, "EVENT_TM"), out arrived))
                    {
                        arrived = DateTime.Today;
                    }

                    string boxId = PendingTablePresenter.CellText(arrival, "BOX_ID").Trim();

                    if (boxId.Length == 0)
                    {
                        boxId = "BOX-" + ((hash % 90) + 10).ToString(CultureInfo.InvariantCulture);
                    }

                    // 10%는 발송 통보 없이 도착(SEND_YN='N') — 미확인 물류.
                    // 나머지 진행 단계(수신/의뢰/Create)는 통보 여부와 무관하게 탄다.
                    AddArrivalRow(board, itemId, boxId, arrived, hash, hash % 10 != 0);
                    boardIds.Add(itemId);
                }
            }

            AddInTransitRows(board);
            AddManualReceivedRows(board, boardIds);
            return board;
        }

        // 현황판 스키마 — 전부 문자열 컬럼(REST/인터페이스 텍스트 값 그대로).
        private static DataTable CreateBoardTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ITEM_ID", typeof(string));
            table.Columns.Add("SEND_YN", typeof(string));
            table.Columns.Add("BOX_ID", typeof(string));
            table.Columns.Add("SEND_FAC", typeof(string));
            table.Columns.Add("SEND_TM", typeof(string));
            table.Columns.Add("ARRIVE_TM", typeof(string));
            table.Columns.Add("RECV_YN", typeof(string));
            table.Columns.Add("RECV_TM", typeof(string));
            table.Columns.Add("ITEM_STAT", typeof(string));
            table.Columns.Add("REQ_NO", typeof(string));
            table.Columns.Add("SAMPLE_NM", typeof(string));
            table.Columns.Add("PROC_YN", typeof(string));
            table.Columns.Add("PROC_TM", typeof(string));
            table.Columns.Add("RECV_DESC", typeof(string));
            return table;
        }

        // 도착한 아이템 한 건의 발송/수신/의뢰 상태를 해시로 결정해 현황판에 넣는다.
        private static void AddArrivalRow(
                DataTable table, string itemId, string boxId, DateTime arrived, int hash, bool notified)
        {
            DataRow row = table.NewRow();
            row["ITEM_ID"] = itemId;
            row["SEND_YN"] = notified ? "Y" : "N";
            row["BOX_ID"] = boxId;
            row["ARRIVE_TM"] = FormatTime(arrived);

            // 발송 통보가 있는 행만 FAC_SEND_MAS 쪽 컬럼(발송 공장/발송 시각)이 있다.
            if (notified)
            {
                row["SEND_FAC"] = sendFacilities[hash % sendFacilities.Length];
                row["SEND_TM"] = FormatTime(arrived.AddHours(-((hash % 36) + 6)));
            }

            // 20%는 자동 Receive 실패(RECV_YN='N') — 이 화면의 수동 Receive 대상.
            bool received = (hash % 5) != 1;

            // 33%는 의뢰서 미작성 — 경과일을 강조해 현업에게 보여줄 대상.
            bool linked = (hash % 3) != 0;

            // 연결 + 수신까지 된 건 중 25%는 자동 Create 실패 — 수동 Create 대상.
            bool created = linked && received && (hash % 4) != 0;

            // 수동 처리 저장소 오버레이 — 이 화면에서 Receive/Create 처리한 건은
            // 재조회에도 처리 완료 상태로 내려간다 (DB에 적재된 것과 동일).
            DateTime manualReceiveTime;
            bool manualReceive = manualReceives.TryGetValue(itemId, out manualReceiveTime);

            DateTime manualCreateTime;
            bool manualCreate = manualCreates.TryGetValue(itemId, out manualCreateTime);

            if (manualReceive)
            {
                received = true;
            }

            if (manualCreate)
            {
                created = true;
            }

            row["RECV_YN"] = received ? "Y" : "N";

            if (received)
            {
                row["RECV_TM"] = manualReceive
                        ? FormatTime(manualReceiveTime)
                        : FormatTime(arrived.AddMinutes((hash % 50) + 5));
                row["ITEM_STAT"] = "Released";
                row["RECV_DESC"] = manualReceive ? "Manual receive" : "Auto received";
                receivedBoardIds.Add(itemId);
            }

            if (linked)
            {
                row["REQ_NO"] = "REQ-" + arrived.ToString("yyMMdd", CultureInfo.InvariantCulture)
                        + "-" + ((hash % 900) + 100).ToString(CultureInfo.InvariantCulture);
                row["SAMPLE_NM"] = sampleNames[hash % sampleNames.Length];
            }

            row["PROC_YN"] = created ? "Y" : "N";

            if (created)
            {
                row["PROC_TM"] = manualCreate
                        ? FormatTime(manualCreateTime)
                        : FormatTime(arrived.AddMinutes((hash % 120) + 60));
            }

            table.Rows.Add(row);
        }

        // 발송 통보만 있고 아직 도착하지 않은(운송 중) 행을 합성한다.
        // 매뉴얼 Receive로 수신된 행은 건너뛴다 — AddManualReceivedRows가
        // 수신 완료 상태로 대신 올린다.
        private static void AddInTransitRows(DataTable table)
        {
            DateTime now = DateTime.Now;

            for (int index = 1; index <= 4; index++)
            {
                string itemId = InTransitItemId(now, index);

                if (manualArrivals.ContainsKey(itemId))
                {
                    continue;
                }

                DataRow row = table.NewRow();
                row["ITEM_ID"] = itemId;
                row["SEND_YN"] = "Y";
                row["BOX_ID"] = "BOX-T" + index.ToString("D2", CultureInfo.InvariantCulture);
                row["SEND_FAC"] = sendFacilities[index % sendFacilities.Length];
                row["SEND_TM"] = FormatTime(InTransitSendTime(now, index));
                row["RECV_YN"] = "N";
                row["PROC_YN"] = "N";
                table.Rows.Add(row);
            }
        }

        // 운송 중(Sent) 합성 행의 발송 시각 — 발송 후 경과일(운송 지연) 배지가
        // 구간별로 보이도록 0일/1일/4일/9일 전으로 분산시킨다.
        private static DateTime InTransitSendTime(DateTime now, int index)
        {
            int[] sendAgeHours = { 5, 30, 96, 216 };
            return now.AddHours(-sendAgeHours[(index - 1) % sendAgeHours.Length]);
        }

        // 운송 중(Sent) 합성 행의 ITEM_ID — AddInTransitRows와 ManualReceive가
        // 같은 규칙을 써야 매뉴얼 수신 검증이 맞아떨어진다.
        private static string InTransitItemId(DateTime now, int index)
        {
            return "SHIP-" + now.ToString("yyMM", CultureInfo.InvariantCulture)
                    + "-" + index.ToString("D3", CultureInfo.InvariantCulture);
        }

        // 매뉴얼 Receive로 강제 수신됐지만 도착 목록(원료)에 없는 아이템을
        // 현황판에 수신 완료 행으로 합성한다 — 인터페이스에 없던 아이템이라
        // 물류 도착 시간(ARRIVE_TM)도 수신 처리 시각으로 적재된 상태다.
        // 의뢰서는 아직 없으므로 상태는 Unlinked로 파생된다.
        private static void AddManualReceivedRows(DataTable table, HashSet<string> boardIds)
        {
            DateTime now = DateTime.Now;

            foreach (KeyValuePair<string, string> entry in manualArrivals)
            {
                if (boardIds.Contains(entry.Key))
                {
                    continue;
                }

                DateTime receiveTime = manualReceives[entry.Key];

                DataRow row = table.NewRow();
                row["ITEM_ID"] = entry.Key;

                // 운송 중(Sent) 합성 행이었으면 발송 통보 정보를 유지하고, 그 외는
                // 어떤 인터페이스에도 없던 강제 수신이라 발송 통보 없음('N') +
                // 수신 처리 때 입력받은 발송 공장만 적재된 상태로 만든다.
                int transitIndex = InTransitIndex(now, entry.Key);

                if (transitIndex > 0)
                {
                    row["SEND_YN"] = "Y";
                    row["SEND_FAC"] = sendFacilities[transitIndex % sendFacilities.Length];
                    row["SEND_TM"] = FormatTime(InTransitSendTime(now, transitIndex));
                    row["BOX_ID"] = "BOX-T" + transitIndex.ToString("D2", CultureInfo.InvariantCulture);
                }
                else
                {
                    row["SEND_YN"] = "N";
                    row["SEND_FAC"] = entry.Value;
                }

                row["ARRIVE_TM"] = FormatTime(receiveTime);
                row["RECV_YN"] = "Y";
                row["RECV_TM"] = FormatTime(receiveTime);
                row["ITEM_STAT"] = "Released";
                row["RECV_DESC"] = "Manual receive";
                row["PROC_YN"] = "N";
                table.Rows.Add(row);
                receivedBoardIds.Add(entry.Key);
            }
        }

        // ITEM_ID가 운송 중(Sent) 합성 행이면 그 순번(1~4), 아니면 0.
        private static int InTransitIndex(DateTime now, string itemId)
        {
            for (int index = 1; index <= 4; index++)
            {
                if (itemId == InTransitItemId(now, index))
                {
                    return index;
                }
            }

            return 0;
        }

        // ITEM_ID → 안정 해시 (조회할 때마다 같은 행이 같은 상태가 나오게 한다).
        private static int StableHash(string text)
        {
            int hash = 0;

            foreach (char ch in text)
            {
                hash = ((hash * 31) + ch) & 0x7FFFFFFF;
            }

            return hash;
        }

        // 인터페이스 시각 표기 공통 형식.
        private static string FormatTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
