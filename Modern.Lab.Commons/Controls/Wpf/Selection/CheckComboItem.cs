using System.ComponentModel;

namespace Modern.Lab.Controls.Wpf.Selection
{
    /// <summary>ModernCheckComboBoxControl의 체크 행 하나를 나타내는 UI 항목 모델.</summary>
    public class CheckComboItem : INotifyPropertyChanged
    {
        private readonly object item;
        private string displayText;
        private bool isChecked;

        public event PropertyChangedEventHandler PropertyChanged;

        public CheckComboItem(object item, string displayText)
        {
            this.item = item;
            this.displayText = displayText ?? string.Empty;
            this.isChecked = false;
        }

        /// <summary>바인딩된 원본 행 (DataRowView, 객체 등).</summary>
        public object Item
        {
            get { return this.item; }
        }

        /// <summary>체크 표시 옆에 보여줄 텍스트.</summary>
        public string DisplayText
        {
            get
            {
                return this.displayText;
            }
            set
            {
                this.displayText = value;
                this.RaisePropertyChanged("DisplayText");
            }
        }

        /// <summary>체크 상태 (체크 표시와 양방향 바인딩).</summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.RaisePropertyChanged("IsChecked");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
