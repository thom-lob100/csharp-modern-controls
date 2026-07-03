namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// Column definition consumed by ModernDataGrid / ModernDataGridControl.
    /// A plain data holder so WinForms forms can define columns without touching
    /// WPF DataGrid types directly.
    /// </summary>
    public class ModernDataGridColumn
    {
        /// <summary>Creates an empty definition (star width, left aligned).</summary>
        public ModernDataGridColumn()
        {
            this.DataPropertyName = string.Empty;
            this.HeaderText = string.Empty;
            this.Width = -1d;
            this.TextAlignment = GridTextAlignment.Left;
        }

        /// <summary>Creates a star-width column bound to the given column/property.</summary>
        public ModernDataGridColumn(string dataPropertyName, string headerText)
            : this()
        {
            this.DataPropertyName = dataPropertyName;
            this.HeaderText = headerText;
        }

        /// <summary>Creates a fixed-width column bound to the given column/property.</summary>
        public ModernDataGridColumn(string dataPropertyName, string headerText, double width)
            : this(dataPropertyName, headerText)
        {
            this.Width = width;
        }

        /// <summary>Source column/property name (DataTable column or object property).</summary>
        public string DataPropertyName { get; set; }

        /// <summary>Header caption.</summary>
        public string HeaderText { get; set; }

        /// <summary>Pixel width. Zero or negative means star sizing (fill remaining space).</summary>
        public double Width { get; set; }

        /// <summary>Horizontal alignment of the cell text.</summary>
        public GridTextAlignment TextAlignment { get; set; }
    }
}
