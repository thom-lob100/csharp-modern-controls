using System.Drawing;

namespace Modern.Lab.Theming
{
    /// <summary>
    /// 테마 선택 플래그 + GDI+ 컨트롤용 중앙 팔레트.
    ///
    /// 두 파이프라인을 하나의 플래그로 묶는다:
    ///   - WPF(ElementHost) 컨트롤: XAML의 StaticResource가 <c>Themes/Tokens.xaml</c>를
    ///     쓰는데, Light가 아니면 <c>SharedResourceDictionary</c>가
    ///     <c>Tokens.&lt;테마&gt;.xaml</c> 오버라이드를 뒤에 병합해 테마 값을 집게 한다.
    ///   - 순수 GDI+ 컨트롤(ModernLabel/StatusBadge/CardPanel/GroupBox/SplitContainer):
    ///     XAML을 읽을 수 없으므로 여기 팔레트 색을 읽는다.
    ///
    /// 테마 구성:
    ///   - Light(기본) / Dark — 기존 그대로.
    ///   - Gray — 어두운 슬레이트 회색 테마 (다크 계열; Dark보다 한 톤 밝은 dim).
    ///   - Purple / Orange / Tomato — 밝은 파스텔 테마 (흰 카드 + 뚜렷한 파스텔
    ///     배경/테두리 틴트 + 채도 있는 액센트).
    ///
    /// <see cref="Mode"/>는 <b>앱 시작 시 첫 컨트롤 생성 전에 한 번</b> 설정한다
    /// (런타임 토글은 지원하지 않는다 — WPF StaticResource가 로드 시 확정되기 때문).
    /// 설정하지 않으면 라이트라서, 이 라이브러리를 쓰는 다른 시스템은 영향받지 않는다.
    /// </summary>
    public static class ModernTheme
    {
        /// <summary>테마 종류.</summary>
        public enum ThemeMode
        {
            /// <summary>기본값 — 기존 라이트 Fluent 테마 (블루 액센트).</summary>
            Light,

            /// <summary>어두운 배경 테마.</summary>
            Dark,

            /// <summary>어두운 슬레이트 회색 테마 (다크 계열 dim).</summary>
            Gray,

            /// <summary>바이올렛 파스텔 테마 (라이트 기반).</summary>
            Purple,

            /// <summary>오렌지 파스텔 테마 (라이트 기반).</summary>
            Orange,

            /// <summary>토마토 파스텔 테마 (라이트 기반).</summary>
            Tomato
        }

        /// <summary>현재 테마. 앱 시작 시 한 번 설정한다.</summary>
        public static ThemeMode Mode { get; set; } = ThemeMode.Light;

        /// <summary>다크 테마 여부 (Dark 전용 플래그).</summary>
        public static bool IsDark
        {
            get { return Mode == ThemeMode.Dark; }
        }

        /// <summary>
        /// 어두운 표면 계열 테마 여부 (Dark, Gray) — 다크 타이틀바, 밝은 텍스트 등
        /// "어두운 화면" 공통 처리가 걸리는 기준.
        /// </summary>
        public static bool IsDarkBased
        {
            get { return Mode == ThemeMode.Dark || Mode == ThemeMode.Gray; }
        }

        // ---- GDI+ 팔레트 (Themes/Tokens.xaml의 색을 미러링; 테마별 대응 값) ----

