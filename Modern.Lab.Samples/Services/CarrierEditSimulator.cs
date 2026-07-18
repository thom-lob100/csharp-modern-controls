using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// ★ 회사 환경 교체 지점 — Carrier Editor 화면의 데모 데이터 + 서버 역할.
    ///
    /// 캐리어 두 종류의 수납 구조를 다룬다:
    /// - FOUP : 웨이퍼 슬롯 25개 (1~25).
    /// - TRAY : 칩이 들어가는 STUB 6개 + LCC 25개. LCC 1개에는 FINGER
    ///   A/B/C/D/E 다섯 자리가 있고, 꽂힌 핑거는 Top/Left/Right 셋 중 한
    ///   위치에 삽입된다 (삽입 위치는 유닛의 속성 — 이동해도 유지된다).
    ///
    /// 처리(Split 이동/Merge 전체 이동/Scrap 폐기)는 전부 서버가 검증 +
    /// 반영하고, 화면은 성공 후 **재조회**로 결과를 받아 보여준다 — 다른
    /// 화면들과 같은 패턴이다. 이동은 전부-아니면-전무: 대상의 같은 종류
    /// 빈 자리가 부족하면 아무것도 옮기지 않고 사유를 돌려준다.
    ///
    /// 홈 환경에는 캐리어 시스템이 없으므로 고정 시드의 인메모리 상태로
    /// 서버처럼 동작한다. 회사 적용 시 이 클래스를 통째로 지우고, 폼의
    /// 조회/처리 메서드가 회사 인터페이스를 직접 호출하게 바꾼다.
    /// </summary>
    internal static class CarrierEditSimulator
    {
        /// <summary>처리 전문의 응답 — 성공 여부와 실패 사유.</summary>
        internal sealed class ActionResult
        {
            /// <summary>처리 성공 여부.</summary>
            internal bool Success;

            /// <summary>실패 사유 (성공이면 빈 문자열) — 화면 표기용 영어 문구.</summary>
            internal string Message;

            /// <summary>처리(이동/폐기)된 유닛 수.</summary>
            internal int Count;
        }

        // ===== 수납 구조 상수 =====

        internal const int FoupSlotCount = 25;
        internal const int TrayStubCount = 6;
        internal const int TrayLccCount = 25;

        /// <summary>LCC 한 개의 핑거 자리 이름 (A~E 다섯 자리).</summary>
        internal static readonly string[] LccFingers = new string[] { "A", "B", "C", "D", "E" };

        // 핑거 삽입 위치 — 시드에서 순환 배정한다.
        private static readonly string[] insertPositions = new string[] { "Top", "Left", "Right" };

        // ===== 인메모리 서버 상태 =====

        private sealed class Foup
        {
            // 슬롯별 웨이퍼 ID (빈 슬롯은 null/빈 문자열).
            internal readonly string[] Slots = new string[FoupSlotCount];

            // 이 FOUP의 랏(아이템). FOUP은 랏 하나만 담되, 안의 웨이퍼는 물리
            // ID를 유지하고 랏은 이 캐리어 값을 따른다 — Merge로 옮겨온 웨이퍼는
            // 대상 FOUP의 랏이 된다(랏이 달라도 공간만 있으면 이동 가능).
            internal string Lot = string.Empty;
        }

        private sealed class Tray
        {
            // STUB별 칩 ID.
            internal readonly string[] Stubs = new string[TrayStubCount];

            // LCC [위치, 핑거]별 칩 ID와 삽입 위치(Top/Left/Right).
            internal readonly string[,] LccChips = new string[TrayLccCount, 5];
            internal readonly string[,] LccInserts = new string[TrayLccCount, 5];
        }

        private static Dictionary<string, Foup> foups;
        private static Dictionary<string, Tray> trays;

        // ===== 조회 (★ 회사 캐리어 조회로 교체) =====

        /// <summary>타입별 캐리어 목록 — ID와 채움 수/용량. 콤보 원천이다.</summary>
        internal static DataTable GetCarriers(string type)
        {
            EnsureSeed();

            DataTable table = new DataTable();
            table.Columns.Add("CARR_ID", typeof(string));
            table.Columns.Add("FILL_CNT", typeof(int));
            table.Columns.Add("CAPACITY", typeof(int));

            if (type == "FOUP")
            {
                foreach (KeyValuePair<string, Foup> entry in foups)
                {
                    table.Rows.Add(entry.Key, CountFoup(entry.Value), FoupSlotCount);
                }
            }
            else
            {
                foreach (KeyValuePair<string, Tray> entry in trays)
                {
                    table.Rows.Add(
                            entry.Key, CountTray(entry.Value),
                            TrayStubCount + TrayLccCount * LccFingers.Length);
                }
            }

            return table;
        }

        /// <summary>캐리어의 수납 현황 — 자리당 한 행(빈 자리 포함)이라 채움
        /// 구조가 그대로 보인다. 컬럼 계약:
        ///   KIND   : "SLOT"(FOUP) / "STUB" / "LCC"
        ///   POS    : 자리 번호 (SLOT 1~25 / STUB 1~6 / LCC 1~25)
        ///   FINGER : LCC 전용 핑거 자리 (A~E, 그 외 "")
        ///   INS_POS: LCC 전용 삽입 위치 (Top/Left/Right, 빈 자리는 "")
        ///   UNIT_ID: 수납된 웨이퍼/칩 ID (빈 자리는 "")
        ///   ITEM_ID: 유닛이 속한 아이템 ID (빈 자리는 "") — 한 캐리어에
        ///            여러 아이템의 유닛이 섞일 수 있다.</summary>
        internal static DataTable GetCarrierUnits(string type, string carrierId)
        {
            EnsureSeed();

            DataTable table = new DataTable();
            table.Columns.Add("KIND", typeof(string));
            table.Columns.Add("POS", typeof(int));
            table.Columns.Add("FINGER", typeof(string));
            table.Columns.Add("INS_POS", typeof(string));
            table.Columns.Add("UNIT_ID", typeof(string));
            table.Columns.Add("ITEM_ID", typeof(string));

            if (type == "FOUP")
            {
                Foup foup = FindFoup(carrierId);

                for (int slot = 0; foup != null && slot < FoupSlotCount; slot++)
                {
                    string unit = foup.Slots[slot] ?? string.Empty;
                    // FOUP의 랏은 캐리어 값(foup.Lot) — 안의 웨이퍼는 ID가 달라도
                    // 모두 이 FOUP의 랏으로 표시된다(Merge로 편입된 웨이퍼 포함).
                    table.Rows.Add("SLOT", slot + 1, string.Empty, string.Empty,
                            unit, unit.Length > 0 ? foup.Lot : string.Empty);
                }

                return table;
            }

            Tray tray = FindTray(carrierId);

            if (tray == null)
            {
                return table;
            }

            for (int stub = 0; stub < TrayStubCount; stub++)
            {
                string unit = tray.Stubs[stub] ?? string.Empty;
                table.Rows.Add("STUB", stub + 1, string.Empty, string.Empty,
                        unit, DeriveItemId(unit));
            }

            for (int pos = 0; pos < TrayLccCount; pos++)
            {
                for (int finger = 0; finger < LccFingers.Length; finger++)
                {
                    string unit = tray.LccChips[pos, finger] ?? string.Empty;
                    table.Rows.Add("LCC", pos + 1, LccFingers[finger],
                            tray.LccInserts[pos, finger] ?? string.Empty,
                            unit, DeriveItemId(unit));
                }
            }

            return table;
        }

        // 유닛 ID에서 소속 아이템 ID를 유도한다 — 데모 유닛 ID는
        // "WF-W01.03"/"CH-C01-S1"처럼 아이템 토큰(W01/C01)을 품고 있다.
        // ★ 회사 환경에서는 조회가 ITEM_ID 컬럼을 직접 내려준다.
        private static string DeriveItemId(string unitId)
        {
            if (string.IsNullOrEmpty(unitId))
            {
                return string.Empty;
            }

            int start = unitId.IndexOf('-');

            if (start < 0)
            {
                return string.Empty;
            }

            int end = start + 1;

            while (end < unitId.Length && unitId[end] != '.' && unitId[end] != '-')
            {
                end = end + 1;
            }

            return "IT-" + unitId.Substring(start + 1, end - start - 1);
        }

        // ===== 처리 (★ 회사 인터페이스 호출로 교체) =====

        /// <summary>이동(Split/Merge/드롭 공용) — 지정 유닛들을 대상 캐리어의
        /// 같은 종류 빈 자리에 채워 옮긴다 (LCC 칩은 삽입 위치 유지).
        /// 채우는 순서는 기본 앞에서부터이고, 앵커(anchorKind/anchorPos —
        /// 드래그앤드롭으로 놓은 자리)를 주면 **그 자리부터** 채우고 모자라면
        /// 앞쪽 빈 자리로 이어진다(랩어라운드) — 자리 번호와 유닛은 묶여
        /// 있지 않으므로 아무 빈 자리나 쓸 수 있다. 전부-아니면-전무: 빈
        /// 자리가 부족하면 아무것도 옮기지 않는다.
        /// units는 GetCarrierUnits 스키마의 행들(KIND/POS/FINGER가 키).</summary>
        internal static ActionResult MoveUnits(
                string type, string fromId, string toId, DataTable units,
                string anchorKind = "", int anchorPos = 0)
        {
            EnsureSeed();

            if (units == null || units.Rows.Count == 0)
            {
                return Fail("No unit selected to move.");
            }

            if (fromId == toId)
            {
                return Fail("Source and target carrier are the same.");
            }

            if (type == "FOUP")
            {
                return MoveFoupUnits(fromId, toId, units, anchorKind == "SLOT" ? anchorPos : 0);
            }

            return MoveTrayUnits(fromId, toId, units, anchorKind, anchorPos);
        }

        /// <summary>폐기 — 지정 유닛들을 캐리어에서 제거한다 (시스템 밖으로).
        /// 빈 자리를 지정하면 실패한다.</summary>
        internal static ActionResult ScrapUnits(string type, string carrierId, DataTable units)
        {
            EnsureSeed();

            if (units == null || units.Rows.Count == 0)
            {
                return Fail("No unit selected to scrap.");
            }

            if (type == "FOUP")
            {
                Foup foup = FindFoup(carrierId);

                if (foup == null)
                {
                    return Fail("Carrier " + carrierId + " not found.");
                }

                // 검증 먼저 (전부-아니면-전무), 통과하면 제거.
                foreach (DataRow row in units.Rows)
                {
                    int slot = ToInt(row["POS"]);

                    if (slot < 1 || slot > FoupSlotCount
                            || string.IsNullOrEmpty(foup.Slots[slot - 1]))
                    {
                        return Fail("Slot " + slot + " has no wafer to scrap.");
                    }
                }

                foreach (DataRow row in units.Rows)
                {
                    foup.Slots[ToInt(row["POS"]) - 1] = null;
                }

                return Succeed(units.Rows.Count);
            }

            Tray tray = FindTray(carrierId);

            if (tray == null)
            {
                return Fail("Carrier " + carrierId + " not found.");
            }

            foreach (DataRow row in units.Rows)
            {
                if (ReadTrayUnit(tray, row) == null)
                {
                    return Fail(DescribeTrayPosition(row) + " has no chip to scrap.");
                }
            }

            foreach (DataRow row in units.Rows)
            {
                ClearTrayUnit(tray, row);
            }

            return Succeed(units.Rows.Count);
        }

        // ===== 이동 내부 구현 =====

        private static ActionResult MoveFoupUnits(
                string fromId, string toId, DataTable units, int anchorPos)
        {
            Foup source = FindFoup(fromId);
            Foup target = FindFoup(toId);

            if (source == null || target == null)
            {
                return Fail("Carrier not found.");
            }

            // 원본 검증(전부-아니면-전무). 랏(아이템) 일치는 확인하지 않는다 —
            // 옮겨온 웨이퍼는 대상 FOUP의 랏이 되므로 공간만 있으면 이동한다.
            List<int> moveSlots = new List<int>();

            foreach (DataRow row in units.Rows)
            {
                int slot = ToInt(row["POS"]);

                if (slot < 1 || slot > FoupSlotCount
                        || string.IsNullOrEmpty(source.Slots[slot - 1]))
                {
                    return Fail("Slot " + slot + " has no wafer to move.");
                }

                moveSlots.Add(slot - 1);
            }

            List<int> empties = new List<int>();

            for (int slot = 0; slot < FoupSlotCount; slot++)
            {
                if (string.IsNullOrEmpty(target.Slots[slot]))
                {
                    empties.Add(slot);
                }
            }

            // 앵커(놓은 슬롯)가 있으면 그 슬롯부터 채운다 (모자라면 앞쪽으로 랩).
            empties = RotateFrom(empties, delegate(int slot) { return slot >= anchorPos - 1; });

            if (empties.Count < moveSlots.Count)
            {
                return Fail("Target " + toId + " has only " + empties.Count
                        + " empty slots — " + moveSlots.Count + " selected.");
            }

            // 대상이 비어 있었으면 옮겨온 랏이 대상의 랏이 되고, 이미 랏이
            // 있으면 옮겨온 웨이퍼가 그 랏으로 편입된다(대상 랏 유지).
            bool targetWasEmpty = CountFoup(target) == 0;

            for (int index = 0; index < moveSlots.Count; index++)
            {
                target.Slots[empties[index]] = source.Slots[moveSlots[index]];
                source.Slots[moveSlots[index]] = null;
            }

            if (targetWasEmpty)
            {
                target.Lot = source.Lot;
            }

            // 원본이 완전히 비면 랏을 지운다(다음에 다른 랏을 받을 수 있게).
            if (CountFoup(source) == 0)
            {
                source.Lot = string.Empty;
            }

            return Succeed(moveSlots.Count);
        }

        private static ActionResult MoveTrayUnits(
                string fromId, string toId, DataTable units, string anchorKind, int anchorPos)
        {
            Tray source = FindTray(fromId);
            Tray target = FindTray(toId);

            if (source == null || target == null)
            {
                return Fail("Carrier not found.");
            }

            // 종류별로 나눠 검증한다 — STUB은 STUB 빈 자리로, LCC 칩은 LCC
            // 빈 핑거 자리로만 옮길 수 있다.
            List<DataRow> stubMoves = new List<DataRow>();
            List<DataRow> lccMoves = new List<DataRow>();

            foreach (DataRow row in units.Rows)
            {
                if (ReadTrayUnit(source, row) == null)
                {
                    return Fail(DescribeTrayPosition(row) + " has no chip to move.");
                }

                if ((row["KIND"] ?? string.Empty).ToString() == "STUB")
                {
                    stubMoves.Add(row);
                }
                else
                {
                    lccMoves.Add(row);
                }
            }

            List<int> stubEmpties = new List<int>();

            for (int stub = 0; stub < TrayStubCount; stub++)
            {
                if (string.IsNullOrEmpty(target.Stubs[stub]))
                {
                    stubEmpties.Add(stub);
                }
            }

            // 앵커(놓은 STUB 자리)가 있으면 그 자리부터 채운다.
            if (anchorKind == "STUB")
            {
                stubEmpties = RotateFrom(stubEmpties, delegate(int stub) { return stub >= anchorPos - 1; });
            }

            if (stubEmpties.Count < stubMoves.Count)
            {
                return Fail("Target " + toId + " has only " + stubEmpties.Count
                        + " empty STUB positions — " + stubMoves.Count + " selected.");
            }

            // LCC는 **LCC 단위**로 옮긴다 — 원본 LCC 하나의 채워진 핑거들을
            // 대상의 LCC 하나에 **같은 핑거 자리(A→A…)** 로 통째로 넣는다.
            // 대상 LCC에 다른 칩이 있어도 필요한 핑거 자리만 비어 있으면 그
            // LCC를 쓰되, 한 대상 LCC에는 원본 LCC 하나만 배정한다(서로 다른
            // 원본 LCC를 한 대상 LCC에 섞지 않는다).
            List<int> sourceLccPositions = new List<int>();
            Dictionary<int, List<DataRow>> sourceLccGroups = new Dictionary<int, List<DataRow>>();

            foreach (DataRow row in lccMoves)
            {
                int pos = ToInt(row["POS"]);
                List<DataRow> group;

                if (!sourceLccGroups.TryGetValue(pos, out group))
                {
                    group = new List<DataRow>();
                    sourceLccGroups[pos] = group;
                    sourceLccPositions.Add(pos);
                }

                group.Add(row);
            }

            sourceLccPositions.Sort();

            // 대상 LCC 후보 순서 (앵커부터, 없으면 1번부터).
            List<int> targetLccOrder = new List<int>();

            for (int pos = 0; pos < TrayLccCount; pos++)
            {
                targetLccOrder.Add(pos);
            }

            if (anchorKind == "LCC")
            {
                targetLccOrder = RotateFrom(targetLccOrder, delegate(int pos) { return pos >= anchorPos - 1; });
            }

            // 각 원본 LCC에 대상 LCC 하나를 배정 (필요 핑거가 모두 빈 LCC).
            Dictionary<int, int> lccAssign = new Dictionary<int, int>();
            HashSet<int> usedTargetLcc = new HashSet<int>();

            foreach (int srcPos in sourceLccPositions)
            {
                int chosen = -1;

                foreach (int tgtPos in targetLccOrder)
                {
                    if (usedTargetLcc.Contains(tgtPos))
                    {
                        continue;
                    }

                    bool fits = true;

                    foreach (DataRow row in sourceLccGroups[srcPos])
                    {
                        if (!string.IsNullOrEmpty(target.LccChips[tgtPos, FingerIndex(row)]))
                        {
                            fits = false;
                            break;
                        }
                    }

                    if (fits)
                    {
                        chosen = tgtPos;
                        break;
                    }
                }

                if (chosen < 0)
                {
                    return Fail("Target " + toId + " has no free LCC for source LCC "
                            + srcPos + " (same finger positions must be empty).");
                }

                lccAssign[srcPos] = chosen;
                usedTargetLcc.Add(chosen);
            }

            // 검증 통과 — STUB 이동.
            for (int index = 0; index < stubMoves.Count; index++)
            {
                int fromStub = ToInt(stubMoves[index]["POS"]) - 1;
                target.Stubs[stubEmpties[index]] = source.Stubs[fromStub];
                source.Stubs[fromStub] = null;
            }

            // LCC 이동 — 배정된 대상 LCC에 같은 핑거 자리로 (삽입 위치 유지).
            int movedLcc = 0;

            foreach (int srcPos in sourceLccPositions)
            {
                int toPos = lccAssign[srcPos];
                int fromIndex = srcPos - 1;

                foreach (DataRow row in sourceLccGroups[srcPos])
                {
                    int finger = FingerIndex(row);
                    target.LccChips[toPos, finger] = source.LccChips[fromIndex, finger];
                    target.LccInserts[toPos, finger] = source.LccInserts[fromIndex, finger];
                    source.LccChips[fromIndex, finger] = null;
                    source.LccInserts[fromIndex, finger] = null;
                    movedLcc = movedLcc + 1;
                }
            }

            return Succeed(stubMoves.Count + movedLcc);
        }

        // ===== 내부 헬퍼 =====

        // 조건을 처음 만족하는 지점부터 시작하도록 목록을 회전한다 — 앵커
        // 이후 자리를 먼저 쓰고 모자라면 앞쪽 자리로 이어지는(랩어라운드)
        // 순서를 만든다. 조건을 만족하는 항목이 없으면 원래 순서 그대로다.
        private static List<T> RotateFrom<T>(List<T> items, Predicate<T> startsHere)
        {
            int start = items.FindIndex(startsHere);

            if (start <= 0)
            {
                return items;
            }

            List<T> rotated = new List<T>(items.Count);

            for (int index = start; index < items.Count; index++)
            {
                rotated.Add(items[index]);
            }

            for (int index = 0; index < start; index++)
            {
                rotated.Add(items[index]);
            }

            return rotated;
        }

        // 유닛 행이 가리키는 트레이 자리의 칩 ID (없으면 null).
        private static string ReadTrayUnit(Tray tray, DataRow row)
        {
            string kind = (row["KIND"] ?? string.Empty).ToString();
            int pos = ToInt(row["POS"]);

            if (kind == "STUB")
            {
                return pos >= 1 && pos <= TrayStubCount ? tray.Stubs[pos - 1] : null;
            }

            int finger = FingerIndex(row);

            if (pos < 1 || pos > TrayLccCount || finger < 0)
            {
                return null;
            }

            string chip = tray.LccChips[pos - 1, finger];
            return string.IsNullOrEmpty(chip) ? null : chip;
        }

        private static void ClearTrayUnit(Tray tray, DataRow row)
        {
            string kind = (row["KIND"] ?? string.Empty).ToString();
            int pos = ToInt(row["POS"]);

            if (kind == "STUB")
            {
                tray.Stubs[pos - 1] = null;
                return;
            }

            int finger = FingerIndex(row);
            tray.LccChips[pos - 1, finger] = null;
            tray.LccInserts[pos - 1, finger] = null;
        }

        // 실패 사유 표기용 자리 설명 ("STUB 3" / "LCC 7-C").
        private static string DescribeTrayPosition(DataRow row)
        {
            string kind = (row["KIND"] ?? string.Empty).ToString();
            int pos = ToInt(row["POS"]);

            return kind == "STUB"
                    ? "STUB " + pos
                    : "LCC " + pos + "-" + (row["FINGER"] ?? string.Empty);
        }

        private static int FingerIndex(DataRow row)
        {
            return Array.IndexOf(LccFingers, (row["FINGER"] ?? string.Empty).ToString());
        }

        private static int ToInt(object value)
        {
            int parsed;
            return int.TryParse((value ?? string.Empty).ToString(), out parsed) ? parsed : 0;
        }

        private static Foup FindFoup(string carrierId)
        {
            Foup foup;
            return foups.TryGetValue(carrierId ?? string.Empty, out foup) ? foup : null;
        }

        private static Tray FindTray(string carrierId)
        {
            Tray tray;
            return trays.TryGetValue(carrierId ?? string.Empty, out tray) ? tray : null;
        }

        private static int CountFoup(Foup foup)
        {
            int count = 0;

            foreach (string slot in foup.Slots)
            {
                if (!string.IsNullOrEmpty(slot))
                {
                    count = count + 1;
                }
            }

            return count;
        }

        private static int CountTray(Tray tray)
        {
            int count = 0;

            foreach (string stub in tray.Stubs)
            {
                if (!string.IsNullOrEmpty(stub))
                {
                    count = count + 1;
                }
            }

            for (int pos = 0; pos < TrayLccCount; pos++)
            {
                for (int finger = 0; finger < LccFingers.Length; finger++)
                {
                    if (!string.IsNullOrEmpty(tray.LccChips[pos, finger]))
                    {
                        count = count + 1;
                    }
                }
            }

            return count;
        }

        private static ActionResult Succeed(int count)
        {
            ActionResult result = new ActionResult();
            result.Success = true;
            result.Message = string.Empty;
            result.Count = count;
            return result;
        }

        private static ActionResult Fail(string message)
        {
            ActionResult result = new ActionResult();
            result.Success = false;
            result.Message = message;
            return result;
        }

        // ===== 데모 시드 =====

        // 채움/부분/빈 캐리어를 골고루 — Split(부분 이동), Merge(랏 달라도
        // 공간만 있으면 이동), 공간 부족 실패까지 한 화면에서 재현할 수 있는
        // 구성. FOUP은 캐리어 랏(Lot)으로 색이 정해지고(옮겨온 웨이퍼는 대상
        // 랏이 됨), TRAY는 칩 ID의 아이템 토큰으로 색을 구분한다(한 트레이에
        // 여러 아이템이 섞인 구성 TRAY-01 포함).
        private static void EnsureSeed()
        {
            if (foups != null)
            {
                return;
            }

            foups = new Dictionary<string, Foup>(StringComparer.Ordinal);
            trays = new Dictionary<string, Tray>(StringComparer.Ordinal);

            // FOUP은 랏 하나만 담되, 옮겨온 웨이퍼는 대상 FOUP의 랏이 된다
            // (랏이 달라도 공간만 있으면 Merge 가능). FOUP-01: IT-W01 부분(12장)
            // / FOUP-02: IT-W02 가득 / 03·04: 빈 것.
            Foup foup1 = new Foup();

            for (int slot = 0; slot < 10; slot++)
            {
                foup1.Slots[slot] = MakeWaferId("W01", slot + 1);
            }

            foup1.Slots[13] = MakeWaferId("W01", 11);
            foup1.Slots[14] = MakeWaferId("W01", 12);
            foup1.Lot = "IT-W01";
            foups["FOUP-01"] = foup1;

            Foup foup2 = new Foup();

            for (int slot = 0; slot < FoupSlotCount; slot++)
            {
                foup2.Slots[slot] = MakeWaferId("W02", slot + 1);
            }

            foup2.Lot = "IT-W02";
            foups["FOUP-02"] = foup2;
            foups["FOUP-03"] = new Foup();
            foups["FOUP-04"] = new Foup();

            // FOUP-05: IT-W03 부분(6장) — **차 있는 대상** 예제. 랏이 달라도
            // 공간만 있으면 Merge되고, 옮겨온 웨이퍼는 IT-W03이 된다.
            Foup foup5 = new Foup();

            for (int slot = 0; slot < 6; slot++)
            {
                foup5.Slots[slot] = MakeWaferId("W03", 21 + slot);
            }

            foup5.Lot = "IT-W03";
            foups["FOUP-05"] = foup5;

            // TRAY-01: IT-C01(STUB 4개 + LCC 1~8) + IT-C02(LCC 9 A/B) 혼재.
            Tray tray1 = new Tray();

            for (int stub = 0; stub < 4; stub++)
            {
                tray1.Stubs[stub] = "CH-C01-S" + (stub + 1);
            }

            int insertCycle = 0;

            for (int pos = 0; pos < 8; pos++)
            {
                for (int finger = 0; finger < LccFingers.Length; finger++)
                {
                    tray1.LccChips[pos, finger] = MakeChipId("C01", pos + 1, finger);
                    tray1.LccInserts[pos, finger] = insertPositions[insertCycle % insertPositions.Length];
                    insertCycle = insertCycle + 1;
                }
            }

            tray1.LccChips[8, 0] = MakeChipId("C02", 9, 0);
            tray1.LccInserts[8, 0] = "Top";
            tray1.LccChips[8, 1] = MakeChipId("C02", 9, 1);
            tray1.LccInserts[8, 1] = "Left";
            trays["TRAY-01"] = tray1;

            // TRAY-03: IT-C03 소량(STUB 2개 + LCC 1 A~C) — Merge 대상으로 적당.
            Tray tray3 = new Tray();
            tray3.Stubs[0] = "CH-C03-S1";
            tray3.Stubs[1] = "CH-C03-S2";

            for (int finger = 0; finger < 3; finger++)
            {
                tray3.LccChips[0, finger] = MakeChipId("C03", 1, finger);
                tray3.LccInserts[0, finger] = insertPositions[finger];
            }

            trays["TRAY-02"] = new Tray();
            trays["TRAY-03"] = tray3;
            trays["TRAY-04"] = new Tray();

            // TRAY-05: IT-C05 부분(STUB 3 + LCC 1~2) — **차 있는 대상** 예제.
            Tray tray5 = new Tray();

            for (int stub = 0; stub < 3; stub++)
            {
                tray5.Stubs[stub] = "CH-C05-S" + (stub + 1);
            }

            for (int pos = 0; pos < 2; pos++)
            {
                for (int finger = 0; finger < LccFingers.Length; finger++)
                {
                    tray5.LccChips[pos, finger] = MakeChipId("C05", pos + 1, finger);
                    tray5.LccInserts[pos, finger] = insertPositions[finger % insertPositions.Length];
                }
            }

            trays["TRAY-05"] = tray5;
        }

        // 웨이퍼 ID — "WF-{아이템 토큰}.{순번}" (예: WF-W01.03).
        private static string MakeWaferId(string itemToken, int sequence)
        {
            return "WF-" + itemToken + "." + sequence.ToString("00", CultureInfo.InvariantCulture);
        }

        // 칩 ID — "CH-{아이템 토큰}-{LCC 위치}{핑거}" (예: CH-C01-03A).
        private static string MakeChipId(string itemToken, int pos, int fingerIndex)
        {
            return "CH-" + itemToken
                    + "-" + pos.ToString("00", CultureInfo.InvariantCulture)
                    + LccFingers[fingerIndex];
        }
    }
}
