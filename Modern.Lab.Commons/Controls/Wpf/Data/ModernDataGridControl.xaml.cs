using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// 모던 읽기 전용 데이터 그리드.
    /// - ItemsSource: 임의의 IEnumerable (DataView, IList, ...)
    /// - SelectedItem: 현재 행 (양방향)
    /// - AutoGenerateColumns: 기본값 true; ApplyColumns 호출 시 명시적 컬럼으로 전환
    /// - SelectionChanged: 행 선택이 바뀔 때 발생
    /// </summary>
    public partial class ModernDataGridControl : UserControl
    {
        /// <summary>표시할 행 목록. 임의의 IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernDataGridControl),
                new PropertyMetadata(null));

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

        /// <summary>행 선택이 바뀔 때 발생한다.</summary>
        public event EventHandler SelectionChanged;

        public ModernDataGridControl()
        {
            this.InitializeComponent();
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

        /// <summary>현재 표시 중인 행 수.</summary>
        public int RowCount
        {
            get { return this.InnerDataGrid.Items.Count; }
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

            if (columns == null)
            {
                return;
            }

            foreach (ModernDataGridColumn definition in columns)
            {
                this.InnerDataGrid.Columns.Add(CreateColumn(definition));
            }
        }

        private static DataGridTextColumn CreateColumn(ModernDataGridColumn definition)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = definition.HeaderText;
            column.Binding = new Binding(definition.DataPropertyName);

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

        private void InnerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}
