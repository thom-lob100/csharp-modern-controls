using System;
using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Input
{
    /// <summary>
    /// System.Windows.Forms.DateTimePicker의 대체 컨트롤
    /// (WPF ModernDatePickerControl을 ElementHost로 호스팅).
    ///
    /// 호환 멤버: Value / ValueChanged / MinDate / MaxDate, Enabled.
    /// DateTimePicker와의 의도적 차이: Value가 DateTime?(nullable)이고
    /// null이 "미선택(전체)"을 의미한다 — ShowCheckBox/Checked 조합을
    /// null 값 하나로 대체한다. 시간 부분은 지원하지 않는다(날짜 전용).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernDatePicker : WpfElementHostBase<Modern.Lab.Controls.Wpf.Input.ModernDatePickerControl>
    {
        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private DateTime? fallbackValue;
        private DateTime? fallbackMinDate;
        private DateTime? fallbackMaxDate;
        private bool fallbackRequired;
        private string fallbackPlaceholder;

        /// <summary>선택 날짜가 바뀔 때 발생한다(DateTimePicker 호환 이름).</summary>
        public event EventHandler ValueChanged;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernDatePicker()
        {
            this.Size = new Size(130, 32);
            this.fallbackValue = null;
            this.fallbackMinDate = null;
            this.fallbackMaxDate = null;
            this.fallbackRequired = false;
            this.fallbackPlaceholder = "yyyy-MM-dd";

            if (this.Wpf != null)
            {
                this.Wpf.ValueChanged += this.OnWpfValueChanged;
            }
        }

        /// <summary>입력 전(빈 필드) 회색으로 표시되는 형식 안내 텍스트.</summary>
        [Category("모던 컨트롤")]
        [Description("입력 전 회색으로 표시할 형식 안내 텍스트")]
        [Localizable(true)]
        [DefaultValue("yyyy-MM-dd")]
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

        /// <summary>필수 입력 필드 표시 — hover/포커스 시 필드 왼쪽에 옅은 빨간 세로 바.</summary>
        [Category("모던 컨트롤")]
        [Description("필수 입력 표시(마우스 오버/포커스 시 옅은 빨간 세로 바)")]
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

        /// <summary>
        /// 선택된 날짜. null은 미선택(전체 조회)을 의미한다.
        /// 조회 조건 초기화 시 null을 할당한다.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? Value
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.SelectedDate;
                }

                return this.fallbackValue;
            }
            set
            {
                this.fallbackValue = value;

                if (this.Wpf != null)
                {
                    this.Wpf.SelectedDate = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>선택 가능한 최소 날짜 (null = 제한 없음).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? MinDate
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.MinDate;
                }

                return this.fallbackMinDate;
            }
            set
            {
                this.fallbackMinDate = value;

                if (this.Wpf != null)
                {
                    this.Wpf.MinDate = value;
                }
            }
        }

        /// <summary>선택 가능한 최대 날짜 (null = 제한 없음).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? MaxDate
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.MaxDate;
                }

                return this.fallbackMaxDate;
            }
            set
            {
                this.fallbackMaxDate = value;

                if (this.Wpf != null)
                {
                    this.Wpf.MaxDate = value;
                }
            }
        }

        private void OnWpfValueChanged(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
