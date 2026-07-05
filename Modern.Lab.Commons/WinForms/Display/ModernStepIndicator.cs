using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Display
{
    /// <summary>
    /// 가로 진행 단계 표시(스텝 인디케이터; 직접 대응하는 WinForms 컨트롤 없음;
    /// WPF ModernStepIndicatorControl을 ElementHost로 호스팅).
    ///
    /// 공정 이벤트 흐름을 한 줄 단계 바로 보여준다. SummaryList와 동일한
    /// 데이터 계약 명명을 따른다: DataSource가 단계 행을, DisplayMember가 단계
    /// 이름 컬럼을, StateMember가 상태 컬럼을 지정한다. 상태 값은 문자열로
    /// "Completed"/"Current"/"Pending"/"Failed"(대소문자 무시).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernStepIndicator : WpfElementHostBase<Modern.Lab.Controls.Wpf.Display.ModernStepIndicatorControl>
    {
        private object dataSource;

        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackDisplayMember;
        private string fallbackStateMember;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernStepIndicator()
        {
            this.Size = new Size(560, 56);
            this.fallbackDisplayMember = string.Empty;
            this.fallbackStateMember = string.Empty;
        }

        /// <summary>
        /// 단계 행 목록: 단계 이름 컬럼과 상태 컬럼을 가진 DataTable, DataView,
        /// IList 또는 임의의 IEnumerable. null이면 단계를 비운다.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                this.dataSource = value;

                if (this.Wpf != null)
                {
                    this.Wpf.ItemsSource = DataSourceConverter.ToItemsSource(value);
                }
            }
        }

        /// <summary>단계 이름으로 쓸 컬럼/속성 이름(WinForms 호환 이름).</summary>
        [Category("모던 컨트롤")]
        [Description("단계 이름으로 사용할 컬럼/속성 이름")]
        [DefaultValue("")]
        public string DisplayMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.LabelMemberPath;
                }

                return this.fallbackDisplayMember;
            }
            set
            {
                this.fallbackDisplayMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.LabelMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 단계 상태로 쓸 컬럼/속성 이름. 값은 문자열
        /// "Completed"/"Current"/"Pending"/"Failed"(대소문자 무시).
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("단계 상태로 사용할 컬럼/속성 이름(Completed/Current/Pending/Failed)")]
        [DefaultValue("")]
        public string StateMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.StateMemberPath;
                }

                return this.fallbackStateMember;
            }
            set
            {
                this.fallbackStateMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.StateMemberPath = value;
                }
            }
        }
    }
}
