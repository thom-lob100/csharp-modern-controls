using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Modern.Lab.Controls.Wpf.Common;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 줄바꿈되는 칩 목록으로 렌더링되는 분류/건수 요약
    /// (예: 부서별 또는 직급별 인원 수).
    /// - ItemsSource: 임의의 IEnumerable (DataView, IList, ...)
    /// - LabelMemberPath / CountMemberPath: 읽어올 컬럼/속성 이름
    /// - Title: 칩 위에 표시되는 선택적 캡션
    ///
    /// 칩 목록은 소스나 멤버 경로가 바뀔 때마다 다시 만들어지므로
    /// 할당 순서는 상관없다(계약 규칙 3).
    /// </summary>
    public partial class ModernSummaryListControl : UserControl
    {
        /// <summary>요약할 행 목록. 임의의 IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(null, OnDataShapeChanged));

        /// <summary>칩 레이블로 쓸 컬럼/속성 이름.</summary>
        public static readonly DependencyProperty LabelMemberPathProperty =
            DependencyProperty.Register(
                "LabelMemberPath",
                typeof(string),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(string.Empty, OnDataShapeChanged));

        /// <summary>칩 건수로 쓸 컬럼/속성 이름.</summary>
        public static readonly DependencyProperty CountMemberPathProperty =
            DependencyProperty.Register(
                "CountMemberPath",
                typeof(string),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(string.Empty, OnDataShapeChanged));

        /// <summary>
        /// 칩 배경색으로 쓸 컬럼/속성 이름. 값은 "#DBEAFE" 같은 hex 또는
        /// "SkyBlue" 같은 색 이름 문자열. 비어 있거나 파싱 불가면 기본 토큰 색으로 폴백.
        /// </summary>
        public static readonly DependencyProperty ColorMemberPathProperty =
            DependencyProperty.Register(
                "ColorMemberPath",
                typeof(string),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(string.Empty, OnDataShapeChanged));

        /// <summary>칩 왼쪽의 선택적 캡션. 비어 있으면 캡션을 숨긴다.</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(string.Empty));

        /// <summary>공용 카드 패널 위에서 쓰도록 카드 크롬(테두리/배경/패딩)을 제거한다.</summary>
        public static readonly DependencyProperty FlatProperty =
            DependencyProperty.Register(
                "Flat",
                typeof(bool),
                typeof(ModernSummaryListControl),
                new PropertyMetadata(false));

        private readonly ObservableCollection<SummaryItem> chipItemModels;

        public ModernSummaryListControl()
        {
            this.chipItemModels = new ObservableCollection<SummaryItem>();
            this.InitializeComponent();
            this.ChipItems.ItemsSource = this.chipItemModels;
        }

        /// <summary>요약할 행 목록.</summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>칩 레이블로 쓸 컬럼/속성 이름.</summary>
        public string LabelMemberPath
        {
            get { return (string)this.GetValue(LabelMemberPathProperty); }
            set { this.SetValue(LabelMemberPathProperty, value); }
        }

        /// <summary>칩 건수로 쓸 컬럼/속성 이름.</summary>
        public string CountMemberPath
        {
            get { return (string)this.GetValue(CountMemberPathProperty); }
            set { this.SetValue(CountMemberPathProperty, value); }
        }

        /// <summary>칩 배경색으로 쓸 컬럼/속성 이름.</summary>
        public string ColorMemberPath
        {
            get { return (string)this.GetValue(ColorMemberPathProperty); }
            set { this.SetValue(ColorMemberPathProperty, value); }
        }

        /// <summary>칩 왼쪽의 선택적 캡션.</summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>공용 카드 패널 위에서 쓰도록 카드 크롬을 제거한다.</summary>
        public bool Flat
        {
            get { return (bool)this.GetValue(FlatProperty); }
            set { this.SetValue(FlatProperty, value); }
        }

        private static void OnDataShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernSummaryListControl)d).RebuildChips();
        }

        // 소스 행을 칩 항목 모델로 변환한다. null/빈 소스와 존재하지 않는
        // 컬럼은 빈 출력으로 렌더링되며 절대 예외를 던지지 않는다(계약 규칙 3).
        private void RebuildChips()
        {
            this.chipItemModels.Clear();

            IEnumerable source = this.ItemsSource;

            if (source == null)
            {
                return;
            }

            foreach (object row in source)
            {
                SummaryItem item = new SummaryItem();
                item.Label = ToDisplayString(MemberPathReader.Read(row, this.LabelMemberPath));
                item.Count = ToDisplayString(MemberPathReader.Read(row, this.CountMemberPath));
                ApplyChipColor(item, MemberPathReader.Read(row, this.ColorMemberPath));
                this.chipItemModels.Add(item);
            }
        }

        // 행의 색 값(hex 또는 색 이름 문자열)을 칩 배경 브러시로 변환한다.
        // 빈 값/파싱 불가면 브러시를 null로 두어 XAML의 기본 토큰 색으로 폴백하고,
        // 어두운 배경이면 글자색을 흰색으로 바꿔 대비를 확보한다(계약 규칙 3: 예외 없음).
        private static void ApplyChipColor(SummaryItem item, object colorValue)
        {
            string colorText = ToDisplayString(colorValue).Trim();

            if (colorText.Length == 0)
            {
                return;
            }

            Color color;

            try
            {
                color = (Color)ColorConverter.ConvertFromString(colorText);
            }
            catch
            {
                return;
            }

            SolidColorBrush backgroundBrush = new SolidColorBrush(color);
            backgroundBrush.Freeze();
            item.Background = backgroundBrush;

            // 배경과 같은 색상 계열의 글자색을 자동 산출한다 (연파랑 배경 → 진파랑 글씨).
            SolidColorBrush foregroundBrush = new SolidColorBrush(DeriveChipForeground(color));
            foregroundBrush.Freeze();
            item.LabelForeground = foregroundBrush;
            item.CountForeground = foregroundBrush;
        }

        // 배경색에서 칩 글자색을 유도한다: 색상(Hue)은 유지하고 명도만 반대쪽으로.
        // - 밝은 배경 → 같은 색상의 진한 톤 (채도 보강 + 명도 0.30)
        // - 어두운 배경 → 같은 색상의 아주 밝은 톤 (명도 0.95)
        // - 무채색 배경 → 밝기에 따라 중립 진회색 또는 흰색
        private static Color DeriveChipForeground(Color background)
        {
            double hue;
            double saturation;
            double lightness;
            RgbToHsl(background, out hue, out saturation, out lightness);

            // 상대 휘도(0~1)로 밝은/어두운 배경을 판정한다 (HSL 명도보다 지각에 가깝다).
            double luminance = ((0.2126 * background.R) + (0.7152 * background.G) + (0.0722 * background.B)) / 255.0;

            if (saturation < 0.15)
            {
                if (luminance < 0.5)
                {
                    return Colors.White;
                }

                return Color.FromRgb(0x37, 0x41, 0x51);
            }

            if (luminance < 0.5)
            {
                return HslToRgb(hue, saturation, 0.95);
            }

            return HslToRgb(hue, Math.Max(saturation, 0.55), 0.30);
        }

        // RGB → HSL 변환. hue/saturation/lightness 모두 0~1 범위.
        private static void RgbToHsl(Color color, out double hue, out double saturation, out double lightness)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            lightness = (max + min) / 2.0;

            if (delta == 0.0)
            {
                hue = 0.0;
                saturation = 0.0;
                return;
            }

            if (lightness > 0.5)
            {
                saturation = delta / (2.0 - max - min);
            }
            else
            {
                saturation = delta / (max + min);
            }

            if (max == r)
            {
                hue = (((g - b) / delta) + (g < b ? 6.0 : 0.0)) / 6.0;
            }
            else if (max == g)
            {
                hue = (((b - r) / delta) + 2.0) / 6.0;
            }
            else
            {
                hue = (((r - g) / delta) + 4.0) / 6.0;
            }
        }

        // HSL → RGB 변환. hue/saturation/lightness 모두 0~1 범위.
        private static Color HslToRgb(double hue, double saturation, double lightness)
        {
            double r;
            double g;
            double b;

            if (saturation == 0.0)
            {
                r = lightness;
                g = lightness;
                b = lightness;
            }
            else
            {
                double q;

                if (lightness < 0.5)
                {
                    q = lightness * (1.0 + saturation);
                }
                else
                {
                    q = lightness + saturation - (lightness * saturation);
                }

                double p = (2.0 * lightness) - q;
                r = HueToRgbChannel(p, q, hue + (1.0 / 3.0));
                g = HueToRgbChannel(p, q, hue);
                b = HueToRgbChannel(p, q, hue - (1.0 / 3.0));
            }

            return Color.FromRgb(
                (byte)Math.Round(r * 255.0),
                (byte)Math.Round(g * 255.0),
                (byte)Math.Round(b * 255.0));
        }

        // HSL 보조: 색상 위치 t에 해당하는 채널 값(0~1)을 구한다.
        private static double HueToRgbChannel(double p, double q, double t)
        {
            if (t < 0.0)
            {
                t = t + 1.0;
            }

            if (t > 1.0)
            {
                t = t - 1.0;
            }

            if (t < 1.0 / 6.0)
            {
                return p + ((q - p) * 6.0 * t);
            }

            if (t < 1.0 / 2.0)
            {
                return q;
            }

            if (t < 2.0 / 3.0)
            {
                return p + ((q - p) * ((2.0 / 3.0) - t) * 6.0);
            }

            return p;
        }

        private static string ToDisplayString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
