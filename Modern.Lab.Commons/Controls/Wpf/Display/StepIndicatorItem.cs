using System.Windows;
using System.Windows.Media;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// ModernStepIndicatorControl이 렌더링하는 단계 하나의 표시 모델.
    /// 상태에 따라 결정되는 색/글리프/연결선을 코드에서 미리 계산해 담아두고,
    /// DataTemplate은 이 속성들을 그대로 바인딩한다(트리거 없이 단순 바인딩).
    /// </summary>
    public class StepIndicatorItem
    {
        /// <summary>노드 아래 표시되는 단계 이름.</summary>
        public string Label { get; set; }

        /// <summary>노드 안 글리프(체크/X 또는 단계 표시 문자).</summary>
        public string Glyph { get; set; }

        /// <summary>글리프 폰트(체크/X는 Segoe MDL2 Assets, 그 외는 본문 폰트).</summary>
        public FontFamily GlyphFontFamily { get; set; }

        /// <summary>노드 원 배경.</summary>
        public Brush NodeBackground { get; set; }

        /// <summary>노드 원 테두리.</summary>
        public Brush NodeBorderBrush { get; set; }

        /// <summary>노드 글리프 색.</summary>
        public Brush NodeForeground { get; set; }

        /// <summary>레이블 글자색.</summary>
        public Brush LabelForeground { get; set; }

        /// <summary>레이블 굵기(현재 단계만 SemiBold, 그 외 Regular).</summary>
        public FontWeight LabelWeight { get; set; }

        /// <summary>왼쪽 연결선 색(진행이 도달했으면 액센트).</summary>
        public Brush LeftConnectorBrush { get; set; }

        /// <summary>오른쪽 연결선 색(진행이 통과했으면 액센트).</summary>
        public Brush RightConnectorBrush { get; set; }

        /// <summary>첫 단계는 왼쪽 연결선을 숨긴다.</summary>
        public Visibility LeftConnectorVisibility { get; set; }

        /// <summary>마지막 단계는 오른쪽 연결선을 숨긴다.</summary>
        public Visibility RightConnectorVisibility { get; set; }

        /// <summary>셀(단계 하나) 폭 — 가용 폭에 따라 컨트롤이 계산해 넣는다.</summary>
        public double CellWidth { get; set; }

        /// <summary>레이블 최대 폭(셀 폭 - 여백). 넘치면 말줄임된다.</summary>
        public double LabelMaxWidth { get; set; }

        /// <summary>레이블 표시 여부 — 컴팩트/접힘 모드에서는 현재 단계만 보인다.</summary>
        public Visibility LabelVisibility { get; set; }

        /// <summary>셀 전체 툴팁 — 말줄임/레이블 숨김에 대비한 전체 텍스트.</summary>
        public string ToolTipText { get; set; }
    }
}
