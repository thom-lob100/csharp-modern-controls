using System;
using System.Collections;
using System.Data;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// 계약이 허용하는 WinForms식 DataSource 형태
    /// (DataTable / DataView / IList / IEnumerable)를 WPF ItemsSource로 변환한다.
    /// 허용 형태가 동일하게 유지되도록 모든 데이터 바인딩 래퍼가 공유한다.
    /// </summary>
    internal static class DataSourceConverter
    {
        /// <summary>null 입력에는 null을 반환하고, 지원하지 않는 형태에는 예외를 던진다.</summary>
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
