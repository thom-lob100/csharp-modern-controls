namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>레이블의 타이포그래피 역할로, 토큰 타입 램프에 매핑된다.</summary>
    public enum LabelKind
    {
        /// <summary>본문 텍스트. Regular 굵기, 기본 텍스트 색.</summary>
        Body,

        /// <summary>섹션 제목. Title 크기, SemiBold, 기본 텍스트 색.</summary>
        Title,

        /// <summary>필드 레이블. Label 크기, SemiBold, 보조 텍스트 색.</summary>
        Label,

        /// <summary>도움말/캡션 텍스트. Helper 크기, Regular, 보조 텍스트 색.</summary>
        Helper
    }
}
