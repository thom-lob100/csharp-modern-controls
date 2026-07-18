using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// ★ 회사 환경 교체 지점 — Equipment / Lots 화면의 데모 데이터 + 서버 역할.
    ///
    /// 회사 환경에서 이 화면의 원천은 장비 마스터/포트 현황과 대기 Lot 큐다:
    /// - 장비: 장비그룹, Down 여부, 인포트/아웃포트 수(2/2, 2/1, 1/1 등)와
    ///   각 포트의 점유 상태(비어있음/투입됨(Loaded)/작업중(Running)/완료(Done)).
    /// - 대기 Lot: 그룹별 투입 대기 큐 — 우선순위 순으로 투입한다.
    ///
    /// 처리(투입/작업시작/작업종료/반출)는 전부 서버가 검증 + 시각 적재를
    /// 수행하고, 화면은 성공 후 **재조회**로 결과를 받아 보여준다 — Pending
    /// Requests 화면과 같은 패턴이다. 실패면 사유를 돌려준다.
    ///
    /// 홈 환경에는 장비 시스템이 없으므로 그룹별 고정 시드로 인메모리 상태를
    /// 만들어 서버처럼 동작한다. 회사 적용 시 이 클래스를 통째로 지우고, 폼의
    /// 조회/처리 메서드가 회사 장비 인터페이스를 직접 호출하게 바꾼다.
    /// </summary>
    internal static class EquipmentLotSimulator
    {
        /// <summary>처리 전문의 응답 — 성공 여부와 실패 사유.</summary>
        internal sealed class ActionResult
        {
            /// <summary>처리 성공 여부.</summary>
            internal bool Success;

            /// <summary>실패 사유 (성공이면 빈 문자열) — 화면 표기용 영어 문구.</summary>
            internal string Message;

            /// <summary>처리된 Lot ID — 서버가 대상을 정하는 처리(Prepare)의
            /// 응답에만 채워진다.</summary>
            internal string LotId = string.Empty;
        }

        // ===== 인메모리 서버 상태 =====

        // 포트 한 칸 — 비어 있으면 LotId가 빈 문자열이다.
        private sealed class Port
        {
            internal string LotId = string.Empty;

            // "Loaded"(투입됨) / "Running"(작업중) / "Done"(완료, 아웃포트) / "".
            internal string Stat = string.Empty;

            // 마지막 상태 변경 시각 (서버가 찍는다).
            internal DateTime Time;

            // Lot 수량 — 취소로 대기 큐에 되돌릴 때 함께 복원한다.
            internal int Qty = 10;

            // 이 포트의 Lot이 담긴 캐리어 — 인포트는 Lot이 자기 캐리어째
            // 들어오고, 아웃포트(Done)는 작업준비 때 배정한 아웃 캐리어다.
            internal string CarrierId = string.Empty;

            // 인포트 전용: 작업준비 때 지정한 아웃포트 번호(1-기준, 0 = 없음).
            // 지정된 아웃포트는 이 작업에 **예약**되어 다른 투입이 쓸 수 없고,
            // 작업종료 시 Lot이 이 포트로 나간다.
            internal int OutIndex;

            // 인포트 전용: 작업준비 때 배정한 아웃 캐리어 — 작업종료 시 Lot이
            // 이 캐리어에 담겨 아웃포트로 나간다 (인 캐리어는 빈 캐리어 풀로
            // 복귀).
            internal string OutCarrierId = string.Empty;
        }

        private sealed class Equipment
        {
            internal string Id;
            internal bool Down;

            // 통신 모드 — "OnLineLocal"(전부 수동 가능) / "OnLineRemote"(자동
            // 통신으로 진행 — 수동은 Prepare만) / "OffLine"(통신 끊김 — 작업
            // 처리 불가, 모드 전환만 가능).
            internal string CommMode = "OnLineLocal";
            internal Port[] InPorts;
            internal Port[] OutPorts;
        }

        private sealed class WaitingLot
        {
            internal string LotId;
            internal int Priority;
            internal int Quantity;
            internal DateTime ArriveTime;

            // Lot이 담겨 있는 캐리어 — 투입 시 캐리어째 인포트로 들어간다.
            internal string CarrierId = string.Empty;
        }

        private sealed class GroupState
        {
            internal readonly List<Equipment> Equipments = new List<Equipment>();
            internal readonly List<WaitingLot> Lots = new List<WaitingLot>();

            // 빈 캐리어 풀 — 작업준비 때 아웃 캐리어를 여기서 배정하고,
            // 작업종료 때 비워진 인 캐리어가 복귀한다.
            internal readonly List<string> EmptyCarriers = new List<string>();
        }

        // 그룹 코드 → 상태. 첫 접근 때 고정 시드로 만들어 프로세스 동안 유지한다.
        private static readonly Dictionary<string, GroupState> groups =
                new Dictionary<string, GroupState>(StringComparer.Ordinal);

        /// <summary>장비그룹 데모 코드 목록 — 그룹 콤보 원천.
        /// ★ 회사 적용 시 장비그룹 조회로 교체한다.</summary>
        internal static string[] GroupCodes
        {
            get { return new string[] { "GRP-A", "GRP-B", "GRP-C" }; }
        }

        // ===== 조회 (★ 회사 장비 현황 조회로 교체) =====

        /// <summary>그룹의 장비/포트 현황 스냅샷 — 원본 컬럼만 준다
        /// (상태·색·버튼 활성 파생은 EquipmentTablePresenter가 한다).</summary>
        internal static DataTable GetEquipments(string group)
        {
            DataTable table = new DataTable();
            table.Columns.Add("EQP_ID", typeof(string));
            table.Columns.Add("EQP_GRP", typeof(string));
            table.Columns.Add("DOWN_YN", typeof(string));
            table.Columns.Add("COMM_MODE", typeof(string));
            table.Columns.Add("IN_CNT", typeof(int));
            table.Columns.Add("OUT_CNT", typeof(int));

            for (int index = 1; index <= 2; index++)
            {
                table.Columns.Add("IN" + index + "_LOT", typeof(string));
                table.Columns.Add("IN" + index + "_STAT", typeof(string));
                table.Columns.Add("IN" + index + "_TM", typeof(string));
                table.Columns.Add("IN" + index + "_CAR", typeof(string));
                table.Columns.Add("IN" + index + "_OUT", typeof(int));
                table.Columns.Add("OUT" + index + "_LOT", typeof(string));
                table.Columns.Add("OUT" + index + "_STAT", typeof(string));
                table.Columns.Add("OUT" + index + "_TM", typeof(string));
                table.Columns.Add("OUT" + index + "_CAR", typeof(string));
                table.Columns.Add("OUT" + index + "_RESV_LOT", typeof(string));
                table.Columns.Add("OUT" + index + "_RESV_CAR", typeof(string));
            }

            foreach (Equipment equipment in GetGroup(group).Equipments)
            {
                DataRow row = table.NewRow();
                row["EQP_ID"] = equipment.Id;
                row["EQP_GRP"] = group;
                row["DOWN_YN"] = equipment.Down ? "Y" : "N";
                row["COMM_MODE"] = equipment.CommMode;
                row["IN_CNT"] = equipment.InPorts.Length;
                row["OUT_CNT"] = equipment.OutPorts.Length;
                FillPorts(row, "IN", equipment.InPorts);
                FillPorts(row, "OUT", equipment.OutPorts);

                // 인포트의 지정 아웃포트 + 아웃포트를 예약 중인 Lot — 화면이
                // "이 작업이 어디로 나가는지 / 이 포트가 왜 못 쓰는지"를 보여준다.
                for (int index = 0; index < equipment.InPorts.Length; index++)
                {
                    row["IN" + (index + 1) + "_OUT"] = equipment.InPorts[index].OutIndex;
                }

                for (int index = 1; index <= equipment.OutPorts.Length; index++)
                {
                    Port reserver = FindReservingPort(equipment, index);
                    row["OUT" + index + "_RESV_LOT"] = reserver != null ? reserver.LotId : string.Empty;
                    row["OUT" + index + "_RESV_CAR"] = reserver != null ? reserver.OutCarrierId : string.Empty;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>그룹의 대기 Lot 큐 스냅샷 — 우선순위 오름차순(급한 순).
        /// CARRIER는 Lot이 담겨 있는 캐리어(투입 시 캐리어째 들어간다).</summary>
        internal static DataTable GetWaitingLots(string group)
        {
            DataTable table = new DataTable();
            table.Columns.Add("LOT_ID", typeof(string));
            table.Columns.Add("CARRIER", typeof(string));
            table.Columns.Add("PRIORITY", typeof(int));
            table.Columns.Add("QTY", typeof(int));
            table.Columns.Add("ARRIVE_TM", typeof(string));

            List<WaitingLot> lots = new List<WaitingLot>(GetGroup(group).Lots);
            lots.Sort(delegate(WaitingLot left, WaitingLot right)
            {
                return left.Priority.CompareTo(right.Priority);
            });

            foreach (WaitingLot lot in lots)
            {
                table.Rows.Add(
                        lot.LotId, lot.CarrierId, lot.Priority, lot.Quantity,
                        FormatTime(lot.ArriveTime));
            }

            return table;
        }

        /// <summary>그룹의 빈 캐리어 풀 스냅샷 — 작업준비 때 아웃 캐리어를
        /// 여기서 고른다. ★ 회사 적용 시 빈 캐리어 조회로 교체한다.</summary>
        internal static DataTable GetEmptyCarriers(string group)
        {
            DataTable table = new DataTable();
            table.Columns.Add("CARRIER_ID", typeof(string));

            List<string> carriers = new List<string>(GetGroup(group).EmptyCarriers);
            carriers.Sort(StringComparer.Ordinal);

            foreach (string carrier in carriers)
            {
                table.Rows.Add(carrier);
            }

            return table;
        }

        /// <summary>작업종료 대상(작업중 Lot)의 슬롯 현황 — 캐리어(듀러블)의
        /// 슬롯별 웨이퍼 장착 상태다. 작업종료 다이얼로그가 슬롯별 판정
        /// (JUDGE_RSLT)을 입력받는 원천으로, WF_ID가 빈 슬롯(빈 자리)은 판정
        /// 대상이 아니다. 작업중 Lot이 없으면 빈 테이블을 돌려준다.
        /// ★ 회사 적용 시 슬롯 맵 조회로 교체한다.</summary>
        internal static DataTable GetEndJobSlots(string group, string eqpId)
        {
            DataTable table = new DataTable();
            table.Columns.Add("DURABLE_ID", typeof(string));
            table.Columns.Add("DURABLE_TYP", typeof(string));
            table.Columns.Add("SUB_DURABLE_TYP", typeof(string));
            table.Columns.Add("SLOT_NO", typeof(int));
            table.Columns.Add("FINGER_ID", typeof(string));
            table.Columns.Add("WF_ID", typeof(string));
            table.Columns.Add("JUDGE_RSLT", typeof(string));

            Equipment equipment = FindEquipment(group, eqpId);
            Port running = equipment != null ? FindPort(equipment.InPorts, "Running") : null;

            if (running == null)
            {
                return table;
            }

            // 데모 슬롯 맵 — 캐리어 6슬롯에 Lot 수량만큼 웨이퍼가 앞에서부터
            // 장착된 구성. 뒤쪽 빈 슬롯은 판정 대상이 아님을 보여준다.
            // 웨이퍼 목록은 종료 검증(EndJob)과 같은 원천(BuildEndJobWafers)이다.
            string[] wafers = BuildEndJobWafers(running);

            for (int slot = 1; slot <= wafers.Length; slot++)
            {
                table.Rows.Add(
                        running.CarrierId,
                        "CARRIER",
                        "FOUP",
                        slot,
                        "FNG-" + slot.ToString("00", CultureInfo.InvariantCulture),
                        wafers[slot - 1],
                        string.Empty);
            }

            return table;
        }

        // 작업중 Lot의 슬롯별 웨이퍼 ID — 앞에서부터 수량만큼 장착, 빈 슬롯은
        // 빈 문자열. 웨이퍼 ID는 Lot ID에서 유도한다 ("LOT-A310" → "WF-A310.01").
        private const int endJobSlotCount = 6;

        private static string[] BuildEndJobWafers(Port running)
        {
            string[] wafers = new string[endJobSlotCount];
            int waferCount = Math.Min(running.Qty, endJobSlotCount);
            string waferBase = running.LotId.Length > 3
                    ? "WF" + running.LotId.Substring(3)
                    : running.LotId;

            for (int slot = 1; slot <= endJobSlotCount; slot++)
            {
                wafers[slot - 1] = slot <= waferCount
                        ? waferBase + "." + slot.ToString("00", CultureInfo.InvariantCulture)
                        : string.Empty;
            }

            return wafers;
        }

        // ===== 처리 (★ 회사 장비 인터페이스 호출로 교체) =====

        /// <summary>작업준비(투입) — 대기 Lot을 **지정한 인포트**에 장착하고
        /// **지정한 아웃포트를 이 작업에 예약**하며 **아웃 캐리어를 배정**한다
        /// (Loaded). In은 Lot이 자기 캐리어째 들어오므로 그대로지만, Out은
        /// 별도의 빈 캐리어가 있어야 한다 — 작업종료 시 Lot이 그 캐리어에
        /// 담겨 나간다. 서버 검증: 장비 존재/Down 아님/인포트 빔/아웃포트
        /// 빔+미예약/아웃 캐리어가 빈 캐리어 풀에 있음/Lot이 큐에 있음.
        /// 포트 번호는 1-기준이다.</summary>
        internal static ActionResult AssignLot(
                string group, string eqpId, string lotId, int inPort, int outPort, string outCarrier)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Select an equipment first.");
            }

            if (equipment.Down)
            {
                return Fail("Equipment " + eqpId + " is down.");
            }

            if (equipment.CommMode == "OffLine")
            {
                return Fail("Equipment " + eqpId + " is offline — switch to online first.");
            }

            if (inPort < 1 || inPort > equipment.InPorts.Length)
            {
                return Fail("In-port " + inPort + " not found on " + eqpId + ".");
            }

            if (equipment.InPorts[inPort - 1].Stat.Length > 0)
            {
                return Fail("In-port In " + inPort + " on " + eqpId + " is occupied.");
            }

            if (outPort < 1 || outPort > equipment.OutPorts.Length)
            {
                return Fail("Out-port " + outPort + " not found on " + eqpId + ".");
            }

            if (equipment.OutPorts[outPort - 1].Stat.Length > 0)
            {
                return Fail("Out-port Out " + outPort + " on " + eqpId + " is occupied.");
            }

            if (FindReservingPort(equipment, outPort) != null)
            {
                return Fail("Out-port Out " + outPort + " on " + eqpId + " is reserved by another job.");
            }

            GroupState state = GetGroup(group);
            string carrier = (outCarrier ?? string.Empty).Trim();

            if (carrier.Length == 0)
            {
                return Fail("Select an out carrier.");
            }

            if (!state.EmptyCarriers.Contains(carrier))
            {
                return Fail("Carrier " + carrier + " is not available.");
            }

            WaitingLot waiting = null;

            foreach (WaitingLot lot in state.Lots)
            {
                if (lot.LotId == lotId)
                {
                    waiting = lot;
                    break;
                }
            }

            if (waiting == null)
            {
                return Fail("Lot " + lotId + " is not in the waiting queue.");
            }

            state.Lots.Remove(waiting);
            state.EmptyCarriers.Remove(carrier);

            Port target = equipment.InPorts[inPort - 1];
            target.LotId = lotId;
            target.Stat = "Loaded";
            target.Time = DateTime.Now;
            target.Qty = waiting.Quantity;
            target.CarrierId = waiting.CarrierId;
            target.OutIndex = outPort;
            target.OutCarrierId = carrier;

            ActionResult result = Succeed();
            result.LotId = lotId;
            return result;
        }

        /// <summary>작업준비 — 대기 큐의 **최우선** Lot을 지정 포트/아웃
        /// 캐리어로 투입한다 (우선순위 투입의 표준 동선; 대상 Lot은 서버가
        /// 정해서 응답 LotId로 돌려준다). 특정 Lot 지정 투입은 AssignLot을 쓴다.</summary>
        internal static ActionResult Prepare(
                string group, string eqpId, int inPort, int outPort, string outCarrier)
        {
            GroupState state = GetGroup(group);
            WaitingLot top = null;

            foreach (WaitingLot lot in state.Lots)
            {
                if (top == null || lot.Priority < top.Priority)
                {
                    top = lot;
                }
            }

            if (top == null)
            {
                return Fail("No waiting lot to prepare.");
            }

            return AssignLot(group, eqpId, top.LotId, inPort, outPort, outCarrier);
        }

        /// <summary>작업시작 — 투입된(Loaded) 인포트 Lot을 작업중(Running)으로
        /// 바꾼다. **작업은 장비당 한 번에 하나** — 이미 작업중인 Lot이 있으면
        /// 먼저 끝내야(End) 다음 Lot을 시작할 수 있다. 시작 시각은 서버가 찍는다.
        /// 수동 시작은 OnLineLocal에서만 — OnLineRemote는 장비가 자동 통신으로
        /// 진행하고, OffLine은 통신이 끊겨 처리할 수 없다.</summary>
        internal static ActionResult StartJob(string group, string eqpId)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Equipment not found.");
            }

            if (equipment.Down)
            {
                return Fail("Equipment " + eqpId + " is down.");
            }

            ActionResult modeCheck = CheckManualJobAllowed(equipment, "start");

            if (modeCheck != null)
            {
                return modeCheck;
            }

            if (FindPort(equipment.InPorts, "Running") != null)
            {
                return Fail("Equipment " + eqpId + " is already running — end the current job first.");
            }

            Port loaded = FindPort(equipment.InPorts, "Loaded");

            if (loaded == null)
            {
                return Fail("No loaded lot to start on " + eqpId + ".");
            }

            loaded.Stat = "Running";
            loaded.Time = DateTime.Now;
            return Succeed();
        }

        /// <summary>작업종료 — 작업중(Running) Lot을 **작업준비 때 지정한
        /// 아웃포트**로 옮긴다(Done). 그 포트가 점유돼 있으면 실패한다 —
        /// 먼저 반출(Unload)해야 한다. 수동 종료도 OnLineLocal에서만 가능하다.
        /// judgeResults는 슬롯별 판정 입력의 **변경분**(화면이 GetChanges로
        /// 추린 입력 행만) — 서버가 보유한 슬롯 맵 기준으로 웨이퍼(WF_ID)가
        /// 있는 슬롯 전부에 SUCC/FAIL이 있는지 검증한다. 데모는 검증만 하고
        /// 저장하지 않는다 (회사는 종료 전문에 함께 실린다).</summary>
        internal static ActionResult EndJob(string group, string eqpId, DataTable judgeResults = null)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Equipment not found.");
            }

            ActionResult modeCheck = CheckManualJobAllowed(equipment, "end");

            if (modeCheck != null)
            {
                return modeCheck;
            }

            Port running = FindPort(equipment.InPorts, "Running");

            if (running == null)
            {
                return Fail("No running lot on " + eqpId + ".");
            }

            // 슬롯 판정 검증 — 화면은 **입력(변경)된 행만** 보내므로, 완결성은
            // 서버가 보유한 슬롯 맵 기준으로 검증한다: 웨이퍼가 있는 슬롯
            // 전부에 SUCC/FAIL 판정이 전문 안에 있어야 한다.
            foreach (string waferId in BuildEndJobWafers(running))
            {
                if (waferId.Length == 0)
                {
                    continue;
                }

                string judge = FindJudge(judgeResults, waferId);

                if (judge != "SUCC" && judge != "FAIL")
                {
                    return Fail("Judge result is missing for wafer " + waferId + ".");
                }
            }

            Port target;
            string targetLabel;

            if (running.OutIndex >= 1 && running.OutIndex <= equipment.OutPorts.Length)
            {
                target = equipment.OutPorts[running.OutIndex - 1];
                targetLabel = "Out " + running.OutIndex;
            }
            else
            {
                // 지정이 없는 예외 데이터 — 비어 있고 예약 안 된 첫 아웃포트로.
                target = null;
                targetLabel = string.Empty;

                for (int index = 1; index <= equipment.OutPorts.Length; index++)
                {
                    if (equipment.OutPorts[index - 1].Stat.Length == 0
                            && FindReservingPort(equipment, index) == null)
                    {
                        target = equipment.OutPorts[index - 1];
                        targetLabel = "Out " + index;
                        break;
                    }
                }

                if (target == null)
                {
                    return Fail("No free out-port on " + eqpId + " — unload first.");
                }
            }

            if (target.Stat.Length > 0)
            {
                return Fail("Out-port " + targetLabel + " on " + eqpId + " is occupied — unload first.");
            }

            // Lot은 작업준비 때 배정한 아웃 캐리어에 담겨 나가고, 비워진
            // 인 캐리어는 빈 캐리어 풀로 복귀한다.
            target.LotId = running.LotId;
            target.Stat = "Done";
            target.Time = DateTime.Now;
            target.CarrierId = running.OutCarrierId;

            if (running.CarrierId.Length > 0)
            {
                GetGroup(group).EmptyCarriers.Add(running.CarrierId);
            }

            running.LotId = string.Empty;
            running.Stat = string.Empty;
            running.CarrierId = string.Empty;
            running.OutIndex = 0;
            running.OutCarrierId = string.Empty;
            return Succeed();
        }

        /// <summary>취소 — 인포트의 투입됨(Loaded)/작업중(Running) Lot을 빼서
        /// 대기 큐 **최우선**으로 되돌린다 (취소한 Lot은 보통 곧 재투입한다).
        /// 아웃포트의 완료 Lot은 취소 대상이 아니라 반출(Unload)로 처리한다.
        /// 포트 번호는 인포트 1-기준이다.</summary>
        internal static ActionResult CancelPort(string group, string eqpId, int inPort)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Equipment not found.");
            }

            if (inPort < 1 || inPort > equipment.InPorts.Length)
            {
                return Fail("In-port " + inPort + " not found on " + eqpId + ".");
            }

            if (equipment.CommMode == "OffLine")
            {
                return Fail("Equipment " + eqpId + " is offline — switch to online first.");
            }

            Port port = equipment.InPorts[inPort - 1];

            if (port.Stat != "Loaded" && port.Stat != "Running")
            {
                return Fail("No lot to cancel on " + eqpId + " in-port " + inPort + ".");
            }

            // 큐 맨 앞(현재 최소 우선순위 - 1)으로 복귀시킨다.
            GroupState state = GetGroup(group);
            int topPriority = 1;

            foreach (WaitingLot lot in state.Lots)
            {
                if (lot.Priority <= topPriority)
                {
                    topPriority = lot.Priority;
                }
            }

            WaitingLot returned = new WaitingLot();
            returned.LotId = port.LotId;
            returned.Priority = state.Lots.Count > 0 ? topPriority - 1 : 1;
            returned.Quantity = port.Qty;
            returned.ArriveTime = DateTime.Now;
            returned.CarrierId = port.CarrierId;
            state.Lots.Add(returned);

            // 배정해 뒀던 아웃 캐리어는 빈 캐리어 풀로 되돌린다.
            if (port.OutCarrierId.Length > 0)
            {
                state.EmptyCarriers.Add(port.OutCarrierId);
            }

            port.LotId = string.Empty;
            port.Stat = string.Empty;
            port.CarrierId = string.Empty;
            port.OutIndex = 0;
            port.OutCarrierId = string.Empty;
            return Succeed();
        }

        /// <summary>Down 설정/해제 — 설정하면 신규 투입/시작이 막힌다 (진행 중
        /// 작업은 그대로 둔다 — 강제 중단이 필요하면 취소를 먼저 한다).</summary>
        internal static ActionResult SetDown(string group, string eqpId, bool down)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Select an equipment first.");
            }

            if (equipment.Down == down)
            {
                return Fail("Equipment " + eqpId + (down ? " is already down." : " is not down."));
            }

            equipment.Down = down;
            return Succeed();
        }

        /// <summary>통신 모드 변경 — OnLineLocal / OnLineRemote / OffLine.
        /// 작업자가 장비 이상 등 판단에 따라 수시로 바꿀 수 있다 (예: Remote
        /// 자동 진행 중 오류가 나면 Local로 내려 수동 처리). 진행 중 작업
        /// 상태는 건드리지 않는다.</summary>
        internal static ActionResult SetCommMode(string group, string eqpId, string mode)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Select an equipment first.");
            }

            if (mode != "OnLineLocal" && mode != "OnLineRemote" && mode != "OffLine")
            {
                return Fail("Unknown communication mode: " + mode + ".");
            }

            if (equipment.CommMode == mode)
            {
                return Fail("Equipment " + eqpId + " is already " + mode + ".");
            }

            equipment.CommMode = mode;
            return Succeed();
        }

        /// <summary>우선순위 변경 — 대기 큐에서 한 칸 위/아래 Lot과 우선순위
        /// 값을 맞바꾼다. 서버가 저장하므로 재조회 시 바뀐 순서로 내려온다.</summary>
        internal static ActionResult MoveLotPriority(string group, string lotId, bool up)
        {
            GroupState state = GetGroup(group);
            List<WaitingLot> sorted = new List<WaitingLot>(state.Lots);
            sorted.Sort(delegate(WaitingLot left, WaitingLot right)
            {
                return left.Priority.CompareTo(right.Priority);
            });

            int index = -1;

            for (int position = 0; position < sorted.Count; position++)
            {
                if (sorted[position].LotId == lotId)
                {
                    index = position;
                    break;
                }
            }

            if (index < 0)
            {
                return Fail("Lot " + lotId + " is not in the waiting queue.");
            }

            int neighbor = up ? index - 1 : index + 1;

            if (neighbor < 0 || neighbor >= sorted.Count)
            {
                return Fail("Lot " + lotId + " is already at the " + (up ? "top" : "bottom") + ".");
            }

            int priority = sorted[index].Priority;
            sorted[index].Priority = sorted[neighbor].Priority;
            sorted[neighbor].Priority = priority;
            return Succeed();
        }

        /// <summary>단일 포트 반출 — 지정한 아웃포트(1-기준)의 완료(Done)
        /// Lot만 비운다. 포트를 집어서 하는 처리라 포트 컨텍스트 메뉴의
        /// 진입점이 쓴다 (장비 단위 전체 반출은 Unload).</summary>
        internal static ActionResult UnloadPort(string group, string eqpId, int outPort)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Equipment not found.");
            }

            if (equipment.CommMode == "OffLine")
            {
                return Fail("Equipment " + eqpId + " is offline — switch to online first.");
            }

            if (outPort < 1 || outPort > equipment.OutPorts.Length)
            {
                return Fail("Out-port " + outPort + " not found on " + eqpId + ".");
            }

            Port port = equipment.OutPorts[outPort - 1];

            if (port.Stat != "Done")
            {
                return Fail("No finished lot to unload on " + eqpId + " out-port " + outPort + ".");
            }

            // 반출 — Lot이 캐리어째 시스템 밖으로 나간다.
            port.LotId = string.Empty;
            port.Stat = string.Empty;
            port.CarrierId = string.Empty;
            return Succeed();
        }

        /// <summary>반출 — 완료(Done) 상태의 아웃포트를 전부 비운다.</summary>
        internal static ActionResult Unload(string group, string eqpId)
        {
            Equipment equipment = FindEquipment(group, eqpId);

            if (equipment == null)
            {
                return Fail("Equipment not found.");
            }

            if (equipment.CommMode == "OffLine")
            {
                return Fail("Equipment " + eqpId + " is offline — switch to online first.");
            }

            int unloaded = 0;

            foreach (Port port in equipment.OutPorts)
            {
                if (port.Stat == "Done")
                {
                    // 반출 — Lot이 캐리어째 시스템 밖으로 나간다.
                    port.LotId = string.Empty;
                    port.Stat = string.Empty;
                    port.CarrierId = string.Empty;
                    unloaded = unloaded + 1;
                }
            }

            if (unloaded == 0)
            {
                return Fail("No finished lot to unload on " + eqpId + ".");
            }

            return Succeed();
        }

        // ===== 내부 헬퍼 =====

        private static ActionResult Succeed()
        {
            ActionResult result = new ActionResult();
            result.Success = true;
            result.Message = string.Empty;
            return result;
        }

        private static ActionResult Fail(string message)
        {
            ActionResult result = new ActionResult();
            result.Success = false;
            result.Message = message;
            return result;
        }

        // 종료 전문(변경분)에서 지정 웨이퍼의 판정 값을 찾는다 ("" = 없음).
        private static string FindJudge(DataTable judgeResults, string waferId)
        {
            if (judgeResults == null)
            {
                return string.Empty;
            }

            foreach (DataRow row in judgeResults.Rows)
            {
                if ((row["WF_ID"] ?? string.Empty).ToString().Trim() == waferId)
                {
                    return (row["JUDGE_RSLT"] ?? string.Empty).ToString().Trim();
                }
            }

            return string.Empty;
        }

        // 수동 작업시작/종료의 통신 모드 검증 — 허용이면 null, 거부면 실패
        // 응답을 돌려준다. OnLineRemote는 장비가 자동 통신으로 진행하므로
        // 수동 개입은 Local로 내린 뒤에만 가능하다.
        private static ActionResult CheckManualJobAllowed(Equipment equipment, string verb)
        {
            if (equipment.CommMode == "OffLine")
            {
                return Fail("Equipment " + equipment.Id + " is offline — switch to online first.");
            }

            if (equipment.CommMode == "OnLineRemote")
            {
                return Fail("Equipment " + equipment.Id
                        + " is online-remote — jobs " + verb + " automatically. Switch to local for manual control.");
            }

            return null;
        }

        private static Equipment FindEquipment(string group, string eqpId)
        {
            foreach (Equipment equipment in GetGroup(group).Equipments)
            {
                if (equipment.Id == eqpId)
                {
                    return equipment;
                }
            }

            return null;
        }

        // 지정 상태의 첫 포트 ("" = 빈 포트).
        private static Port FindPort(Port[] ports, string stat)
        {
            foreach (Port port in ports)
            {
                if (port.Stat == stat)
                {
                    return port;
                }
            }

            return null;
        }

        // 지정 아웃포트 번호(1-기준)를 예약 중인 인포트 — 투입됨/작업중 작업이
        // 그 포트로 나가기로 되어 있으면 예약 상태다.
        private static Port FindReservingPort(Equipment equipment, int outIndex)
        {
            foreach (Port port in equipment.InPorts)
            {
                if (port.OutIndex == outIndex
                        && (port.Stat == "Loaded" || port.Stat == "Running"))
                {
                    return port;
                }
            }

            return null;
        }

        private static void FillPorts(DataRow row, string prefix, Port[] ports)
        {
            for (int index = 0; index < ports.Length; index++)
            {
                string column = prefix + (index + 1);
                row[column + "_LOT"] = ports[index].LotId;
                row[column + "_STAT"] = ports[index].Stat;
                row[column + "_CAR"] = ports[index].CarrierId;
                row[column + "_TM"] = ports[index].Stat.Length > 0
                        ? FormatTime(ports[index].Time)
                        : string.Empty;
            }
        }

        // ===== 데모 시드 =====

        private static GroupState GetGroup(string group)
        {
            GroupState state;

            if (!groups.TryGetValue(group ?? string.Empty, out state))
            {
                state = SeedGroup(group ?? string.Empty);
                groups[group ?? string.Empty] = state;
            }

            return state;
        }

        // 그룹별 고정 시드 — 포트 구성(2/2, 2/1, 1/1)과 진행 단계(빈 장비/투입됨/
        // 작업중/완료 대기/Down)를 골고루 섞어 화면의 모든 상태가 한 번에 보이게 한다.
        private static GroupState SeedGroup(string group)
        {
            GroupState state = new GroupState();
            DateTime now = DateTime.Now;

            switch (group)
            {
                case "GRP-A":
                    // Running 작업은 Out 2 지정(아웃 캐리어 배정 포함) — Out 1은
                    // 이미 완료 Lot이 대기 중. Remote — 작업이 자동 통신으로 진행.
                    state.Equipments.Add(MakeEquipment("EQP-A01", 2, 2, false, "OnLineRemote"));
                    SetPort(state, "EQP-A01", true, 0, "LOT-A310", "Running", now.AddMinutes(-45), 2, "CAR-A907");
                    SetPort(state, "EQP-A01", false, 0, "LOT-A305", "Done", now.AddMinutes(-10));

                    state.Equipments.Add(MakeEquipment("EQP-A02", 2, 1, false));
                    SetPort(state, "EQP-A02", true, 0, "LOT-A311", "Loaded", now.AddMinutes(-5), 1, "CAR-A908");

                    state.Equipments.Add(MakeEquipment("EQP-A03", 1, 1, false));

                    // 작업은 장비당 한 번에 하나 — 두 번째 인포트는 다음 Lot이
                    // 투입된 채 대기(Loaded)하고 Out 2를 예약해 둔 구성.
                    state.Equipments.Add(MakeEquipment("EQP-A04", 2, 2, false));

                    // 수량 4 — 작업종료 다이얼로그에서 빈 슬롯 2개가 보이는 구성.
                    SetPort(state, "EQP-A04", true, 0, "LOT-A307", "Running", now.AddHours(-2), 1, "CAR-A909", 4);
                    SetPort(state, "EQP-A04", true, 1, "LOT-A308", "Loaded", now.AddHours(-1), 2, "CAR-A910");

                    // Down + OffLine — 통신까지 끊긴 장비 (모드 전환 외 처리 불가).
                    state.Equipments.Add(MakeEquipment("EQP-A05", 1, 1, true, "OffLine"));

                    // 지정 아웃포트(Out 1)에 이미 완료 Lot이 있는 구성 —
                    // 반출(Unload) 전에는 작업종료가 막히는 시나리오.
                    state.Equipments.Add(MakeEquipment("EQP-A06", 2, 1, false));
                    SetPort(state, "EQP-A06", true, 0, "LOT-A309", "Running", now.AddMinutes(-30), 1, "CAR-A911");
                    SetPort(state, "EQP-A06", false, 0, "LOT-A300", "Done", now.AddMinutes(-20));

                    AddLots(state, "LOT-A4", 8, now);
                    AddEmptyCarriers(state, "CAR-A9", 6);
                    break;

                case "GRP-B":
                    state.Equipments.Add(MakeEquipment("EQP-B01", 1, 1, false, "OnLineRemote"));
                    SetPort(state, "EQP-B01", true, 0, "LOT-B210", "Running", now.AddMinutes(-70), 1, "CAR-B904");

                    state.Equipments.Add(MakeEquipment("EQP-B02", 2, 2, false));

                    state.Equipments.Add(MakeEquipment("EQP-B03", 1, 1, true, "OffLine"));

                    state.Equipments.Add(MakeEquipment("EQP-B04", 2, 1, false));
                    SetPort(state, "EQP-B04", true, 0, "LOT-B211", "Loaded", now.AddMinutes(-15), 1, "CAR-B905");

                    AddLots(state, "LOT-B3", 5, now);
                    AddEmptyCarriers(state, "CAR-B9", 3);
                    break;

                default:
                    state.Equipments.Add(MakeEquipment("EQP-C01", 2, 2, false));

                    state.Equipments.Add(MakeEquipment("EQP-C02", 1, 1, false, "OnLineRemote"));
                    SetPort(state, "EQP-C02", true, 0, "LOT-C110", "Running", now.AddMinutes(-25), 1, "CAR-C903");
                    SetPort(state, "EQP-C02", false, 0, "LOT-C105", "Done", now.AddMinutes(-90));

                    state.Equipments.Add(MakeEquipment("EQP-C03", 1, 1, false));

                    AddLots(state, "LOT-C2", 3, now);
                    AddEmptyCarriers(state, "CAR-C9", 2);
                    break;
            }

            return state;
        }

        private static Equipment MakeEquipment(
                string id, int inCount, int outCount, bool down, string commMode = "OnLineLocal")
        {
            Equipment equipment = new Equipment();
            equipment.Id = id;
            equipment.Down = down;
            equipment.CommMode = commMode;
            equipment.InPorts = MakePorts(inCount);
            equipment.OutPorts = MakePorts(outCount);
            return equipment;
        }

        private static Port[] MakePorts(int count)
        {
            Port[] ports = new Port[count];

            for (int index = 0; index < count; index++)
            {
                ports[index] = new Port();
            }

            return ports;
        }

        // 포트 시드 — 캐리어는 Lot ID에서 유도한다 ("LOT-A310" → "CAR-A310").
        // 인포트에 아웃포트를 지정하면 outCarrier(아웃 캐리어)도 함께 배정한다.
        // qty는 Lot 수량(= 슬롯 맵의 웨이퍼 장착 수) — 6 미만이면 작업종료
        // 다이얼로그에서 빈 슬롯(판정 대상 아님)이 보인다.
        private static void SetPort(
                GroupState state, string eqpId, bool inPort, int index,
                string lotId, string stat, DateTime time, int outIndex = 0, string outCarrier = "",
                int qty = 10)
        {
            foreach (Equipment equipment in state.Equipments)
            {
                if (equipment.Id != eqpId)
                {
                    continue;
                }

                Port port = inPort ? equipment.InPorts[index] : equipment.OutPorts[index];
                port.LotId = lotId;
                port.Stat = stat;
                port.Time = time;
                port.Qty = qty;
                port.CarrierId = lotId.Length > 3 ? "CAR" + lotId.Substring(3) : string.Empty;
                port.OutIndex = outIndex;
                port.OutCarrierId = outCarrier;
                return;
            }
        }

        // 대기 Lot 시드 — 우선순위 1..count, 수량/도착시각은 순번 기반 고정값.
        // 캐리어는 Lot ID에서 유도한다 ("LOT-A4101" → "CAR-A4101").
        private static void AddLots(GroupState state, string idPrefix, int count, DateTime now)
        {
            for (int index = 1; index <= count; index++)
            {
                WaitingLot lot = new WaitingLot();
                lot.LotId = idPrefix + (100 + index).ToString(CultureInfo.InvariantCulture);
                lot.Priority = index;
                lot.Quantity = ((index * 7) % 20) + 5;
                lot.ArriveTime = now.AddHours(-(index * 3));
                lot.CarrierId = "CAR" + lot.LotId.Substring(3);
                state.Lots.Add(lot);
            }
        }

        // 빈 캐리어 풀 시드 — "CAR-A901" 형태로 count개.
        private static void AddEmptyCarriers(GroupState state, string idPrefix, int count)
        {
            for (int index = 1; index <= count; index++)
            {
                state.EmptyCarriers.Add(idPrefix + (index).ToString("00", CultureInfo.InvariantCulture));
            }
        }

        // 인터페이스 시각 표기 공통 형식.
        private static string FormatTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
