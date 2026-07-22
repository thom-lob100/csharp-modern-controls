namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>ModernDropDownButtonControl의 메뉴 항목 하나를 나타내는 UI 항목 모델.</summary>
    public class DropDownButtonItem
    {
        public DropDownButtonItem(object value, string displayText, bool isEnabled)
        {
            this.Value = value;
            this.DisplayText = displayText;
            this.IsEnabled = isEnabled;

            // WinForms ToolStrip 관례 — 표시 텍스트 "-"는 항목이 아니라
            // 구분선이다 (컨텍스트 메뉴의 ToolStripSeparator와 같은 의미).
            this.IsSeparator = displayText == "-";
        }

        /// <summary>항목 값 (ValueMemberPath 기준).</summary>
        public object Value { get; private set; }

        /// <summary>메뉴에 표시되는 텍스트.</summary>
        public string DisplayText { get; private set; }

        /// <summary>항목 실행 가능 여부 (EnabledMemberPath 기준) — 비활성이면
        /// 회색으로 표시되고 클릭되지 않는다.</summary>
        public bool IsEnabled { get; private set; }

        /// <summary>구분선 여부 — 표시 텍스트가 "-"인 행. 클릭되지 않고
        /// 가는 가로선으로 그려진다.</summary>
        public bool IsSeparator { get; private set; }
    }
}
