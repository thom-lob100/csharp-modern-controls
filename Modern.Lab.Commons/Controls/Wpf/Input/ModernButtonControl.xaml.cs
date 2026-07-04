using System.Windows;
using System.Windows.Controls;

namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>
    /// 모던 둥근 버튼 컨트롤.
    /// - Text: 버튼에 표시되는 캡션
    /// - Kind: 색상 세트를 전환하는 시각적 종류 (Primary/Secondary/Danger)
    /// - Click: 버튼이 눌릴 때 발생
    /// </summary>
    public partial class ModernButtonControl : UserControl
    {
        /// <summary>버튼에 표시되는 캡션.</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ModernButtonControl),
                new PropertyMetadata("버튼"));

        /// <summary>시각적 종류(색상 / 강조 수준). 기본값은 Primary.</summary>
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                "Kind",
                typeof(ButtonKind),
                typeof(ModernButtonControl),
                new PropertyMetadata(ButtonKind.Primary));

        /// <summary>캡션 앞에 표시되는 아이콘 글리프(Segoe MDL2 Assets). 비어 있으면 숨긴다.</summary>
        public static readonly DependencyProperty IconGlyphProperty =
            DependencyProperty.Register(
                "IconGlyph",
                typeof(string),
                typeof(ModernButtonControl),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 버튼이 클릭될 때 발생한다. 내부 버튼의 Click을 전달한다.
        /// </summary>
        public event RoutedEventHandler Click;

        public ModernButtonControl()
        {
            this.InitializeComponent();
        }

        /// <summary>버튼에 표시되는 캡션.</summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>시각적 종류 (Primary/Secondary/Danger).</summary>
        public ButtonKind Kind
        {
            get { return (ButtonKind)this.GetValue(KindProperty); }
            set { this.SetValue(KindProperty, value); }
        }

        /// <summary>캡션 앞에 표시되는 아이콘 글리프(Segoe MDL2 Assets).</summary>
        public string IconGlyph
        {
            get { return (string)this.GetValue(IconGlyphProperty); }
            set { this.SetValue(IconGlyphProperty, value); }
        }

        // 내부 버튼의 Click을 이 컨트롤의 Click으로 다시 발생시킨다.
        private void InnerButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
    }
}
