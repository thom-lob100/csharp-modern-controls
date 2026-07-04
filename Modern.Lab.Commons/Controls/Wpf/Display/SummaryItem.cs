using System.ComponentModel;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>ModernSummaryListControl의 칩 하나를 나타내는 UI 항목 모델.</summary>
    public class SummaryItem : INotifyPropertyChanged
    {
        private string label;
        private string count;

        public event PropertyChangedEventHandler PropertyChanged;

        public SummaryItem()
        {
            this.label = string.Empty;
            this.count = string.Empty;
        }

        /// <summary>분류 텍스트 (예: 부서명 또는 직급명).</summary>
        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
                this.RaisePropertyChanged("Label");
            }
        }

        /// <summary>레이블 옆에 표시되는 건수 텍스트.</summary>
        public string Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
                this.RaisePropertyChanged("Count");
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
