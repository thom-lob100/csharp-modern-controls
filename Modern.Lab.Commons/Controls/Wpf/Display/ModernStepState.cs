namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 스텝 인디케이터의 각 단계 상태.
    /// 데이터 바인딩 시에는 이 이름의 문자열("Completed" 등, 대소문자 무시)로 전달한다.
    /// </summary>
    public enum ModernStepState
    {
        /// <summary>아직 도달하지 않은 단계(회색).</summary>
        Pending,

        /// <summary>현재 진행 중인 단계(액센트 강조).</summary>
        Current,

        /// <summary>완료된 단계(액센트 채움 + 체크).</summary>
        Completed,

        /// <summary>실패/중단된 단계(빨강 + X). 예: Scrap.</summary>
        Failed
    }
}
