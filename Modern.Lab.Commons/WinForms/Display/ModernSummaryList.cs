using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Display
{
    /// <summary>
    /// 분류/건수 칩 목록(새 개념으로, 직접 대응하는 WinForms 컨트롤 없음;
    /// WPF ModernSummaryListControl을 ElementHost로 호스팅).
    ///
    /// ComboBox 데이터 계약 명명을 따른다: DataSource가 행을 담고,
    /// DisplayMember가 레이블 컬럼을, ValueMember가 건수 컬럼을 지정한다.
    /// 멤버 할당 순서는 상관없다.
    /// </summary>
    [ToolboxItem(true)]
    public class ModernSummaryList : WpfElementHostBase<Modern.Lab.Controls.Wpf.Display.ModernSummaryListControl>
    {
        private object dataSource;

        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackTitle;
        private string fallbackDisplayMember;
        private string fallbackValueMember;
        private string fallbackColorMember;
        private bool fallbackFlat;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernSummaryList()
        {
            this.Size = new Size(320, 88);
            this.fallbackTitle = string.Empty;
            this.fallbackDisplayMember = string.Empty;
            this.fallbackValueMember = string.Empty;
            this.fallbackColorMember = string.Empty;
            this.fallbackFlat = false;
        }

        /// <summary>공용 카드 패널(예: ModernCardPanel) 위에서 쓰도록 카드 크롬을 제거한다.</summary>
        [Category("모던 컨트롤")]
        [Description("카드 테두리/배경 제거 — 카드 판넬 위에 평면 배치할 때 사용")]
        [DefaultValue(false)]
        public bool Flat
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Flat;
                }

                return this.fallbackFlat;
            }
            set
            {
                this.fallbackFlat = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Flat = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }

        /// <summary>
        /// 요약할 행 목록: 레이블 컬럼과 건수 컬럼을 가진 DataTable, DataView,
        /// IList 또는 임의의 IEnumerable. null이면 칩을 비운다.
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

        /// <summary>칩 레이블로 쓸 컬럼/속성 이름(WinForms 호환 이름).</summary>
        [Category("모던 컨트롤")]
        [Description("칩 라벨로 사용할 컬럼/속성 이름")]
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

        /// <summary>칩 건수로 쓸 컬럼/속성 이름(WinForms 호환 이름).</summary>
        [Category("모던 컨트롤")]
        [Description("칩 인원수/건수로 사용할 컬럼/속성 이름")]
        [DefaultValue("")]
        public string ValueMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.CountMemberPath;
                }

                return this.fallbackValueMember;
            }
            set
            {
                this.fallbackValueMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.CountMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 칩 배경색으로 쓸 컬럼/속성 이름. 값은 "#DBEAFE" 같은 hex 또는
        /// "SkyBlue" 같은 색 이름 문자열. 비어 있거나 파싱 불가면 기본색으로 폴백.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("칩 배경색으로 사용할 컬럼/속성 이름(hex 또는 색 이름 문자열; 비우면 기본색)")]
        [DefaultValue("")]
        public string ColorMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.ColorMemberPath;
                }

                return this.fallbackColorMember;
            }
            set
            {
                this.fallbackColorMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.ColorMemberPath = value;
                }
            }
        }

        /// <summary>칩 위에 표시되는 선택적 캡션. 비어 있으면 캡션을 숨긴다.</summary>
        [Category("모던 컨트롤")]
        [Description("칩 목록 위에 표시할 제목(비우면 숨김)")]
        [Localizable(true)]
        [DefaultValue("")]
        public string Title
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Title;
                }

                return this.fallbackTitle;
            }
            set
            {
                this.fallbackTitle = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Title = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }
    }
}
