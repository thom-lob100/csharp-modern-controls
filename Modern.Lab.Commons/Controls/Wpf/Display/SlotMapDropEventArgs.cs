using System;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// ModernSlotMap 드롭 이벤트 인자 — 드래그되어 온 셀 키들과, 놓은 위치의
    /// 셀 키(앵커)를 전달한다. 실제 이동 처리(검증 포함)는 화면(폼)이 서버
    /// 호출로 수행한다 — 컨트롤은 의도만 전달한다.
    /// </summary>
    public class SlotMapDropEventArgs : EventArgs
    {
        public SlotMapDropEventArgs(string[] keys, string anchorKey)
        {
            this.Keys = keys ?? new string[0];
            this.AnchorKey = anchorKey ?? string.Empty;
        }

        /// <summary>드래그된 원본 셀 키 목록 (원본 맵의 SlotMapCell.Key).</summary>
        public string[] Keys { get; private set; }

        /// <summary>놓은 자리의 셀 키 — 셀 밖(구획 여백)에 놓으면 빈 문자열
        /// (= 앞에서부터 채움).</summary>
        public string AnchorKey { get; private set; }
    }
}
