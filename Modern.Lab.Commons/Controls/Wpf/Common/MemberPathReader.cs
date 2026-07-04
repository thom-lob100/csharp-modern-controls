using System.ComponentModel;
using System.Data;

namespace Modern.Lab.Controls.Wpf.Common
{
    /// <summary>
    /// Reads a named column/property from a bound row item, mirroring how
    /// WinForms DisplayMember/ValueMember resolve values. Shared by controls
    /// that need display text outside of a WPF binding (filtering, chips).
    /// </summary>
    internal static class MemberPathReader
    {
        /// <summary>Returns the member value, or null when it cannot be resolved.</summary>
        internal static object Read(object row, string memberPath)
        {
            if (row == null || string.IsNullOrEmpty(memberPath))
            {
                return null;
            }

            DataRowView rowView = row as DataRowView;

            if (rowView != null)
            {
                if (rowView.Row.Table.Columns.Contains(memberPath))
                {
                    return rowView[memberPath];
                }

                return null;
            }

            PropertyDescriptor property = TypeDescriptor.GetProperties(row).Find(memberPath, true);

            if (property != null)
            {
                return property.GetValue(row);
            }

            return null;
        }

        /// <summary>
        /// Returns the member value as display text. Falls back to the item's
        /// own ToString() when no member path is set; never returns null.
        /// </summary>
        internal static string ReadDisplayText(object row, string memberPath)
        {
            if (row == null)
            {
                return string.Empty;
            }

            object value = string.IsNullOrEmpty(memberPath) ? row : Read(row, memberPath);

            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
