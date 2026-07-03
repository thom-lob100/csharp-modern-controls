using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modern.Lab.Controls.Wpf.Input
{
    /// <summary>
    /// Modern single-line text input.
    /// - Text: input text (two-way)
    /// - Placeholder: hint shown while the text is empty
    /// - IsReadOnly: read-only state
    /// - TextChanged: raised whenever Text changes
    /// - EnterPressed: raised when the Enter key is pressed (search-on-enter)
    /// </summary>
    public partial class ModernTextBoxControl : UserControl
    {
        /// <summary>Input text. Two-way by default.</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(ModernTextBoxControl),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextPropertyChanged));

        /// <summary>Hint text shown while the input is empty.</summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                "Placeholder",
                typeof(string),
                typeof(ModernTextBoxControl),
                new PropertyMetadata(string.Empty));

        /// <summary>Read-only state of the editor.</summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(ModernTextBoxControl),
                new PropertyMetadata(false));

        /// <summary>Raised whenever <see cref="Text"/> changes.</summary>
        public event EventHandler TextChanged;

        /// <summary>Raised when the Enter key is pressed inside the editor.</summary>
        public event EventHandler EnterPressed;

        public ModernTextBoxControl()
        {
            this.InitializeComponent();
        }

        /// <summary>Input text.</summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>Hint text shown while the input is empty.</summary>
        public string Placeholder
        {
            get { return (string)this.GetValue(PlaceholderProperty); }
            set { this.SetValue(PlaceholderProperty, value); }
        }

        /// <summary>Read-only state of the editor.</summary>
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>Moves keyboard focus into the editor.</summary>
        public void FocusEditor()
        {
            this.InnerTextBox.Focus();
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModernTextBoxControl control = (ModernTextBoxControl)d;

            if (control.TextChanged != null)
            {
                control.TextChanged(control, EventArgs.Empty);
            }
        }

        private void InnerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.EnterPressed != null)
            {
                this.EnterPressed(this, EventArgs.Empty);
            }
        }
    }
}
