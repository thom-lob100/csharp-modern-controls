using System.Collections.Generic;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// ModernSlotMap의 자리 한 칸. 두 형태를 지원한다:
    /// - 단일 수납: UnitId 하나 (FOUP 슬롯, TRAY STUB) — SubCells를 비워 둔다.
    /// - 복합 수납: SubCells에 하위 자리들 (TRAY LCC의 핑거 A~E).
    /// 선택/미리보기의 단위는 셀이다 — 복합 셀을 선택하면 그 안의 채워진
    /// 하위 유닛 전부가 대상이 된다.
    /// </summary>
    public class SlotMapCell
    {
        /// <summary>빈 자리(수납 없음)를 만든다.</summary>
        public SlotMapCell()
        {
            this.Key = string.Empty;
            this.Label = string.Empty;
            this.UnitId = string.Empty;
            this.Color = string.Empty;
            this.ToolTip = string.Empty;
            this.SubCells = null;
        }

        /// <summary>선택 식별 키 — 화면(폼)이 부여하고 SelectedKeys로 돌려받는다
        /// (예: "SLOT|7", "LCC|3").</summary>
        public string Key { get; set; }

        /// <summary>자리 번호 표기 (예: "7").</summary>
        public string Label { get; set; }

        /// <summary>단일 수납 자리의 유닛 ID (빈 자리는 빈 문자열).</summary>
        public string UnitId { get; set; }

        /// <summary>채움 색 재정의 (선택, "#DCFCE7" 형식) — 비우면 토큰 기본
        /// 채움색을 쓴다. 유닛의 소속(아이템)별 색 구분에 쓴다.</summary>
        public string Color { get; set; }

        /// <summary>툴팁 재정의 (선택) — 비우면 UnitId가 툴팁이 된다.
        /// "WF-W01.03 — IT-W01"처럼 소속 아이템을 함께 표기할 때 쓴다.</summary>
        public string ToolTip { get; set; }

        /// <summary>복합 수납의 하위 자리 목록 (예: LCC 핑거 A~E). null이면
        /// 단일 수납 자리다.</summary>
        public List<SlotMapSubCell> SubCells { get; set; }

        /// <summary>이 자리에 유닛이 하나라도 있는지.</summary>
        public bool Filled
        {
            get { return this.UnitCount > 0; }
        }

        /// <summary>수납된 유닛 수 (단일 자리는 0/1).</summary>
        public int UnitCount
        {
            get
            {
                if (this.SubCells == null)
                {
                    return string.IsNullOrEmpty(this.UnitId) ? 0 : 1;
                }

                int count = 0;

                foreach (SlotMapSubCell sub in this.SubCells)
                {
                    if (!string.IsNullOrEmpty(sub.UnitId))
                    {
                        count = count + 1;
                    }
                }

                return count;
            }
        }

        /// <summary>이 자리가 더 받을 수 있는 유닛 수 (미리보기 배정용).</summary>
        public int EmptyUnitCount
        {
            get
            {
                if (this.SubCells == null)
                {
                    return string.IsNullOrEmpty(this.UnitId) ? 1 : 0;
                }

                return this.SubCells.Count - this.UnitCount;
            }
        }
    }
}
