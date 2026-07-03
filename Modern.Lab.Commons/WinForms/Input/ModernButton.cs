using System;
using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Input
{
    /// <summary>
    /// Drop-in replacement for System.Windows.Forms.Button
    /// (WPF ModernButtonControl hosted through ElementHost).
    ///
    /// Compatible members: Text (override, localizable), Click, Enabled.
    /// Text overrides Control.Text instead of hiding it with `new` — hiding
    /// breaks ComponentResourceManager.ApplyResources on Localizable forms and
    /// any host code that assigns Text through a Control reference
    /// (docs/design-notes.md section 3).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernButton : WpfElementHostBase<Modern.Lab.Controls.Wpf.Input.ModernButtonControl>
    {
        // Fallback storage so the property grid still works when design-time
        // WPF construction failed (Wpf == null).
        private string fallbackText;
        private Modern.Lab.Controls.Wpf.Input.ButtonKind fallbackKind;
        private string fallbackIconGlyph;

        /// <summary>Creates the control with a sensible default size.</summary>
        public ModernButton()
        {
            this.Size = new Size(120, 32);
            this.fallbackText = "버튼";
            this.fallbackKind = Modern.Lab.Controls.Wpf.Input.ButtonKind.Primary;
            this.fallbackIconGlyph = string.Empty;

            if (this.Wpf != null)
            {
                // Forward the WPF click as the standard WinForms Click event.
                this.Wpf.Click += this.OnWpfButtonClick;
            }
        }

        /// <summary>Caption shown on the button.</summary>
        [Category("모던 컨트롤")]
        [Description("버튼에 표시할 텍스트")]
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("버튼")]
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

        /// <summary>Visual kind (Primary/Secondary/Danger).</summary>
        [Category("모던 컨트롤")]
        [Description("버튼 종류(Primary/Secondary/Danger)")]
        [DefaultValue(Modern.Lab.Controls.Wpf.Input.ButtonKind.Primary)]
        public Modern.Lab.Controls.Wpf.Input.ButtonKind Kind
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Kind;
                }

                return this.fallbackKind;
            }
            set
            {
                this.fallbackKind = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Kind = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>Icon glyph (Segoe MDL2 Assets) shown before the caption.</summary>
        [Category("모던 컨트롤")]
        [Description("버튼 글자 앞 아이콘 글리프(Segoe MDL2 Assets)")]
        [DefaultValue("")]
        public string IconGlyph
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.IconGlyph;
                }

                return this.fallbackIconGlyph;
            }
            set
            {
                this.fallbackIconGlyph = value;

                if (this.Wpf != null)
                {
                    this.Wpf.IconGlyph = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        private void OnWpfButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.OnClick(EventArgs.Empty);
        }
    }
}
