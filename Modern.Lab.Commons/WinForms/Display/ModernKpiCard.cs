using System.ComponentModel;
using System.Drawing;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Display
{
    /// <summary>
    /// 소형 통계 카드(수작업 Label 한 쌍을 대체; WPF ModernKpiCardControl을
    /// ElementHost로 호스팅).
    ///
    /// Title은 한 번 설정하고, 매 조회 후 Value를 갱신한다
    /// (예: Value = grid.RowCount.ToString()).
    /// </summary>
    [ToolboxItem(true)]
    public class ModernKpiCard : WpfElementHostBase<Modern.Lab.Controls.Wpf.Display.ModernKpiCardControl>
    {
        // 디자인 타임 WPF 생성이 실패한 경우(Wpf == null)에도 속성 그리드가
        // 동작하도록 하는 폴백 저장소.
        private string fallbackTitle;
        private string fallbackValue;
        private bool fallbackFlat;

        /// <summary>적절한 기본 크기로 컨트롤을 생성한다.</summary>
        public ModernKpiCard()
        {
            this.Size = new Size(160, 72);
            this.fallbackTitle = "제목";
            this.fallbackValue = "0";
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

        /// <summary>값 위에 표시되는 캡션.</summary>
        [Category("모던 컨트롤")]
        [Description("값 위에 표시할 제목")]
        [Localizable(true)]
        [DefaultValue("제목")]
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

        /// <summary>강조 표시되는 값 텍스트.</summary>
        [Category("모던 컨트롤")]
        [Description("강조 표시할 값 텍스트")]
        [DefaultValue("0")]
        public string Value
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.Value;
                }

                return this.fallbackValue;
            }
            set
            {
                this.fallbackValue = value;

                if (this.Wpf != null)
                {
                    this.Wpf.Value = value;
                }

                this.InvalidateDesignTimePreview();
            }
        }
    }
}
