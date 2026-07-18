namespace Modern.Lab.WinForms.Controls.Display
{
    /// <summary>
    /// ModernStatusBadge의 모양 — 상태 강조(알약)와 수치/코드 표시(둥근 사각)
    /// 어느 쪽이 어울리는지에 따라 고른다.
    /// </summary>
    public enum BadgeShape
    {
        /// <summary>좌우가 반원인 알약(캡슐) 모양 — 기본.</summary>
        Pill = 0,

        /// <summary>모서리만 살짝 둥근 사각 모양 (컨트롤 radius 4와 동일한 인상).</summary>
        Rounded = 1
    }
}
