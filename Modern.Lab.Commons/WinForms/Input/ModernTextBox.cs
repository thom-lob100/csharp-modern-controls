using System;
using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Input
{
    /// <summary>
    /// Drop-in replacement for System.Windows.Forms.TextBox
    /// (WPF ModernTextBoxControl hosted through ElementHost).
    ///
    /// Compatible members: Text (override, localizable), TextChanged (the
    /// standard WinForms event, raised when the inner WPF text changes),
    /// ReadOnly, Enabled. Additional members: PlaceholderText, EnterPressed
    /// (search-on-enter; the WinForms KeyDown event does not fire for keys
    /// handled inside the hosted WPF editor).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernTextBox : WpfElementHostBase<Modern.Lab.Controls.Wpf.Input.ModernTextBoxControl>
    {
        // Fallback storage so the property grid still works when design-time
        // WPF construction failed (Wpf == null).
        private string fallbackText;
        private string fallbackPlaceholder;
        private bool fallbackReadOnly;

        /// <summary>Raised when the Enter key is pressed inside the editor.</summary>
        public event EventHandler EnterPressed;

        /// <summary>Creates the control with a sensible default size.</summary>
        public ModernTextBox()
        {
            this.Size = new Size(200, 32);
            this.fallbackText = string.Empty;
            this.fallbackPlaceholder = string.Empty;
            this.fallbackReadOnly = false;

            if (this.Wpf != null)
            {
                this.Wpf.TextChanged += this.OnWpfTextChanged;
                this.Wpf.EnterPressed += this.OnWpfEnterPressed;
            }
        }

        /// <summary>Input text.</summary>
        [Category("모던 컨트롤")]
        [Description("입력하거나 표시할 텍스트")]
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Text;
                }

                return this.fallbackText;
            }
            set
            {
                this.fallbackText = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Text = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>Hint text shown while the input is empty.</summary>
        [Category("모던 컨트롤")]
        [Description("입력이 비어 있을 때 표시할 힌트 텍스트")]
        [Localizable(true)]
        [DefaultValue("")]
        public string PlaceholderText
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Placeholder;
                }

                return this.fallbackPlaceholder;
            }
            set
            {
                this.fallbackPlaceholder = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Placeholder = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>Read-only state (same semantics as TextBox.ReadOnly).</summary>
        [Category("모던 컨트롤")]
        [Description("읽기 전용 여부")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.IsReadOnly;
                }

                return this.fallbackReadOnly;
            }
            set
            {
                this.fallbackReadOnly = value;

                if (this.Wpf != null)
                {
                    this.Wpf.IsReadOnly = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        // Raises the standard WinForms TextChanged so existing handler wiring
        // (this.textBox.TextChanged += ...) keeps working after the swap.
        private void OnWpfTextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(EventArgs.Empty);
        }

        private void OnWpfEnterPressed(object sender, EventArgs e)
        {
            if (this.EnterPressed != null)
            {
                this.EnterPressed(this, EventArgs.Empty);
            }
        }
    }
}
