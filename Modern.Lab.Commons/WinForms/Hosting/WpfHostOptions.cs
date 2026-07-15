namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// ElementHost 래퍼 공통 옵션. <see cref="Modern.Lab.Theming.ModernTheme.Mode"/>처럼
    /// 앱 시작 시(첫 컨트롤 생성 전) 한 번 설정한다.
    /// </summary>
    public static class WpfHostOptions
    {
        /// <summary>
        /// true면 모든 래퍼가 생성 시점에 ElementHost의 기본 Cursor 속성 매핑을
        /// 제거한다 — WinForms 쪽 커서 상태(<c>Cursor</c>/<c>UseWaitCursor</c>)가
        /// WPF 콘텐츠로 복사되지 않고, WPF가 항상 자기 커서를 관리한다.
        ///
        /// 용도: 호스트 폼(수정 불가한 공용 base form 등)이 조회 중 WaitCursor를
        /// 걸었다가 복원이 매핑에 반영되지 않는 경로(<c>Cursor.Current</c> 복원,
        /// 비 UI 스레드 복원 등)로 인해 WPF 컨트롤 위에만 Wait 커서가 영구히
        /// 남는 문제를 원천 차단한다.
        ///
        /// 트레이드오프: 폼이 의도적으로 건 Wait 커서도 WPF 콘텐츠 위에서는
        /// 보이지 않게 된다 (네이티브 컨트롤은 영향 없음). 기본값 false —
        /// 켜지 않으면 기존과 완전히 동일하게 동작한다.
        /// </summary>
        public static bool DisableCursorPropertyMap { get; set; }
    }
}
