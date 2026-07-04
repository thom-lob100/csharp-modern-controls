using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Modern.Lab.Controls.Wpf.Common;
using Modern.Lab.Controls.Wpf.Input;

namespace Modern.Lab.Controls.Wpf.Selection
{
    /// <summary>
    /// 모던 드롭다운 선택기.
    /// - ItemsSource / DisplayMemberPath / SelectedValuePath: 바인딩 표면
    /// - SelectedItem / SelectedValue: 현재 선택 (양방향)
    /// - IsEditable: 검색형 콤보 — 입력하면 목록이 필터링된다
    ///   (한국어 초성 매칭 포함)
    /// - Placeholder: 아무것도 선택/입력되지 않은 동안 표시되는 힌트
    /// - SelectionChanged: 선택이 바뀔 때 발생
    ///
    /// 내부 ComboBox는 ItemsSource의 내부 필터링 스냅숏에 바인딩되어 편집 가능
    /// 모드가 입력 중에 필터링할 수 있다. 소스 컬렉션 변경
    /// (ObservableCollection / IBindingList)은 감시되어 다시 반영된다.
    /// </summary>
    public partial class ModernComboBoxControl : UserControl
    {
        /// <summary>표시할 항목 목록. 임의의 IEnumerable (DataView, IList, ...).</summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ModernComboBoxControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>현재 선택된 항목. 기본적으로 양방향 바인딩.</summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(ModernComboBoxControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>선택된 항목의 값(SelectedValuePath 기준). 기본적으로 양방향 바인딩.</summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                "SelectedValue",
                typeof(object),
                typeof(ModernComboBoxControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>각 항목의 표시 텍스트에 사용되는 멤버 경로.</summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(ModernComboBoxControl),
                new PropertyMetadata(string.Empty));

        /// <summary>SelectedValue에 사용되는 멤버 경로.</summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                "SelectedValuePath",
                typeof(string),
                typeof(ModernComboBoxControl),
                new PropertyMetadata(string.Empty));

