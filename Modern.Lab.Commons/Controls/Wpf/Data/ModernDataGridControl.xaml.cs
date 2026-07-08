using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Modern.Lab.Controls.Wpf.Common;

namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// 모던 읽기 전용 데이터 그리드.
    /// - ItemsSource: 임의의 IEnumerable (DataView, IList, ...)
    /// - SelectedItem: 현재 행 (양방향)
    /// - AutoGenerateColumns: 기본값 true; ApplyColumns 호출 시 명시적 컬럼으로 전환
    /// - SelectionChanged: 행 선택이 바뀔 때 발생
    /// - ShowStatusBar: 그리드 하단 상태바 표시 여부 (행 수 자동 표기 + StatusText)
    /// </summary>
    public partial class ModernDataGridControl : UserControl
    {
        /// <summary>표시할 행 목록. 임의의 IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernDataGridControl),
                new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// 컬럼 너비를 헤더 캡션과 데이터 내용 중 더 넓은 쪽에 맞춰 자동 계산할지
        /// 여부. ApplyColumns로 정의된 컬럼에만 적용되며, 켜져 있으면 데이터가
        /// 바뀔 때마다 각 컬럼이 잘림 없이 표시되는 최소 너비로 재계산된다
        /// (정의의 Width는 무시). 스크롤 중 너비가 흔들리는 WPF Auto 크기 대신
        /// 전체 데이터를 미리 측정해 고정 픽셀 너비를 넣는다.
        /// </summary>
        public static readonly DependencyProperty AutoFitColumnsProperty =
            DependencyProperty.Register(
                "AutoFitColumns",
                typeof(bool),
                typeof(ModernDataGridControl),
                new PropertyMetadata(false, OnAutoFitColumnsChanged));

        /// <summary>현재 선택된 행 항목. 기본적으로 양방향 바인딩.</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>데이터 소스로부터 컬럼을 자동 생성할지 여부.</summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(
                "AutoGenerateColumns",
                typeof(bool),
                typeof(ModernDataGridControl),
                new PropertyMetadata(true));

        /// <summary>그리드 하단 상태바 표시 여부.</summary>
        public static readonly DependencyProperty ShowStatusBarProperty =
            DependencyProperty.Register(
                "ShowStatusBar",
                typeof(bool),
                typeof(ModernDataGridControl),
                new PropertyMetadata(false, OnStatusBarAppearanceChanged));

        /// <summary>상태바 오른쪽에 표시할 자유 텍스트 (빈 문자열이면 표시 없음).</summary>
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register(
                "StatusText",
                typeof(string),
                typeof(ModernDataGridControl),
                new PropertyMetadata(string.Empty, OnStatusBarAppearanceChanged));

        /// <summary>상태바 왼쪽 행 수 표기 형식. {0}에 현재 행 수가 들어간다.</summary>
        public static readonly DependencyProperty StatusCountFormatProperty =
            DependencyProperty.Register(
                "StatusCountFormat",
                typeof(string),
                typeof(ModernDataGridControl),
                new PropertyMetadata("{0:N0} rows", OnStatusBarAppearanceChanged));

        /// <summary>
        /// 행 배경색으로 쓸 컬럼/속성 이름 (선택 사항). 값은 "#FEE2E2" 같은 색
        /// 문자열 또는 색 이름. 비어 있거나 해석 불가한 행은 기본 배경(교차색)을
        /// 유지한다. 상태(Scrap 등)에 따라 행을 색으로 구분할 때 쓴다.
        /// </summary>
        public static readonly DependencyProperty RowColorMemberPathProperty =
            DependencyProperty.Register(
                "RowColorMemberPath",
                typeof(string),
                typeof(ModernDataGridControl),
                new PropertyMetadata(string.Empty, OnRowColorMemberPathChanged));

        /// <summary>행 선택이 바뀔 때 발생한다.</summary>
        public event EventHandler SelectionChanged;

        /// <summary>높이 변화로 표시 가능 행 수(VisibleRowCapacity)가 바뀔 때 발생한다.</summary>
        public event EventHandler VisibleRowCapacityChanged;

        // 마지막으로 통지한 표시 가능 행 수 — 같은 값이면 이벤트를 삼킨다.
        private int lastCapacity;

        // ApplyColumns로 받은 컬럼 정의 — AutoFitColumns 재계산의 원천.
        private IList<ModernDataGridColumn> columnDefinitions;

        // ===== AutoFitColumns 측정 상수 =====
        // 컬럼당 실제 픽셀 측정할 후보 문자열 수 — 글자 수 상위 후보만 측정해
        // 큰 데이터에서도 비용을 일정하게 유지한다.
        private const int autoFitCandidateCount = 8;
        // 자동 맞춤 너비의 하한/상한 (지나치게 좁은 컬럼과 폭주 컬럼 방지).
        private const double autoFitMinWidth = 48d;
        private const double autoFitMaxWidth = 600d;
        // 셀 좌우 패딩(Pad.Field 12,0) + 우측 구분선/렌더링 오차 여유.
        private const double autoFitCellPadding = 28d;
        // 헤더 전용 추가 여유: 오른쪽 고정 정렬 글리프(여백 6 + 글리프 폭).
        private const double autoFitSortGlyphReserve = 18d;

        public ModernDataGridControl()
        {
            this.InitializeComponent();
            this.SizeChanged += this.OnControlSizeChanged;

            // 행 수 자동 갱신: 소스 교체/필터 변경 등으로 Items가 바뀔 때마다
            // 상태바의 카운트 텍스트를 다시 쓴다.
            ((INotifyCollectionChanged)this.InnerDataGrid.Items).CollectionChanged += this.OnItemsCollectionChanged;

            // 행 색상: 행이 화면에 실체화될 때마다 RowColorMember 값으로 배경을 칠한다.
            this.InnerDataGrid.LoadingRow += this.OnLoadingRow;
        }

        /// <summary>표시할 행 목록.</summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>현재 선택된 행 항목.</summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>데이터 소스로부터 컬럼을 자동 생성할지 여부.</summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)this.GetValue(AutoGenerateColumnsProperty); }
            set { this.SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>컬럼 너비를 헤더와 데이터 내용에 맞춰 자동 계산할지 여부.</summary>
        public bool AutoFitColumns
        {
            get { return (bool)this.GetValue(AutoFitColumnsProperty); }
            set { this.SetValue(AutoFitColumnsProperty, value); }
        }

        /// <summary>그리드 하단 상태바 표시 여부.</summary>
        public bool ShowStatusBar
        {
            get { return (bool)this.GetValue(ShowStatusBarProperty); }
            set { this.SetValue(ShowStatusBarProperty, value); }
        }

        /// <summary>상태바 오른쪽에 표시할 자유 텍스트.</summary>
        public string StatusText
        {
            get { return (string)this.GetValue(StatusTextProperty); }
            set { this.SetValue(StatusTextProperty, value); }
        }

        /// <summary>상태바 왼쪽 행 수 표기 형식 ({0} = 행 수).</summary>
        public string StatusCountFormat
        {
            get { return (string)this.GetValue(StatusCountFormatProperty); }
            set { this.SetValue(StatusCountFormatProperty, value); }
        }

        /// <summary>행 배경색으로 쓸 컬럼/속성 이름 (선택 사항).</summary>
        public string RowColorMemberPath
        {
            get { return (string)this.GetValue(RowColorMemberPathProperty); }
            set { this.SetValue(RowColorMemberPathProperty, value); }
        }

        /// <summary>현재 표시 중인 행 수.</summary>
        public int RowCount
        {
            get { return this.InnerDataGrid.Items.Count; }
        }

        /// <summary>
        /// 현재 높이에서 세로 스크롤 없이 표시 가능한 행 수 (최소 1).
        /// 행/헤더 높이가 토큰으로 고정되어 있어 결정적으로 계산된다.
        /// 페이지 크기를 화면 높이에 맞추는 용도(ModernPagination.PageSize 연동).
        /// </summary>
        public int VisibleRowCapacity
        {
            get
            {
                double rowHeight = (double)this.FindResource("Size.RowHeight");
                double headerHeight = (double)this.FindResource("Size.GridHeaderHeight");

                // 바깥 카드 테두리 위아래 1px씩 제외
                double available = this.ActualHeight - headerHeight - 2.0;

                // 상태바가 보이면 그 높이만큼 행 표시 영역이 줄어든다.
                if (this.StatusBarStrip.Visibility == Visibility.Visible)
                {
                    available = available - this.StatusBarStrip.ActualHeight;
                }

                if (available < rowHeight)
                {
                    return 1;
                }

                return (int)(available / rowHeight);
            }
        }

        private void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            int capacity = this.VisibleRowCapacity;

            if (capacity == this.lastCapacity)
            {
                return;
            }

            this.lastCapacity = capacity;

            if (this.VisibleRowCapacityChanged != null)
            {
                this.VisibleRowCapacityChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>선택된 행의 인덱스(아무것도 선택되지 않았으면 -1).</summary>
        public int SelectedIndex
        {
            get { return this.InnerDataGrid.SelectedIndex; }
            set { this.InnerDataGrid.SelectedIndex = value; }
        }

        /// <summary>
        /// 컬럼을 명시적 정의로 교체하고 자동 생성을 끈다.
        /// null이나 빈 목록을 넘기면 컬럼만 비운다.
        /// </summary>
        public void ApplyColumns(IList<ModernDataGridColumn> columns)
        {
            this.AutoGenerateColumns = false;
            this.InnerDataGrid.Columns.Clear();
            this.columnDefinitions = null;

            if (columns == null)
            {
                return;
            }

            this.columnDefinitions = new List<ModernDataGridColumn>(columns);

            foreach (ModernDataGridColumn definition in columns)
            {
                this.InnerDataGrid.Columns.Add(CreateColumn(definition));
            }

            // 데이터가 이미 할당돼 있으면(할당 순서 내성) 즉시 자동 맞춤을 적용한다.
            this.AutoFitColumnWidths();
        }

        private static DataGridTextColumn CreateColumn(ModernDataGridColumn definition)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = definition.HeaderText;

            Binding binding = new Binding(definition.DataPropertyName);

            // 표시 형식: "N0" 같은 단순 형식은 "{0:N0}"으로 해석된다(WPF 규칙).
            // 원본이 타입 컬럼(int/decimal/DateTime)일 때만 효과가 있다.
            if (!string.IsNullOrEmpty(definition.Format))
            {
                binding.StringFormat = definition.Format;
            }

            column.Binding = binding;

            if (definition.Width > 0d)
            {
                column.Width = new DataGridLength(definition.Width);
            }
            else
            {
                column.Width = new DataGridLength(1d, DataGridLengthUnitType.Star);
            }

            if (definition.TextAlignment != GridTextAlignment.Left)
            {
                TextAlignment alignment = definition.TextAlignment == GridTextAlignment.Center
                    ? TextAlignment.Center
                    : TextAlignment.Right;

                Style elementStyle = new Style(typeof(TextBlock));
                elementStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, alignment));
                column.ElementStyle = elementStyle;
            }

            return column;
        }

        private static void OnStatusBarAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernDataGridControl)d).RefreshStatusBar();
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernDataGridControl)d).AutoFitColumnWidths();
        }

        private static void OnAutoFitColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernDataGridControl)d).AutoFitColumnWidths();
        }

        // ===== 컬럼 자동 맞춤 (헤더 + 데이터 최대 폭) =====

        // 각 컬럼 너비를 max(헤더 캡션 폭 + 글리프 여유, 데이터 최대 폭)으로
        // 재계산해 고정 픽셀 너비로 넣는다. WPF DataGridLength.Auto는 가상화로
        // 실체화된 행만 보고 계산해 스크롤 중 너비가 계속 변하므로 쓰지 않는다.
        private void AutoFitColumnWidths()
        {
            if (!this.AutoFitColumns || this.columnDefinitions == null)
            {
                return;
            }

            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            Typeface headerTypeface = new Typeface(this.FontFamily, FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal);
            Typeface bodyTypeface = new Typeface(this.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            double headerFontSize = (double)this.FindResource("Font.Size.Label");
            double bodyFontSize = (double)this.FindResource("Font.Size.Body");

            int count = Math.Min(this.columnDefinitions.Count, this.InnerDataGrid.Columns.Count);

            for (int index = 0; index < count; index++)
            {
                ModernDataGridColumn definition = this.columnDefinitions[index];

                double width = MeasureText(definition.HeaderText, headerTypeface, headerFontSize, pixelsPerDip)
                    + autoFitCellPadding + autoFitSortGlyphReserve;

                foreach (string candidate in CollectLongestCellTexts(this.ItemsSource, definition))
                {
                    double cellWidth = MeasureText(candidate, bodyTypeface, bodyFontSize, pixelsPerDip)
                        + autoFitCellPadding;

                    if (cellWidth > width)
                    {
                        width = cellWidth;
                    }
                }

                if (width < autoFitMinWidth)
                {
                    width = autoFitMinWidth;
                }

                if (width > autoFitMaxWidth)
                {
                    width = autoFitMaxWidth;
                }

                this.InnerDataGrid.Columns[index].Width = new DataGridLength(Math.Ceiling(width));
            }
        }

        // 한 컬럼의 셀 텍스트 중 "글자 수 상위" 후보만 모은다. 픽셀 폭은 글자 수와
        // 단조 일치하지 않지만(글자별 폭 차이), 상위 여러 개를 함께 측정하면
        // 실용적으로 안전하다 — 전체 행을 모두 픽셀 측정하는 비용을 피한다.
        private static List<string> CollectLongestCellTexts(IEnumerable source, ModernDataGridColumn definition)
        {
            List<string> candidates = new List<string>();

            if (source == null)
            {
                return candidates;
            }

            foreach (object row in source)
            {
                string text = FormatCellText(MemberPathReader.Read(row, definition.DataPropertyName), definition.Format);

                if (text.Length == 0 || candidates.Contains(text))
                {
                    continue;
                }

                if (candidates.Count < autoFitCandidateCount)
                {
                    candidates.Add(text);
                    continue;
                }

                // 가장 짧은 후보를 찾아 더 긴 텍스트로 교체한다.
                int shortestIndex = 0;

                for (int index = 1; index < candidates.Count; index++)
                {
                    if (candidates[index].Length < candidates[shortestIndex].Length)
                    {
                        shortestIndex = index;
                    }
                }

                if (text.Length > candidates[shortestIndex].Length)
                {
                    candidates[shortestIndex] = text;
                }
            }

            return candidates;
        }

        // 셀에 표시될 문자열을 만든다 — 컬럼 Format이 있으면 바인딩 StringFormat과
        // 같은 규칙("{0:형식}")으로 적용하고, 실패하면 기본 문자열 표현을 쓴다.
        private static string FormatCellText(object value, string format)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(format))
            {
                try
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0:" + format + "}", value);
                }
                catch (FormatException)
                {
                    // 형식 오류는 기본 표현으로 폴백한다 (바인딩 표시와 동일한 완화).
                }
            }

            return value.ToString();
        }

        private static double MeasureText(string text, Typeface typeface, double fontSize, double pixelsPerDip)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0d;
            }

            FormattedText formatted = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.Black,
                pixelsPerDip);

            return formatted.WidthIncludingTrailingWhitespace;
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshStatusBar();
        }

        private static void OnRowColorMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModernDataGridControl control = (ModernDataGridControl)d;

            // 이미 실체화된 행에 새 경로를 반영하려면 다시 실체화시켜 LoadingRow를 재발생시킨다.
            if (control.InnerDataGrid.Items.Count > 0)
            {
                control.InnerDataGrid.Items.Refresh();
            }
        }

        // 각 행이 화면에 실체화될 때 배경을 칠한다. RowColorMember 값이 유효한 색이면
        // 그 색으로, 아니면 로컬 배경을 지워 그리드 기본 교차색을 유지한다.
        // (경로가 비어 있으면 기존 동작 그대로 — 배경을 건드리지 않는다.)
        private void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            string path = this.RowColorMemberPath;

            if (string.IsNullOrEmpty(path))
            {
                e.Row.ClearValue(Control.BackgroundProperty);
                return;
            }

            Color color;

            if (ChipColorHelper.TryParseColor(ToText(MemberPathReader.Read(e.Row.Item, path)), out color))
            {
                SolidColorBrush brush = new SolidColorBrush(color);
                brush.Freeze();
                e.Row.Background = brush;
            }
            else
            {
                e.Row.ClearValue(Control.BackgroundProperty);
            }
        }

        private static string ToText(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        // 상태바 표시/텍스트를 현재 속성과 행 수로 다시 그린다.
        // 형식 문자열 오류는 형식 그대로 출력하는 것으로 완화한다(예외 없음).
        private void RefreshStatusBar()
        {
            this.StatusBarStrip.Visibility = this.ShowStatusBar ? Visibility.Visible : Visibility.Collapsed;

            if (!this.ShowStatusBar)
            {
                return;
            }

            string format = this.StatusCountFormat;
            string countText;

            try
            {
                countText = string.Format(format ?? string.Empty, this.InnerDataGrid.Items.Count);
            }
            catch (FormatException)
            {
                countText = format;
            }

            this.StatusCountText.Text = countText;
            this.StatusRightText.Text = this.StatusText ?? string.Empty;
        }

        private void InnerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}
