using System;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// ModernSlotMap의 셀 클릭 이벤트 인자 — 클릭된 채움 셀의 키(SlotMapCell.Key)를
    /// 전달한다. 컨트롤은 클릭 시 선택 상태를 스스로 바꾸지 않고 이 이벤트만
    /// 발생시키며, 선택 표시는 화면(폼)이 SetSelectedKeys로 직접 관리한다.
    /// </summary>
    public class SlotMapCellEventArgs : EventArgs
    {
        public SlotMapCellEventArgs(string key)
        {
            this.Key = key ?? string.Empty;
        }

        /// <summary>클릭된 셀의 키.</summary>
        public string Key { get; private set; }
    }
}