        /// <summary>아무것도 선택되지 않은 동안 표시되는 힌트 텍스트.</summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                "Placeholder",
                typeof(string),
                typeof(ModernComboBoxControl),
                new PropertyMetadata(string.Empty));

        /// <summary>검색형 콤보: 입력하면 목록이 필터링된다.</summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                "IsEditable",
                typeof(bool),
                typeof(ModernComboBoxControl),
                new PropertyMetadata(false, OnIsEditableChanged));

        private readonly ObservableCollection<object> filteredItems;
        private TextBox editableTextBox;
        private bool isRebuildingItems;
        private bool suppressFilter;

        /// <summary>선택이 바뀔 때 발생한다.</summary>
        public event EventHandler SelectionChanged;

        public ModernComboBoxControl()
        {
            this.filteredItems = new ObservableCollection<object>();
            this.InitializeComponent();
            this.InnerComboBox.ItemsSource = this.filteredItems;
            this.InnerComboBox.DropDownClosed += this.OnDropDownClosed;
            this.UpdatePlaceholderVisibility();
        }

        /// <summary>표시할 항목 목록.</summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>현재 선택된 항목.</summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>선택된 항목의 값(SelectedValuePath 기준).</summary>
        public object SelectedValue
        {
            get { return this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        /// <summary>각 항목의 표시 텍스트에 사용되는 멤버 경로.</summary>
        public string DisplayMemberPath
        {
            get { return (string)this.GetValue(DisplayMemberPathProperty); }
            set { this.SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>SelectedValue에 사용되는 멤버 경로.</summary>
        public string SelectedValuePath
        {
            get { return (string)this.GetValue(SelectedValuePathProperty); }
            set { this.SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>아무것도 선택되지 않은 동안 표시되는 힌트 텍스트.</summary>
        public string Placeholder
        {
            get { return (string)this.GetValue(PlaceholderProperty); }
            set { this.SetValue(PlaceholderProperty, value); }
        }

        /// <summary>검색형 콤보: 입력하면 목록이 필터링된다.</summary>
        public bool IsEditable
        {
            get { return (bool)this.GetValue(IsEditableProperty); }
            set { this.SetValue(IsEditableProperty, value); }
        }

        /// <summary>선택된 항목의 인덱스(아무것도 선택되지 않았으면 -1).</summary>
        public int SelectedIndex
        {
            get { return this.InnerComboBox.SelectedIndex; }
            set { this.InnerComboBox.SelectedIndex = value; }
        }

        /// <summary>현재 선택의 표시 텍스트(내부 ComboBox 텍스트).</summary>
        public string SelectionText
        {
            get { return this.InnerComboBox.Text; }
        }

        /// <summary>편집 가능 텍스트를 설정한다(검색형 콤보 전용).</summary>
        public void SetEditableText(string value)
        {
            if (this.IsEditable)
            {
                this.InnerComboBox.Text = value ?? string.Empty;
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModernComboBoxControl control = (ModernComboBoxControl)d;

            control.DetachSourceListeners(e.OldValue);
            control.AttachSourceListeners(e.NewValue);
            control.RebuildFilteredItems(null);
        }

        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernComboBoxControl)d).UpdatePlaceholderVisibility();
        }

        // 소스 컬렉션 변경을 감시하여(수동 Items 컬렉션은 ObservableCollection,
        // DataView는 IBindingList) 뒤늦게 추가된 항목도 직접 ItemsSource에
        // 바인딩했을 때처럼 나타나게 한다.
        private void AttachSourceListeners(object source)
        {
            INotifyCollectionChanged observable = source as INotifyCollectionChanged;

            if (observable != null)
            {
                observable.CollectionChanged += this.OnSourceCollectionChanged;
                return;
            }

            IBindingList bindingList = source as IBindingList;

            if (bindingList != null)
            {
                bindingList.ListChanged += this.OnSourceListChanged;
            }
        }

        private void DetachSourceListeners(object source)
        {
            INotifyCollectionChanged observable = source as INotifyCollectionChanged;

            if (observable != null)
            {
                observable.CollectionChanged -= this.OnSourceCollectionChanged;
                return;
            }

            IBindingList bindingList = source as IBindingList;

            if (bindingList != null)
            {
                bindingList.ListChanged -= this.OnSourceListChanged;
            }
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RebuildFilteredItems(null);
        }

        private void OnSourceListChanged(object sender, ListChangedEventArgs e)
        {
            this.RebuildFilteredItems(null);
        }

        // ItemsSource로부터 내부 목록을 다시 만들며, 표시 텍스트가 필터와 매칭되는
        // 항목만 남긴다(한국어 인식). 선택과 입력 텍스트를 보존하고, 재구성 중에는
        // SelectionChanged 잡음을 억제한다.
        private void RebuildFilteredItems(string filterText)
        {
            this.isRebuildingItems = true;

            try
            {
                object previousSelection = this.InnerComboBox.SelectedItem;
                string previousEditorText = this.editableTextBox != null ? this.editableTextBox.Text : null;
                int previousCaret = this.editableTextBox != null ? this.editableTextBox.CaretIndex : 0;

                this.filteredItems.Clear();

                IEnumerable source = this.ItemsSource;

                if (source != null)
                {
                    foreach (object item in source)
                    {
                        if (string.IsNullOrEmpty(filterText) ||
                            HangulTextMatcher.Contains(
                                MemberPathReader.ReadDisplayText(item, this.DisplayMemberPath), filterText))
                        {
                            this.filteredItems.Add(item);
                        }
                    }
                }

                if (previousSelection != null && this.filteredItems.Contains(previousSelection))
                {
                    this.InnerComboBox.SelectedItem = previousSelection;
                }

                // 선택을 비우거나 복원하면 편집 가능 텍스트가 다시 쓰이므로,
                // 사용자가 입력한 텍스트(와 캐럿)를 되돌려 놓는다. 이때 동기적으로
                // 발생하는 TextChanged는 isRebuildingItems로 무시된다.
                if (this.editableTextBox != null && previousEditorText != null &&
                    !string.Equals(this.editableTextBox.Text, previousEditorText, StringComparison.Ordinal))
                {
                    this.editableTextBox.Text = previousEditorText;
                    this.editableTextBox.CaretIndex = Math.Min(previousCaret, previousEditorText.Length);
                }
            }
            finally
            {
                this.isRebuildingItems = false;
            }

            this.UpdatePlaceholderVisibility();
        }

        // 템플릿에서 편집 가능 텍스트 영역을 얻어 입력을 후킹한다.
        // Loaded는 호스트 폼이 데이터를 이미 바인딩한 뒤 발생하므로,
        // 여기서 실제 에디터 상태를 기준으로 placeholder를 다시 평가한다.
        private void InnerComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox editor = this.InnerComboBox.Template.FindName("PART_EditableTextBox", this.InnerComboBox) as TextBox;

            if (!object.ReferenceEquals(editor, this.editableTextBox))
            {
                if (this.editableTextBox != null)
                {
                    this.editableTextBox.TextChanged -= this.OnEditableTextChanged;
                }

                this.editableTextBox = editor;

                if (this.editableTextBox != null)
                {
                    this.editableTextBox.TextChanged += this.OnEditableTextChanged;
                }
            }

            this.UpdatePlaceholderVisibility();
        }

        // IME 조합 중에도 발생하므로, 첫 자음 입력부터 목록이 필터링되고
        // placeholder가 숨겨진다.
        private void OnEditableTextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdatePlaceholderVisibility();

            if (!this.IsEditable || this.isRebuildingItems)
            {
                return;
            }

            if (this.suppressFilter)
            {
                this.suppressFilter = false;
                return;
            }

            if (!this.InnerComboBox.IsKeyboardFocusWithin)
            {
                return;
            }

            string typed = this.editableTextBox.Text;

            if (string.IsNullOrEmpty(typed))
            {
                // 텍스트를 지우면 선택도 지워진다(빈 값은 "전체"를 의미).
                if (this.InnerComboBox.SelectedItem != null)
                {
                    this.InnerComboBox.SelectedItem = null;
                    // 선택 변경은 텍스트 재작성을 예상하고 suppressFilter를
                    // 설정하지만, 텍스트가 이미 비어 있으므로 플래그를 지운다.
                    this.suppressFilter = false;
                }

                this.RebuildFilteredItems(null);
                this.InnerComboBox.IsDropDownOpen = false;
                return;
            }

            this.RebuildFilteredItems(typed);
            this.InnerComboBox.IsDropDownOpen = this.filteredItems.Count > 0;
        }

        // 셰브런으로 드롭다운을 다시 열 때는 전체 목록이 보여야 한다.
        // 또한 오래된 suppress 플래그를 지운다(텍스트를 바꾸지 않은 선택이
        // 다음 키 입력의 필터링을 삼켜버릴 수 있기 때문).
        private void OnDropDownClosed(object sender, EventArgs e)
        {
            if (this.IsEditable)
            {
                this.RebuildFilteredItems(null);
                this.suppressFilter = false;
            }
        }

        private void UpdatePlaceholderVisibility()
        {
            bool empty;

            if (this.IsEditable)
            {
                string editorText = this.editableTextBox != null ? this.editableTextBox.Text : this.InnerComboBox.Text;
                empty = string.IsNullOrEmpty(editorText);
            }
            else
            {
                empty = this.InnerComboBox.SelectedItem == null;
            }

            this.PlaceholderOverlay.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void InnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isRebuildingItems)
            {
                return;
            }

            // 선택을 확정하면 편집 가능 텍스트가 항목의 표시 텍스트로 다시 쓰인다;
            // 이 변경이 필터링을 다시 트리거해서는 안 된다.
            if (this.IsEditable)
            {
                this.suppressFilter = true;
            }

            this.UpdatePlaceholderVisibility();

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}
