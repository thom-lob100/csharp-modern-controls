using System;
using System.ComponentModel;
using System.Drawing;
using Modern.Lab.Controls.Wpf.Data;
using Modern.Lab.WinForms.Controls.Hosting;

namespace Modern.Lab.WinForms.Controls.Data
{
    /// <summary>
    /// Drop-in replacement for System.Windows.Forms.DataGridView (read-only list
    /// scenarios; WPF ModernDataGridControl hosted through ElementHost).
    ///
    /// Compatible members: DataSource (DataTable/DataView/IList/IEnumerable),
    /// AutoGenerateColumns, RowCount, SelectionChanged, Enabled.
    ///
    /// Contract behaviors (docs/design-notes.md section 6-1):
    /// - Assigning DataSource resets the selection, selects the first row when
    ///   data is present (matching DataGridView's initial current row), and
    ///   raises SelectionChanged exactly once per assignment.
    /// - Null/empty data renders as an empty grid, never throws.
    /// </summary>
    [ToolboxItem(true)]
    public class ModernDataGrid : WpfElementHostBase<ModernDataGridControl>
    {
        private object dataSource;
        private bool suppressSelectionChanged;

        // Fallback storage so the property grid still works when design-time
        // WPF construction failed (Wpf == null).
        private bool fallbackAutoGenerateColumns;

        /// <summary>Raised when the row selection changes (WinForms-compatible name).</summary>
        public event EventHandler SelectionChanged;

        /// <summary>Creates the control with a sensible default size.</summary>
        public ModernDataGrid()
        {
            this.Size = new Size(480, 240);
            this.fallbackAutoGenerateColumns = true;

            if (this.Wpf != null)
            {
                this.Wpf.SelectionChanged += this.OnWpfSelectionChanged;
            }
        }

        /// <summary>
        /// Data source: DataTable, DataView, IList or any IEnumerable. Assigning
        /// resets the selection, selects the first row when data is present, and
        /// raises SelectionChanged once. Assigning null clears the grid.
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

                if (this.Wpf == null)
                {
                    return;
                }

                this.suppressSelectionChanged = true;

                try
                {
                    this.Wpf.SelectedItem = null;
                    this.Wpf.ItemsSource = DataSourceConverter.ToItemsSource(value);

                    if (this.Wpf.RowCount > 0)
                    {
                        // Match DataGridView: the first row becomes current on bind.
                        this.Wpf.SelectedIndex = 0;
                    }
                }
                finally
                {
                    this.suppressSelectionChanged = false;
                }

                this.RaiseSelectionChanged();
            }
        }

        /// <summary>Whether columns are generated from the data source (WinForms-compatible).</summary>
        [Category("모던 컨트롤")]
        [Description("데이터 원본에서 컬럼을 자동 생성할지 여부")]
        [DefaultValue(true)]
        public bool AutoGenerateColumns
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.AutoGenerateColumns;
                }

                return this.fallbackAutoGenerateColumns;
            }
            set
            {
                this.fallbackAutoGenerateColumns = value;

                if (this.Wpf != null)
                {
                    this.Wpf.AutoGenerateColumns = value;
                }
            }
        }

        /// <summary>Number of rows currently shown (WinForms-compatible name).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowCount
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.RowCount;
                }

                return 0;
            }
        }

        /// <summary>Currently selected row item (DataRowView for DataTable sources).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            set
            {
                if (this.Wpf != null)
                {
                    this.Wpf.SelectedItem = value;
                }
            }
        }

        /// <summary>Index of the selected row (-1 when nothing is selected).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                if (this.Wpf != null)
                {
                    return this.Wpf.SelectedIndex;
                }

                return -1;
            }
            set
            {
                if (this.Wpf != null)
                {
                    this.Wpf.SelectedIndex = value;
                }
            }
        }

        /// <summary>
        /// Replaces the columns with explicit definitions and turns off
        /// auto-generation. Call before assigning DataSource.
        /// </summary>
        public void ConfigureColumns(params ModernDataGridColumn[] columns)
        {
            this.fallbackAutoGenerateColumns = false;

            if (this.Wpf != null)
            {
                this.Wpf.ApplyColumns(columns);
            }
        }

        private void OnWpfSelectionChanged(object sender, EventArgs e)
        {
            if (!this.suppressSelectionChanged)
            {
                this.RaiseSelectionChanged();
            }
        }

        private void RaiseSelectionChanged()
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}
