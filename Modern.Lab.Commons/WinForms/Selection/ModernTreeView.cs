using System;
using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Selection
{
    /// <summary>
    /// System.Windows.Forms.TreeView의 대체 컨트롤 (조직도·분류 계층 선택;
    /// WPF ModernTreeViewControl을 ElementHost로 호스팅).
    ///
    /// TreeNode를 코드로 쌓는 대신, 서버 조직 테이블(평면 자기참조:
    /// 키/부모키/명칭)을 DataSource로 그대로 할당한다. 부모 키가 비어 있거나
    /// 목록에 없는 행이 루트가 된다.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("SelectedValueChanged")]
    public class ModernTreeView : WpfElementHostBase<Modern.Lab.Controls.Wpf.Selection.ModernTreeViewControl>
    {
        private object dataSource;

        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackIdMember;
        private string fallbackParentIdMember;
        private string fallbackDisplayMember;
        private string fallbackForeColorMember;
        private string fallbackIconMember;
        private string fallbackSubTextMember;
        private string fallbackBadgeMember;
        private string fallbackBadgeColorMember;
        private bool fallbackShowGuideLines;
        private string fallbackEmptyText = "No data";

        /// <summary>선택 노드가 바뀔 때 발생한다.</summary>
        public event EventHandler SelectedValueChanged;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernTreeView()
        {
            this.Size = new Size(200, 300);
            this.fallbackIdMember = string.Empty;
            this.fallbackParentIdMember = string.Empty;
            this.fallbackDisplayMember = string.Empty;
            this.fallbackForeColorMember = string.Empty;
            this.fallbackIconMember = string.Empty;
            this.fallbackSubTextMember = string.Empty;
            this.fallbackBadgeMember = string.Empty;
            this.fallbackBadgeColorMember = string.Empty;
            this.fallbackShowGuideLines = false;

            if (this.Wpf != null)
            {
                this.Wpf.SelectedValueChanged += this.OnWpfSelectedValueChanged;
            }
        }

        /// <summary>
        /// 트리를 구성할 행 목록: 키/부모키/명칭 컬럼을 가진 DataTable, DataView,
        /// IList 또는 임의의 IEnumerable. null이면 트리를 비운다.
        /// 재할당 시 기존 SelectedValue가 새 트리에 없으면 미선택으로 초기화된다.
        /// DataTable/DataView 소스는 Id/ParentId/Display/ForeColor 멤버 컬럼이 없으면
        /// 빈 컬럼으로 자동 보장한다 — 폼에서 컬럼 목록을 다시 나열할 필요가 없다.
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

                DataSourceConverter.EnsureColumns(value, new string[]
                {
                    this.IdMember, this.ParentIdMember, this.DisplayMember, this.ForeColorMember,
                    this.IconMember, this.SubTextMember, this.BadgeMember, this.BadgeColorMember
                });

                if (this.Wpf != null)
                {
                    this.Wpf.ItemsSource = DataSourceConverter.ToItemsSource(value);
                }
            }
        }

        /// <summary>노드 키로 사용할 컬럼/속성 이름.</summary>
        [Category("모던 컨트롤")]
        [Description("노드 키로 사용할 컬럼/속성 이름")]
        [DefaultValue("")]
        public string IdMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.IdMemberPath;
                }

                return this.fallbackIdMember;
            }
            set
            {
                this.fallbackIdMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.IdMemberPath = value;
                }
            }
        }

        /// <summary>부모 키로 사용할 컬럼/속성 이름.</summary>
        [Category("모던 컨트롤")]
        [Description("부모 키로 사용할 컬럼/속성 이름")]
        [DefaultValue("")]
        public string ParentIdMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.ParentIdMemberPath;
                }

                return this.fallbackParentIdMember;
            }
            set
            {
                this.fallbackParentIdMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.ParentIdMemberPath = value;
                }
            }
        }

        /// <summary>노드 표시 텍스트로 사용할 컬럼/속성 이름(WinForms 호환 이름).</summary>
        [Category("모던 컨트롤")]
        [Description("노드 표시 텍스트로 사용할 컬럼/속성 이름")]
        [DefaultValue("")]
        public string DisplayMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.DisplayMemberPath;
                }

                return this.fallbackDisplayMember;
            }
            set
            {
                this.fallbackDisplayMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.DisplayMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 노드 텍스트 색으로 사용할 컬럼/속성 이름 (선택 사항).
        /// 값은 "#DC2626" 같은 색 문자열 — 비어 있거나 해석 불가면 기본색.
        /// 상태(Scrap 등)에 따라 노드를 색으로 구분할 때 쓴다.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("노드 텍스트 색 컬럼/속성 이름 — 값은 #RRGGBB 색 문자열")]
        [DefaultValue("")]
        public string ForeColorMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.ForeColorMemberPath;
                }

                return this.fallbackForeColorMember;
            }
            set
            {
                this.fallbackForeColorMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.ForeColorMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 노드 글리프로 사용할 컬럼/속성 이름 (선택 사항). 값은 프리셋 이름
        /// (Disc/Chip/Slice/Stack/Box/Folder/Dot, 대소문자 무시) 또는 Segoe MDL2
        /// Assets 글리프 16진 코드("E950"). 비었거나 해석 불가하면 아이콘 없음.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("노드 글리프 컬럼/속성 이름 — 값은 프리셋(Disc/Chip/Slice/Stack/Box/Folder/Dot) 또는 MDL2 16진 코드")]
        [DefaultValue("")]
        public string IconMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.IconMemberPath;
                }

                return this.fallbackIconMember;
            }
            set
            {
                this.fallbackIconMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.IconMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 보조 텍스트로 사용할 컬럼/속성 이름 (선택 사항). 주 텍스트 뒤에
        /// 흐린 색으로 붙는다 — 모델/분류처럼 ID만으로 부족한 문맥 한 조각.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("보조 텍스트 컬럼/속성 이름 — 주 텍스트 뒤에 흐린 색으로 표시")]
        [DefaultValue("")]
        public string SubTextMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.SubTextMemberPath;
                }

                return this.fallbackSubTextMember;
            }
            set
            {
                this.fallbackSubTextMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.SubTextMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 행 오른쪽 끝 상태 배지 텍스트로 사용할 컬럼/속성 이름 (선택 사항).
        /// 값이 빈 행은 배지를 그리지 않는다. BadgeColorMember와 짝.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("상태 배지 텍스트 컬럼/속성 이름 — 행 오른쪽 끝 알약으로 표시")]
        [DefaultValue("")]
        public string BadgeMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.BadgeMemberPath;
                }

                return this.fallbackBadgeMember;
            }
            set
            {
                this.fallbackBadgeMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.BadgeMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 배지 배경색으로 사용할 컬럼/속성 이름 (선택 사항). 값은 "#FEE2E2"
        /// 같은 색 문자열 — 글자색은 배경에서 자동 유도(그리드 배지와 동일 규칙).
        /// 비었거나 해석 불가하면 중립 회색 배지.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("배지 배경색 컬럼/속성 이름 — 값은 #RRGGBB 색 문자열, 글자색 자동")]
        [DefaultValue("")]
        public string BadgeColorMember
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.BadgeColorMemberPath;
                }

                return this.fallbackBadgeColorMember;
            }
            set
            {
                this.fallbackBadgeColorMember = value;

                if (this.Wpf != null)
                {
                    this.Wpf.BadgeColorMemberPath = value;
                }
            }
        }

        /// <summary>
        /// 들여쓰기 레벨마다 옅은 세로 가이드라인을 그린다 (기본 false).
        /// 3단 이상 깊은 계보에서 부모-자식 소속을 또렷하게 한다.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("들여쓰기 가이드라인 표시 여부 (기본 false)")]
        [DefaultValue(false)]
        public bool ShowGuideLines
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.ShowGuideLines;
                }

                return this.fallbackShowGuideLines;
            }
            set
            {
                this.fallbackShowGuideLines = value;

                if (this.Wpf != null)
                {
                    this.Wpf.ShowGuideLines = value;
                }
            }
        }

        /// <summary>
        /// 노드가 하나도 없을 때 가운데에 표시할 안내 문구.
        /// 기본 "No data" — 화면 문맥에 맞게 바꾸거나 빈 문자열로 끈다.
        /// </summary>
        [Category("모던 컨트롤")]
        [Description("노드 0개일 때 표시할 안내 문구 (빈 문자열 = 표시 안 함)")]
        [DefaultValue("No data")]
        [Localizable(true)]
        public string EmptyText
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.EmptyText;
                }

                return this.fallbackEmptyText;
            }
            set
            {
                this.fallbackEmptyText = value;

                if (this.Wpf != null)
                {
                    this.Wpf.EmptyText = value;
                }
            }
        }

        /// <summary>
        /// 선택 노드의 키 (IdMember 기준). null = 미선택.
        /// DataSource보다 먼저 설정해도 트리 구성 시 적용되고,
        /// 설정 시 선택 노드가 보이도록 조상이 자동 펼쳐진다.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedValue
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.SelectedValue;
                }

                return null;
            }
            set
            {
                if (this.Wpf != null)
                {
                    this.Wpf.SelectedValue = value;
                }
            }
        }

        /// <summary>선택 노드의 원본 행 (DataRowView 등). 미선택이면 null.</summary>
        [Browsable(false)]
        public object SelectedItem
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.SelectedItem;
                }

                return null;
            }
        }

        /// <summary>모든 노드를 펼친다.</summary>
        public void ExpandAll()
        {
            if (this.Wpf != null)
            {
                this.Wpf.ExpandAll();
            }
        }

        /// <summary>모든 노드를 접는다.</summary>
        public void CollapseAll()
        {
            if (this.Wpf != null)
            {
                this.Wpf.CollapseAll();
            }
        }

        private void OnWpfSelectedValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedValueChanged != null)
            {
                this.SelectedValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
