using System.Windows;

namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// 그리드 컬럼 필터의 XAML 연동용 첨부 속성 — 코드(GridFilterController)가
    /// DataGridColumn에 설정하고, 헤더 템플릿의 트리거가 읽어 깔때기 버튼의
    /// 표시 여부(Filterable)와 활성 색(IsActive — 필터가 걸린 컬럼)을 결정한다.
    /// 깔때기는 헤더 셀 **맨 오른쪽에 고정**되고 정렬 글리프가 그 바로 왼쪽에
    /// 온다 — 버튼(액션) 위치는 항상 일정하고, 정렬 표시가 나타나도 밀리지
    /// 않는다.
    /// </summary>
    public static class GridFilterAttach
    {
        /// <summary>이 컬럼 헤더에 값 필터 깔때기 버튼을 표시할지 여부.</summary>
        public static readonly DependencyProperty FilterableProperty =
            DependencyProperty.RegisterAttached(
                "Filterable",
                typeof(bool),
                typeof(GridFilterAttach),
                new PropertyMetadata(false));

        /// <summary>Filterable 첨부 속성 값을 읽는다.</summary>
        public static bool GetFilterable(DependencyObject element)
        {
            return (bool)element.GetValue(FilterableProperty);
        }

        /// <summary>Filterable 첨부 속성 값을 설정한다.</summary>
        public static void SetFilterable(DependencyObject element, bool value)
        {
            element.SetValue(FilterableProperty, value);
        }

        /// <summary>이 컬럼에 필터가 걸려 있는지 여부 (깔때기 액센트색 표시).</summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached(
                "IsActive",
                typeof(bool),
                typeof(GridFilterAttach),
                new PropertyMetadata(false));

        /// <summary>IsActive 첨부 속성 값을 읽는다.</summary>
        public static bool GetIsActive(DependencyObject element)
        {
            return (bool)element.GetValue(IsActiveProperty);
        }

        /// <summary>IsActive 첨부 속성 값을 설정한다.</summary>
        public static void SetIsActive(DependencyObject element, bool value)
        {
            element.SetValue(IsActiveProperty, value);
        }
    }
}
