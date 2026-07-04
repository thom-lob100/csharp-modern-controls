using System.Windows;
using System.Windows.Controls;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 소형 통계 카드.
    /// - Title: 숫자 위에 표시되는 캡션 (예: "조회 건수")
    /// - Value: 강조할 숫자/텍스트
    /// </summary>
    public partial class ModernKpiCardControl : UserControl
    {
        /// <summary>값 위에 표시되는 캡션.</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(ModernKpiCardControl),
                new PropertyMetadata("제목"));

        /// <summary>강조 표시되는 값 텍스트.</summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(string),
                typeof(ModernKpiCardControl),
                new PropertyMetadata("0"));

        /// <summary>공용 카드 패널 위에서 쓰도록 카드 크롬(테두리/배경/패딩)을 제거한다.</summary>
        public static readonly DependencyProperty FlatProperty =
            DependencyProperty.Register(
                "Flat",
                typeof(bool),
                typeof(ModernKpiCardControl),
                new PropertyMetadata(false));

        public ModernKpiCardControl()
        {
            this.InitializeComponent();
        }

        /// <summary>값 위에 표시되는 캡션.</summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>강조 표시되는 값 텍스트.</summary>
        public string Value
        {
            get { return (string)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>공용 카드 패널 위에서 쓰도록 카드 크롬을 제거한다.</summary>
        public bool Flat
        {
            get { return (bool)this.GetValue(FlatProperty); }
            set { this.SetValue(FlatProperty, value); }
        }
    }
}
