using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Input
{
    /// <summary>
    /// System.Windows.Forms.TextBox의 드롭인 대체 컨트롤
    /// (WPF ModernTextBoxControl을 ElementHost로 호스팅).
    ///
    /// 호환 멤버: Text(override, localizable), TextChanged(내부 WPF 텍스트가
    /// 바뀔 때 발생하는 표준 WinForms 이벤트), ReadOnly, Enabled,
    /// AutoCompleteMode/AutoCompleteSource/AutoCompleteCustomSource
    /// (검색창 스타일 추천 드롭다운; None이 아닌 모든 모드는 Suggest로
    /// 동작하며 CustomSource만 지원한다).
    /// 추가 멤버: PlaceholderText, EnterPressed(엔터로 검색; 호스팅된 WPF
    /// 에디터 내부에서 처리되는 키에는 WinForms KeyDown 이벤트가 발생하지
    /// 않는다).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernTextBox : WpfElementHostBase<Modern.Lab.Controls.Wpf.Input.ModernTextBoxControl>
    {
        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackText;
        private string fallbackPlaceholder;
        private bool fallbackReadOnly;

        private AutoCompleteMode autoCompleteMode;
        private AutoCompleteSource autoCompleteSource;
        private AutoCompleteStringCollection autoCompleteCustomSource;

        /// <summary>에디터 안에서 Enter 키를 눌렀을 때 발생한다.</summary>
        public event EventHandler EnterPressed;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernTextBox()
        {
            this.Size = new Size(200, 32);
            this.fallbackText = string.Empty;
            this.fallbackPlaceholder = string.Empty;
            this.fallbackReadOnly = false;
            this.autoCompleteMode = AutoCompleteMode.None;
            this.autoCompleteSource = AutoCompleteSource.None;
            this.autoCompleteCustomSource = null;

            if (this.Wpf != null)
            {
                this.Wpf.TextChanged += this.OnWpfTextChanged;
                this.Wpf.EnterPressed += this.OnWpfEnterPressed;
            }
        }

        /// <summary>입력 텍스트.</summary>
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

        /// <summary>입력이 비어 있는 동안 표시되는 힌트 텍스트.</summary>
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

        /// <summary>읽기 전용 상태(TextBox.ReadOnly와 동일한 의미).</summary>
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

        /// <summary>
        /// 자동완성 동작(WinForms 호환 이름). None 이외의 값은 모두
        /// 추천 드롭다운을 활성화한다(Suggest 동작).
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("자동완성 동작 — None 외의 값은 모두 제안 목록(Suggest)으로 동작")]
        [DefaultValue(AutoCompleteMode.None)]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return this.autoCompleteMode;
            }
            set
            {
                this.autoCompleteMode = value;
                this.ApplyAutoComplete();
            }
        }

        /// <summary>
        /// 자동완성 원본(WinForms 호환 이름). CustomSource만 지원하며,
        /// 다른 값은 드롭다운을 비활성화한다.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("자동완성 원본 — CustomSource만 지원")]
        [DefaultValue(AutoCompleteSource.None)]
        public AutoCompleteSource AutoCompleteSource
        {
            get
            {
                return this.autoCompleteSource;
            }
            set
            {
                this.autoCompleteSource = value;
                this.ApplyAutoComplete();
            }
        }

        /// <summary>
        /// 사용자 지정 자동완성 후보(WinForms 호환 이름과 타입).
        /// 채운 뒤 할당하고, 후보를 갱신하려면 다시 할당한다.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
                return this.autoCompleteCustomSource;
            }
            set
            {
                this.autoCompleteCustomSource = value;
                this.ApplyAutoComplete();
            }
        }

        // 유효한 후보 목록을 WPF 컨트롤에 전달한다. 순서에 관대함:
        // Mode/Source/CustomSource는 어떤 순서로 할당해도 된다(계약 규칙 3).
        private void ApplyAutoComplete()
        {
            if (this.Wpf == null)
            {
                return;
            }

            bool enabled =
                this.autoCompleteMode != AutoCompleteMode.None &&
                this.autoCompleteSource == AutoCompleteSource.CustomSource &&
                this.autoCompleteCustomSource != null;

            this.Wpf.AutoCompleteItemsSource = enabled ? this.autoCompleteCustomSource : null;
        }

        // 표준 WinForms TextChanged를 발생시켜, 기존 핸들러 연결
        // (this.textBox.TextChanged += ...)이 교체 후에도 계속 동작하게 한다.
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