        /// <summary>카드/입력 표면 (Brush.Surface)</summary>
        public static Color Surface
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(42, 42, 43);
                    case ThemeMode.Gray: return Rgb(53, 56, 62);
                    default: return Rgb(255, 255, 255);
                }
            }
        }

        /// <summary>폼/페이지 바탕 (Brush.Background) — 테마 톤이 가장 크게 갈리는 곳.</summary>
        public static Color Background
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(30, 30, 30);
                    case ThemeMode.Gray: return Rgb(43, 46, 51);
                    case ThemeMode.Purple: return Rgb(234, 227, 248);
                    case ThemeMode.Orange: return Rgb(250, 231, 211);
                    case ThemeMode.Tomato: return Rgb(251, 227, 221);
                    default: return Rgb(243, 244, 246);
                }
            }
        }

        /// <summary>기본 테두리 (Brush.Border)</summary>
        public static Color Border
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(63, 63, 70);
                    case ThemeMode.Gray: return Rgb(74, 78, 85);
                    case ThemeMode.Purple: return Rgb(216, 204, 240);
                    case ThemeMode.Orange: return Rgb(240, 217, 190);
                    case ThemeMode.Tomato: return Rgb(242, 207, 198);
                    default: return Rgb(209, 213, 219);
                }
            }
        }

        /// <summary>옅은 테두리/구분선 (Brush.BorderSubtle)</summary>
        public static Color BorderSubtle
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(58, 58, 60);
                    case ThemeMode.Gray: return Rgb(67, 71, 77);
                    case ThemeMode.Purple: return Rgb(228, 220, 244);
                    case ThemeMode.Orange: return Rgb(247, 229, 208);
                    case ThemeMode.Tomato: return Rgb(248, 222, 215);
                    default: return Rgb(229, 231, 235);
                }
            }
        }

        /// <summary>본문/제목 텍스트 (Brush.TextPrimary)</summary>
        public static Color TextPrimary
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(230, 230, 230);
                    case ThemeMode.Gray: return Rgb(226, 228, 231);
                    default: return Rgb(17, 24, 39);
                }
            }
        }

        /// <summary>보조 텍스트 (Brush.TextSecondary)</summary>
        public static Color TextSecondary
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(156, 163, 175);
                    case ThemeMode.Gray: return Rgb(166, 171, 179);
                    default: return Rgb(107, 114, 128);
                }
            }
        }

        /// <summary>비활성 텍스트 (Brush.DisabledText)</summary>
        public static Color DisabledText
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(107, 114, 128);
                    case ThemeMode.Gray: return Rgb(118, 124, 133);
                    default: return Rgb(156, 163, 175);
                }
            }
        }

        /// <summary>액센트 (Brush.Accent) — 테마의 주 색. 어두운 계열에서는 밝게.</summary>
        public static Color Accent
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(59, 158, 255);
                    case ThemeMode.Gray: return Rgb(123, 144, 168);
                    case ThemeMode.Purple: return Rgb(124, 58, 237);
                    case ThemeMode.Orange: return Rgb(234, 88, 12);
                    case ThemeMode.Tomato: return Rgb(217, 68, 43);
                    default: return Rgb(0, 120, 212);
                }
            }
        }

        /// <summary>필수 표시/위험 빨강 (Brush.ErrorBorder)</summary>
        public static Color RequiredRed
        {
            get { return IsDarkBased ? Rgb(240, 113, 113) : Rgb(220, 38, 38); }
        }

        /// <summary>중립 배지 배경 (Brush.NeutralBackground)</summary>
        public static Color NeutralBackground
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(58, 58, 60);
                    case ThemeMode.Gray: return Rgb(67, 71, 77);
                    default: return Rgb(243, 244, 246);
                }
            }
        }

        /// <summary>중립 배지 텍스트 (Brush.NeutralText)</summary>
        public static Color NeutralText
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(209, 213, 219);
                    case ThemeMode.Gray: return Rgb(201, 205, 211);
                    default: return Rgb(55, 65, 81);
                }
            }
        }

        /// <summary>선택 강조 배경 (Brush.SelectedBackground)</summary>
        public static Color SelectionBackground
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(22, 58, 94);
                    case ThemeMode.Gray: return Rgb(62, 74, 90);
                    case ThemeMode.Purple: return Rgb(221, 207, 247);
                    case ThemeMode.Orange: return Rgb(252, 224, 192);
                    case ThemeMode.Tomato: return Rgb(252, 214, 204);
                    default: return Rgb(182, 217, 242);
                }
            }
        }

        /// <summary>보조 표면 — 카드 위 살짝 다른 톤 (Brush.SurfaceAlt / HeaderBackground)</summary>
        public static Color SurfaceAlt
        {
            get
            {
                switch (Mode)
                {
                    case ThemeMode.Dark: return Rgb(50, 50, 51);
                    case ThemeMode.Gray: return Rgb(60, 64, 70);
                    case ThemeMode.Purple: return Rgb(243, 238, 251);
                    case ThemeMode.Orange: return Rgb(253, 242, 230);
                    case ThemeMode.Tomato: return Rgb(253, 240, 236);
                    default: return Rgb(249, 250, 251);
                }
            }
        }

        private static Color Rgb(int r, int g, int b)
        {
            return Color.FromArgb(255, r, g, b);
        }
    }
}
