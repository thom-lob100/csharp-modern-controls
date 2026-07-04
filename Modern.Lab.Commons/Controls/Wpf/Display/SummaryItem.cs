using System.ComponentModel;
using System.Windows.Media;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>ModernSummaryListControl의 칩 하나를 나타내는 UI 항목 모델.</summary>
    public class SummaryItem : INotifyPropertyChanged
    {
        private string label;
        private string count;
        private Brush background;
        private Brush labelForeground;
        private Brush countForeground;

        public event PropertyChangedEventHandler PropertyChanged;

        public SummaryItem()
        {
            this.label = string.Empty;
            this.count = string.Empty;
            this.background = null;
            this.labelForeground = null;
            this.countForeground = null;
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

        /// <summary>칩 배경 브러시. null이면 기본 토큰(Brush.NeutralBackground)으로 폴백.</summary>
        public Brush Background
        {
            get
            {
                return this.background;
            }
            set
            {
                this.background = value;
                this.RaisePropertyChanged("Background");
            }
        }

        /// <summary>레이블 글자 브러시. null이면 기본 토큰(Brush.NeutralText)으로 폴백.</summary>
        public Brush LabelForeground
        {
            get
            {
                return this.labelForeground;
            }
            set
            {
                this.labelForeground = value;
                this.RaisePropertyChanged("LabelForeground");
            }
        }

        /// <summary>건수 글자 브러시. null이면 기본 토큰(Brush.SelectedText)으로 폴백.</summary>
        public Brush CountForeground
        {
            get
            {
                return this.countForeground;
            }
            set
            {
                this.countForeground = value;
                this.RaisePropertyChanged("CountForeground");
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
