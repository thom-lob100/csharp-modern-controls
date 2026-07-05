using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Modern.Lab.Controls.Wpf.Common;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 가로 진행 단계 표시(스텝 인디케이터). 공정 이벤트 흐름
    /// (예: Created → Released → JobPrep → JobStart → JobEnd, 또는 → Scrapped)을
    /// 한 줄로 보여 "지금 어디까지 왔는지"를 한눈에 파악하게 한다.
    ///
    /// - ItemsSource: 임의의 IEnumerable (DataView, IList, ...)
    /// - LabelMemberPath: 단계 이름 컬럼/속성
    /// - StateMemberPath: 단계 상태 컬럼/속성 — 값은 ModernStepState 이름 문자열
    ///   ("Completed"/"Current"/"Pending"/"Failed", 대소문자 무시)
    ///
    /// 소스나 멤버 경로가 바뀔 때마다 다시 그리므로 할당 순서는 상관없다(계약 규칙 3).
    /// 색·글리프는 전부 디자인 토큰(Themes/Tokens.xaml)에서 가져온다.
    /// </summary>
    public partial class ModernStepIndicatorControl : UserControl
    {
        // Segoe MDL2 Assets 글리프: 완료 체크(E73E) / 실패 X(E711).
        private const string GlyphCompleted = "";
        private const string GlyphFailed = "";
        private const string GlyphCurrent = "●"; // ● 검은 원 (본문 폰트)

        /// <summary>단계 행 목록. 임의의 IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernStepIndicatorControl),
                new PropertyMetadata(null, OnDataShapeChanged));

        /// <summary>단계 이름으로 쓸 컬럼/속성 이름.</summary>
        public static readonly DependencyProperty LabelMemberPathProperty =
            DependencyProperty.Register(
                "LabelMemberPath",
                typeof(string),
                typeof(ModernStepIndicatorControl),
                new PropertyMetadata(string.Empty, OnDataShapeChanged));

        /// <summary>단계 상태로 쓸 컬럼/속성 이름(값은 ModernStepState 이름 문자열).</summary>
        public static readonly DependencyProperty StateMemberPathProperty =
            DependencyProperty.Register(
                "StateMemberPath",
                typeof(string),
                typeof(ModernStepIndicatorControl),
                new PropertyMetadata(string.Empty, OnDataShapeChanged));

        private readonly ObservableCollection<StepIndicatorItem> stepModels;

        public ModernStepIndicatorControl()
        {
            this.stepModels = new ObservableCollection<StepIndicatorItem>();
            this.InitializeComponent();
            this.StepItemsControl.ItemsSource = this.stepModels;
        }

        /// <summary>단계 행 목록.</summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>단계 이름으로 쓸 컬럼/속성 이름.</summary>
        public string LabelMemberPath
        {
            get { return (string)this.GetValue(LabelMemberPathProperty); }
            set { this.SetValue(LabelMemberPathProperty, value); }
        }

        /// <summary>단계 상태로 쓸 컬럼/속성 이름.</summary>
        public string StateMemberPath
        {
            get { return (string)this.GetValue(StateMemberPathProperty); }
            set { this.SetValue(StateMemberPathProperty, value); }
        }

        private static void OnDataShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernStepIndicatorControl)d).RebuildSteps();
        }

        // 소스 행을 단계 모델로 변환한다. 각 단계의 색/글리프/연결선을 상태에 맞춰
        // 미리 계산해 담는다. null/빈 소스는 빈 표시로 렌더링하며 예외를 던지지 않는다.
        private void RebuildSteps()
        {
            this.stepModels.Clear();

            IEnumerable source = this.ItemsSource;

            if (source == null)
            {
                return;
            }

            // 먼저 (레이블, 상태)만 뽑아 목록으로 만든다 — 첫/마지막 연결선 판단에 개수가 필요.
            List<StepRow> rows = new List<StepRow>();

            foreach (object row in source)
            {
                StepRow parsed = new StepRow();
                parsed.Label = ToText(MemberPathReader.Read(row, this.LabelMemberPath));
                parsed.State = ParseState(ToText(MemberPathReader.Read(row, this.StateMemberPath)));
                rows.Add(parsed);
            }

            for (int index = 0; index < rows.Count; index++)
            {
                this.stepModels.Add(this.BuildStepItem(rows[index], index, rows.Count));
            }
        }

        // 단계 하나의 표시 모델을 상태와 위치(첫/마지막)로부터 만든다.
        private StepIndicatorItem BuildStepItem(StepRow row, int index, int count)
        {
            Brush accent = this.Brush("Brush.Accent");
            Brush onAccent = this.Brush("Brush.OnAccent");
            Brush surface = this.Brush("Brush.Surface");
            Brush border = this.Brush("Brush.Border");
            Brush textPrimary = this.Brush("Brush.TextPrimary");
            Brush textSecondary = this.Brush("Brush.TextSecondary");
            Brush connectorOff = this.Brush("Brush.BorderSubtle");
            Brush error = this.Brush("Brush.ErrorBorder");
            Brush errorText = this.Brush("Brush.ErrorText");

            FontFamily bodyFont = this.FontFamily;
            FontFamily mdl2 = new FontFamily("Segoe MDL2 Assets");

            StepIndicatorItem item = new StepIndicatorItem();
            item.Label = row.Label;
            item.LabelWeight = FontWeights.Normal;

            // 진행이 이 노드에 도달했는가(왼쪽) / 통과했는가(오른쪽).
            bool reached = row.State == ModernStepState.Completed
                || row.State == ModernStepState.Current
                || row.State == ModernStepState.Failed;
            bool passed = row.State == ModernStepState.Completed;

            item.LeftConnectorBrush = reached ? accent : connectorOff;
            item.RightConnectorBrush = passed ? accent : connectorOff;
            item.LeftConnectorVisibility = index > 0 ? Visibility.Visible : Visibility.Hidden;
            item.RightConnectorVisibility = index < count - 1 ? Visibility.Visible : Visibility.Hidden;

            switch (row.State)
            {
                case ModernStepState.Completed:
                    item.NodeBackground = accent;
                    item.NodeBorderBrush = accent;
                    item.NodeForeground = onAccent;
                    item.Glyph = GlyphCompleted;
                    item.GlyphFontFamily = mdl2;
                    item.LabelForeground = textPrimary;
                    break;

                case ModernStepState.Current:
                    item.NodeBackground = surface;
                    item.NodeBorderBrush = accent;
                    item.NodeForeground = accent;
                    item.Glyph = GlyphCurrent;
                    item.GlyphFontFamily = bodyFont;
                    item.LabelForeground = textPrimary;
                    item.LabelWeight = FontWeights.SemiBold;
                    break;

                case ModernStepState.Failed:
                    item.NodeBackground = error;
                    item.NodeBorderBrush = error;
                    item.NodeForeground = onAccent;
                    item.Glyph = GlyphFailed;
                    item.GlyphFontFamily = mdl2;
                    item.LabelForeground = errorText;
                    item.LabelWeight = FontWeights.SemiBold;
                    break;

                default:
                    // Pending
                    item.NodeBackground = surface;
                    item.NodeBorderBrush = border;
                    item.NodeForeground = textSecondary;
                    item.Glyph = (index + 1).ToString();
                    item.GlyphFontFamily = bodyFont;
                    item.LabelForeground = textSecondary;
                    break;
            }

            return item;
        }

        private Brush Brush(string tokenKey)
        {
            return (Brush)this.FindResource(tokenKey);
        }

        private static ModernStepState ParseState(string text)
        {
            ModernStepState state;

            if (Enum.TryParse(text, true, out state))
            {
                return state;
            }

            return ModernStepState.Pending;
        }

        private static string ToText(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        // 연결선 판단을 위해 개수를 먼저 알아야 하므로 임시로 모으는 경량 구조체.
        private struct StepRow
        {
            public string Label;
            public ModernStepState State;
        }
    }
}
