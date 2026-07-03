using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// Modern read-only data grid.
    /// - ItemsSource: any IEnumerable (DataView, IList, ...)
    /// - SelectedItem: current row (two-way)
    /// - AutoGenerateColumns: true by default; ApplyColumns switches to explicit columns
    /// - SelectionChanged: raised when the row selection changes
    /// </summary>
    public partial class ModernDataGridControl : UserControl
    {
        /// <summary>Rows to display. Any IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernDataGridControl),
                new PropertyMetadata(null));

        /// <summary>Currently selected row item. Two-way by default.</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Whether columns are generated from the data source.</summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(
                "AutoGenerateColumns",
                typeof(bool),
                typeof(ModernDataGridControl),
                new PropertyMetadata(true));

        /// <summary>Raised when the row selection changes.</summary>
        public event EventHandler SelectionChanged;

        public ModernDataGridControl()
        {
            this.InitializeComponent();
        }

        /// <summary>Rows to display.</summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>Currently selected row item.</summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>Whether columns are generated from the data source.</summary>
        public bool AutoGenerateColumns
        {
            get { return (bool)this.GetValue(AutoGenerateColumnsProperty); }
            set { this.SetValue(AutoGenerateColumnsProperty, value); }
        }

        /// <summary>Number of rows currently shown.</summary>
        public int RowCount
        {
            get { return this.InnerDataGrid.Items.Count; }
        }

        /// <summary>Index of the selected row (-1 when nothing is selected).</summary>
        public int SelectedIndex
        {
            get { return this.InnerDataGrid.SelectedIndex; }
            set { this.InnerDataGrid.SelectedIndex = value; }
        }

        /// <summary>
        /// Replaces the columns with explicit definitions and turns off
        /// auto-generation. Passing null or an empty list just clears the columns.
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
