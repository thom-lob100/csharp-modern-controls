using System.ComponentModel;
using System.Data;

namespace Modern.Lab.Controls.Wpf.Common
{
    /// <summary>
    /// 바인딩된 행 항목에서 이름으로 지정된 컬럼/속성 값을 읽으며, WinForms의
    /// DisplayMember/ValueMember가 값을 해석하는 방식을 그대로 따른다. WPF 바인딩
    /// 밖에서 표시 텍스트가 필요한 컨트롤(필터링, 칩)이 공유한다.
    /// </summary>
    internal static class MemberPathReader
    {
        /// <summary>멤버 값을 반환하며, 해석할 수 없으면 null을 반환한다.</summary>
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
        /// 멤버 값을 표시 텍스트로 반환한다. 멤버 경로가 설정되지 않은 경우
        /// 항목 자체의 ToString()으로 폴백하며, 절대 null을 반환하지 않는다.
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
