namespace Modern.Lab.Controls.Wpf.Data
{
    /// <summary>
    /// ModernDataGrid / ModernDataGridControl이 사용하는 컬럼 정의.
    /// WinForms 폼이 WPF DataGrid 타입을 직접 다루지 않고도 컬럼을 정의할 수 있도록
    /// 하는 단순 데이터 홀더이다.
    /// </summary>
    public class ModernDataGridColumn
    {
        /// <summary>빈 정의를 만든다(스타 너비, 왼쪽 정렬).</summary>
        public ModernDataGridColumn()
        {
            this.DataPropertyName = string.Empty;
            this.HeaderText = string.Empty;
            this.Width = -1d;
            this.TextAlignment = GridTextAlignment.Left;
        }

        /// <summary>지정한 컬럼/속성에 바인딩되는 스타 너비 컬럼을 만든다.</summary>
        public ModernDataGridColumn(string dataPropertyName, string headerText)
            : this()
        {
            this.DataPropertyName = dataPropertyName;
            this.HeaderText = headerText;
        }

        /// <summary>지정한 컬럼/속성에 바인딩되는 고정 너비 컬럼을 만든다.</summary>
        public ModernDataGridColumn(string dataPropertyName, string headerText, double width)
            : this(dataPropertyName, headerText)
        {
            this.Width = width;
        }

        /// <summary>원본 컬럼/속성 이름(DataTable 컬럼 또는 객체 속성).</summary>
        public string DataPropertyName { get; set; }

        /// <summary>헤더 캡션.</summary>
        public string HeaderText { get; set; }

        /// <summary>픽셀 너비. 0 이하이면 스타 크기 조정(남은 공간 채우기)을 의미한다.</summary>
        public double Width { get; set; }

        /// <summary>셀 텍스트의 가로 정렬.</summary>
        public GridTextAlignment TextAlignment { get; set; }
    }
}
