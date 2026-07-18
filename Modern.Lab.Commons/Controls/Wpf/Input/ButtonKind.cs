namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>버튼의 시각적 종류(색상 / 강조 수준).</summary>
    public enum ButtonKind
    {
        /// <summary>주 액션(예: 조회). 액센트 파랑 배경, 흰 텍스트. 화면당 하나.</summary>
        Primary,

        /// <summary>실행 계열 액션(신규/저장/수정). 흰 배경, 회색 테두리.</summary>
        Secondary,

        /// <summary>파괴적 액션(삭제 등). 아웃라인 빨강 — 흰 배경 위 빨간 텍스트/테두리.</summary>
        Danger,

        /// <summary>낮은 강조 액션(초기화/내보내기 등). 테두리 없는 텍스트 버튼, hover 시 배경 표시.</summary>
        Subtle,

        /// <summary>
        /// 중요 실행 액션(실행). Success 초록 채움, 흰 텍스트 —
        /// Primary와 같은 강조 무게지만 조회 액션과 시각적으로 구분된다.
        /// 화면당 하나, 최대 하나의 Primary와 함께 배치한다.
        /// </summary>
        Execute,

        /// <summary>
        /// Excel 내보내기 전용. Excel을 연상시키는 초록 아웃라인(Surface 배경 +
        /// 초록 텍스트/테두리, hover 시 옅은 초록 채움) — 채움형(Primary/
        /// Execute)보다 가볍게 포인트만 준다. 색은 Brush.Excel* 토큰이라
        /// 테마별로 다르게 정의된다 (다크 테마는 밝은 초록).
        /// </summary>
        Excel
    }
}
