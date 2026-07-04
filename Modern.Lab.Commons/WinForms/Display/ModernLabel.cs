using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Display
{
    /// <summary>
    /// System.Windows.Forms.Label의 드롭인 대체 컨트롤
    /// (WPF ModernLabelControl을 ElementHost로 호스팅).
    ///
    /// 호환 멤버: Text(override, localizable), Enabled.
    /// Label과 달리 AutoSize가 없으므로 컨트롤에 명시적 크기를 지정한다
    /// (긴 텍스트는 말줄임표로 잘린다).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernLabel : WpfElementHostBase<Modern.Lab.Controls.Wpf.Display.ModernLabelControl>
    {
        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackText;
        private Modern.Lab.Controls.Wpf.Display.LabelKind fallbackKind;
        private bool fallbackRequired;
        private bool fallbackTitleBar;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernLabel()
        {
            this.Size = new Size(120, 24);
            this.fallbackText = "레이블";
            this.fallbackKind = Modern.Lab.Controls.Wpf.Display.LabelKind.Body;
            this.fallbackRequired = false;
            this.fallbackTitleBar = false;
        }

        /// <summary>Kind가 Title일 때 텍스트 왼쪽에 액센트색 세로 타이틀 바를 표시한다.</summary>
        [Category("모던 컨트롤")]
        [Description("Kind가 Title일 때 왼쪽에 액센트색 세로 타이틀 바 표시")]
        [DefaultValue(false)]
        public bool TitleBar
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.TitleBar;
                }

                return this.fallbackTitleBar;
            }
            set
            {
                this.fallbackTitleBar = value;

                if (this.Wpf != null)
                {
                    this.Wpf.TitleBar = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>텍스트 뒤에 빨간 별표를 표시한다(필수 필드 표시).</summary>
        [Category("모던 컨트롤")]
        [Description("필수 입력 표시(빨간 별표)를 텍스트 뒤에 붙일지 여부")]
        [DefaultValue(false)]
        public bool Required
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Required;
                }

                return this.fallbackRequired;
            }
            set
            {
                this.fallbackRequired = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Required = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>표시할 텍스트.</summary>
        [Category("모던 컨트롤")]
        [Description("표시할 텍스트")]
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("레이블")]
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

        /// <summary>타이포그래피 역할 (Body/Title/Label/Helper).</summary>
        [Category("모던 컨트롤")]
        [Description("타이포그래피 역할(Body/Title/Label/Helper)")]
        [DefaultValue(Modern.Lab.Controls.Wpf.Display.LabelKind.Body)]
        public Modern.Lab.Controls.Wpf.Display.LabelKind Kind
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
    }
}
