using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Modern.Lab.Controls.Wpf.Common;

namespace Modern.Lab.Controls.Wpf.Data
{
    // ===== ModernDataGridControl의 배지/버튼 셀이 쓰는 값 변환기 모음 =====
    // (GridColumnFactory가 생성하는 CellTemplate 바인딩 전용 — 외부 공개 아님.)

    /// <summary>색 문자열("#FEE2E2") → 배경 브러시. 해석 불가면 투명(일반 텍스트처럼 보임).</summary>
    internal sealed class BadgeBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color;

            if (ChipColorHelper.TryParseColor(value == null ? null : value.ToString(), out color))
            {
                SolidColorBrush brush = new SolidColorBrush(color);
                brush.Freeze();
                return brush;
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>색 문자열 → 배경에서 유도한 글자색 브러시. 해석 불가면 기본 글자색 상속.</summary>
    internal sealed class BadgeForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color;

            if (ChipColorHelper.TryParseColor(value == null ? null : value.ToString(), out color))
            {
                SolidColorBrush brush = new SolidColorBrush(ChipColorHelper.DeriveForeground(color));
                brush.Freeze();
                return brush;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 콤보 셀 배지 표시용 — 선택 값(항목 문자열) → 항목별 지정 색 브러시.
    /// GridColumnFactory가 컬럼의 (ComboItems, ComboItemColors) 쌍으로 만들어
    /// 배경(foreground=false)과 글자색(foreground=true) 두 인스턴스를 쓴다.
    /// 목록에 없는 값(미선택 포함)은 UnsetValue — 바인딩의 FallbackValue나
    /// 기본값(배경 없음 / 글자색 상속)으로 떨어진다.
    /// </summary>
    internal sealed class ComboItemBadgeConverter : IValueConverter
    {
        private readonly System.Collections.Generic.Dictionary<string, Brush> brushes;

        internal ComboItemBadgeConverter(string[] items, string[] colors, bool foreground)
        {
            this.brushes = new System.Collections.Generic.Dictionary<string, Brush>(
                    StringComparer.OrdinalIgnoreCase);

            int count = items != null && colors != null
                    ? Math.Min(items.Length, colors.Length)
                    : 0;

            for (int index = 0; index < count; index++)
            {
                Color color;

                if (items[index] != null && ChipColorHelper.TryParseColor(colors[index], out color))
                {
                    SolidColorBrush brush = new SolidColorBrush(
                            foreground ? ChipColorHelper.DeriveForeground(color) : color);
                    brush.Freeze();
                    this.brushes[items[index]] = brush;
                }
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush;

            if (value != null && this.brushes.TryGetValue(value.ToString(), out brush))
            {
                return brush;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>bool 또는 "Y"/"YES"/"TRUE"/"1" 계열 문자열을 참으로 해석한다.</summary>
    internal sealed class TruthyToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value;
            }

            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            string text = value.ToString().Trim();

            return text.Equals("Y", StringComparison.OrdinalIgnoreCase)
                || text.Equals("YES", StringComparison.OrdinalIgnoreCase)
                || text.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                || text == "1";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
