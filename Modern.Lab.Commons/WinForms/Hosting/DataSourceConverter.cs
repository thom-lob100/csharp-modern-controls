using System;
using System.Collections;
using System.Data;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// Converts the WinForms-style DataSource shapes accepted by the contract
    /// (DataTable / DataView / IList / IEnumerable) into a WPF ItemsSource.
    /// Shared by every data-bound wrapper so the accepted shapes stay identical.
    /// </summary>
    internal static class DataSourceConverter
    {
        /// <summary>Returns null for null input; throws for unsupported shapes.</summary>
        internal static IEnumerable ToItemsSource(object value)
        {
            if (value == null)
            {
                return null;
            }

            DataTable table = value as DataTable;

            if (table != null)
            {
                return table.DefaultView;
            }

            DataView view = value as DataView;

            if (view != null)
            {
                return view;
            }

            IEnumerable enumerable = value as IEnumerable;

            if (enumerable != null)
            {
                return enumerable;
            }

            throw new ArgumentException(
                "DataSource must be a DataTable, DataView, IList or IEnumerable.",
                "value");
        }
    }
}
