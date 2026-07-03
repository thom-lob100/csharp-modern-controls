using System.Windows;
using System.Windows.Controls;

namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>
    /// Modern rounded button control.
    /// - Text: caption shown on the button
    /// - Kind: visual kind (Primary/Secondary/Danger) that switches the color set
    /// - Click: raised when the button is pressed
    /// </summary>
    public partial class ModernButtonControl : UserControl
    {
        /// <summary>Caption shown on the button.</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ModernButtonControl),
                new PropertyMetadata("버튼"));

        /// <summary>Visual kind (color / emphasis level). Defaults to Primary.</summary>
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(
                "Kind",
                typeof(ButtonKind),
                typeof(ModernButtonControl),
                new PropertyMetadata(ButtonKind.Primary));

        /// <summary>Icon glyph (Segoe MDL2 Assets) shown before the caption. Empty hides it.</summary>
        public static readonly DependencyProperty IconGlyphProperty =
            DependencyProperty.Register(
                "IconGlyph",
                typeof(string),
                typeof(ModernButtonControl),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Raised when the button is clicked. Forwards the inner button's Click.
        /// </summary>
        public event RoutedEventHandler Click;

        public ModernButtonControl()
        {
            this.InitializeComponent();
        }

        /// <summary>Caption shown on the button.</summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>Visual kind (Primary/Secondary/Danger).</summary>
        public ButtonKind Kind
        {
            get { return (ButtonKind)this.GetValue(KindProperty); }
            set { this.SetValue(KindProperty, value); }
        }

        /// <summary>Icon glyph (Segoe MDL2 Assets) shown before the caption.</summary>
        public string IconGlyph
        {
            get { return (string)this.GetValue(IconGlyphProperty); }
            set { this.SetValue(IconGlyphProperty, value); }
        }

        // Re-raises the inner button's Click as this control's Click.
        private void InnerButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
    }
}
