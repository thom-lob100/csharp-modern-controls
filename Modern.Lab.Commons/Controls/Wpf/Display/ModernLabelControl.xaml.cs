using System.Windows;
using System.Windows.Controls;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 모던 텍스트 레이블.
    /// - Text: 표시할 텍스트
    /// - Kind: 토큰 타입 램프 기반 타이포그래피 역할 (Body/Title/Label/Helper)
    /// </summary>
    public partial class ModernLabelControl : UserControl
    {
        /// <summary>표시할 텍스트.</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ModernLabelControl),
                new PropertyMetadata("레이블"));

        /// <summary>타이포그래피 역할. 기본값은 Body.</summary>
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                "Kind",
                typeof(LabelKind),
                typeof(ModernLabelControl),
                new PropertyMetadata(LabelKind.Body));

        /// <summary>텍스트 뒤에 빨간 별표를 표시한다(필수 필드 표시).</summary>
        public static readonly DependencyProperty RequiredProperty =
            DependencyProperty.Register(
                "Required",
                typeof(bool),
                typeof(ModernLabelControl),
                new PropertyMetadata(false));

        /// <summary>
        /// Kind가 Title일 때 텍스트 왼쪽에 액센트색 세로 타이틀 바를 표시한다.
        /// Title이 아닌 Kind에서는 값과 무관하게 표시되지 않는다.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty =
            DependencyProperty.Register(
                "TitleBar",
                typeof(bool),
                typeof(ModernLabelControl),
                new PropertyMetadata(false));

        public ModernLabelControl()
        {
            this.InitializeComponent();
        }

        /// <summary>표시할 텍스트.</summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>타이포그래피 역할 (Body/Title/Label/Helper).</summary>
        public LabelKind Kind
        {
            get { return (LabelKind)this.GetValue(KindProperty); }
            set { this.SetValue(KindProperty, value); }
        }

        /// <summary>텍스트 뒤에 빨간 별표를 표시한다(필수 필드 표시).</summary>
        public bool Required
        {
            get { return (bool)this.GetValue(RequiredProperty); }
            set { this.SetValue(RequiredProperty, value); }
        }

        /// <summary>Kind가 Title일 때 왼쪽에 액센트색 세로 타이틀 바를 표시한다.</summary>
        public bool TitleBar
        {
            get { return (bool)this.GetValue(TitleBarProperty); }
            set { this.SetValue(TitleBarProperty, value); }
        }
    }
}
