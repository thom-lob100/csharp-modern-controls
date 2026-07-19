using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Modern.Lab.Controls.Wpf.Common;

namespace Modern.Lab.Controls.Wpf.Display
{
    /// <summary>
    /// 슬롯 맵 — 캐리어(운반체)의 수납 구조를 실물 단면처럼 그리는 표시/선택
    /// 컨트롤. 구획(SlotMapSection) 단위로 셀 격자를 그린다.
    ///
    /// 셀 표현 — **자리(번호)는 고정, 유닛(ID)은 움직인다**를 시각으로 나눈다:
    /// - 단일 수납(슬롯/스텁): 고정 번호 눈금 + 그 옆의 유닛 토큰(ID가 적힌
    ///   색 바). 빈 자리는 회색 빈 틀만 남는다.
    /// - 복합 수납(LCC 핑거 A~E): 핑거당 미니 행 — 이름 도트 + 유닛 ID +
    ///   삽입 위치 틱(Top/Left/Right, 도트 가장자리 액센트 바).
    ///
    /// 상호작용:
    /// - 채워진 셀 클릭 = 선택 토글 (AllowSelection) — SelectedKeys/
    ///   SelectionChanged로 화면에 전달.
    /// - EnableDragOut = 채워진 셀을 끌 수 있다 (선택된 셀을 끌면 선택 전체가
    ///   함께 간다). AcceptDrops = 드롭을 받아 UnitsDropped(키들 + 앵커 셀
    ///   키)를 발생시킨다 — 앵커는 "이 자리부터 채워 달라"는 의도이며 검증과
    ///   실제 이동은 화면(서버 호출)이 한다.
    /// - SetPreview(구획별 유닛 수) = "들어갈 빈 자리" 하이라이트. 부족하면
    ///   구획 집계가 빨간 "need n more"가 된다.
    ///
    /// 데이터는 SetSections로 통째로 준다(재조회 = 재구성) — 셀 시각 트리는
    /// 코드가 만들고 색은 전부 토큰에서 읽는다.
    /// </summary>
    public partial class ModernSlotMapControl : UserControl
    {
        // 드래그 페이로드의 DataObject 형식 이름.
        private const string dragDataFormat = "ModernLab.SlotMapKeys";

        /// <summary>셀 클릭 선택 허용 여부 — 대상(읽기 전용) 맵은 끈다.</summary>
        public static readonly DependencyProperty AllowSelectionProperty =
            DependencyProperty.Register(
                "AllowSelection", typeof(bool), typeof(ModernSlotMapControl),
                new PropertyMetadata(true));

        /// <summary>채워진 셀 드래그 시작 허용 여부 (원본 맵용, 기본 false).</summary>
        public static readonly DependencyProperty EnableDragOutProperty =
            DependencyProperty.Register(
                "EnableDragOut", typeof(bool), typeof(ModernSlotMapControl),
                new PropertyMetadata(false));

        /// <summary>드롭 수용 여부 (대상 맵용, 기본 false) — 켜면 AllowDrop이
        /// 함께 켜지고 UnitsDropped가 발생한다.</summary>
        public static readonly DependencyProperty AcceptDropsProperty =
            DependencyProperty.Register(
                "AcceptDrops", typeof(bool), typeof(ModernSlotMapControl),
                new PropertyMetadata(false, OnAcceptDropsChanged));

        /// <summary>선택이 바뀔 때 발생한다 (재구성/프로그램 선택 해제 등).</summary>
        public event EventHandler SelectionChanged;

        /// <summary>채움 셀을 클릭할 때 발생한다 — 선택 상태는 바꾸지 않고
        /// 클릭된 키만 알린다. 선택 표시는 화면이 SetSelectedKeys로 관리한다.</summary>
        public event EventHandler<SlotMapCellEventArgs> CellClicked;

        /// <summary>드롭을 받을 때 발생한다 — 끌려온 셀 키들과 앵커 셀 키.</summary>
        public event EventHandler<SlotMapDropEventArgs> UnitsDropped;

        // 셀 시각 요소 묶음 — 선택/미리보기 상태 변경 시 다시 칠할 대상.
        private sealed class CellVisual
        {
            internal SlotMapCell Cell;
            internal Border Outer;
            internal Border Token;
            internal Border LabelChip;
            internal TextBlock LabelText;
            internal TextBlock UnitText;
            internal List<Border> DotBorders;
            internal List<TextBlock> DotTexts;
            internal List<TextBlock> SubUnitTexts;
        }

        private SlotMapSection[] sections;
        private readonly List<List<CellVisual>> sectionVisuals = new List<List<CellVisual>>();
        private readonly List<TextBlock> sectionCountTexts = new List<TextBlock>();
        private readonly HashSet<string> selectedKeys = new HashSet<string>(StringComparer.Ordinal);

        // 클릭으로 강조한 단일 셀 — 스테이징(selectedKeys)과 별개로 살짝 다른
        // 색으로 표시한다. SetClickKey로 설정한다.
        private string clickKey;

        // 미리보기 맵 — 자리 키("SLOT|7" / "STUB|3" / "LCC|3|A") → 들어올 유닛
        // ID. 해당 자리가 비어 있으면 "→ ID"로 표기한다. 화면(폼)이 서버 배치
        // 계획을 그대로 받아 주므로 미리보기와 실제 이동 결과가 일치한다.
        private System.Collections.Generic.Dictionary<string, string> previewMap;

        // 드래그 시작 판정 — 마우스 다운 셀과 좌표를 기억해 두고, 이동 거리가
        // 시스템 임계값을 넘으면 드래그로, 그대로 떼면 클릭(선택 토글)으로 본다.
        private CellVisual pressCandidate;
        private Point pressPoint;

        public ModernSlotMapControl()
        {
            this.InitializeComponent();
        }

        /// <summary>셀 클릭 선택 허용 여부 (기본 true).</summary>
        public bool AllowSelection
        {
            get { return (bool)this.GetValue(AllowSelectionProperty); }
            set { this.SetValue(AllowSelectionProperty, value); }
        }

        /// <summary>채워진 셀 드래그 시작 허용 여부 (기본 false).</summary>
        public bool EnableDragOut
        {
            get { return (bool)this.GetValue(EnableDragOutProperty); }
            set { this.SetValue(EnableDragOutProperty, value); }
        }

        /// <summary>드롭 수용 여부 (기본 false).</summary>
        public bool AcceptDrops
        {
            get { return (bool)this.GetValue(AcceptDropsProperty); }
            set { this.SetValue(AcceptDropsProperty, value); }
        }

        private static void OnAcceptDropsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ModernSlotMapControl control = (ModernSlotMapControl)d;
            control.AllowDrop = (bool)e.NewValue;

            if ((bool)e.NewValue)
            {
                control.DragOver -= control.OnMapDragOver;
                control.Drop -= control.OnMapDrop;
                control.DragOver += control.OnMapDragOver;
                control.Drop += control.OnMapDrop;
            }
        }

        /// <summary>현재 선택된 셀 키 목록 (없으면 빈 배열).</summary>
        public string[] SelectedKeys
        {
            get
            {
                string[] keys = new string[this.selectedKeys.Count];
                this.selectedKeys.CopyTo(keys);
                return keys;
            }
        }

        /// <summary>구획들을 통째로 다시 그린다 — 재조회 반영 경로. 기존
        /// 선택과 미리보기는 초기화된다.</summary>
        public void SetSections(SlotMapSection[] newSections)
        {
            bool hadSelection = this.selectedKeys.Count > 0;

            this.sections = newSections;
            this.selectedKeys.Clear();
            this.clickKey = null;
            this.previewMap = null;
            this.RebuildVisualTree();

            if (hadSelection)
            {
                this.RaiseSelectionChanged();
            }
        }

        /// <summary>지정한 키들만 선택 상태로 만든다 — 이벤트를 발생시키지
        /// 않는다(이동 후 도착지에서 "방금 옮긴 유닛"을 강조하는 용도).</summary>
        public void SetSelectedKeys(string[] keys)
        {
            this.selectedKeys.Clear();

            if (keys != null)
            {
                foreach (string key in keys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        this.selectedKeys.Add(key);
                    }
                }
            }

            this.RefreshAllVisuals();
        }

        /// <summary>클릭 강조 셀을 지정한다 (스테이징과 다른 색). null이면 없음.
        /// 이벤트를 발생시키지 않는다.</summary>
        public void SetClickKey(string key)
        {
            this.clickKey = key;
            this.RefreshAllVisuals();
        }

        /// <summary>선택을 모두 해제한다.</summary>
        public void ClearSelection()
        {
            if (this.selectedKeys.Count == 0)
            {
                return;
            }

            this.selectedKeys.Clear();
            this.RefreshAllVisuals();
            this.RaiseSelectionChanged();
        }

        /// <summary>"들어갈 자리" 미리보기 — 자리 키("SLOT|7"/"STUB|3"/
        /// "LCC|3|A") → 들어올 유닛 ID 맵을 준다 (null = 해제). 그 자리가 비어
        /// 있으면 "→ ID"로 표기·하이라이트하고, 계획된 자리가 부족하면 집계에
        /// 빨간 부족분(need n more)을 표기한다.</summary>
        public void SetPreview(System.Collections.Generic.Dictionary<string, string> map)
        {
            this.previewMap = map;
            this.RefreshAllVisuals();
        }

        // ===== 시각 트리 구성 =====

        private void RebuildVisualTree()
        {
            this.SectionHost.Children.Clear();
            this.sectionVisuals.Clear();
            this.sectionCountTexts.Clear();
            this.pressCandidate = null;

            if (this.sections == null)
            {
                return;
            }

            for (int index = 0; index < this.sections.Length; index++)
            {
                this.SectionHost.Children.Add(this.BuildSection(this.sections[index]));
            }

            this.RefreshAllVisuals();
        }

        private UIElement BuildSection(SlotMapSection section)
        {
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(0d, 0d, 0d, 8d);

            // 구획 머리 — 제목(좌) + 채움 집계(우, 미리보기/부족분 포함).
            Grid header = new Grid();
            header.Margin = new Thickness(2d, 0d, 2d, 4d);
            header.ColumnDefinitions.Add(new ColumnDefinition());
            header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock title = new TextBlock();
            title.Text = section.Title;
            title.FontSize = (double)this.FindResource("Font.Size.Label");
            title.FontWeight = FontWeights.SemiBold;
            title.Foreground = (Brush)this.FindResource("Brush.TextSecondary");
            header.Children.Add(title);

            TextBlock count = new TextBlock();
            count.FontSize = (double)this.FindResource("Font.Size.Label");
            count.Foreground = (Brush)this.FindResource("Brush.TextSecondary");
            Grid.SetColumn(count, 1);
            header.Children.Add(count);
            this.sectionCountTexts.Add(count);

            panel.Children.Add(header);

            // 셀 격자 — Columns 열의 UniformGrid (1 = 세로 사다리).
            UniformGrid grid = new UniformGrid();
            grid.Columns = Math.Max(1, section.Columns);

            List<CellVisual> visuals = new List<CellVisual>();

            foreach (SlotMapCell cell in section.Cells)
            {
                CellVisual visual = this.BuildCell(cell, section.CellFontSize);
                visuals.Add(visual);
                grid.Children.Add(visual.Outer);
            }

            this.sectionVisuals.Add(visuals);
            panel.Children.Add(grid);
            return panel;
        }

        private CellVisual BuildCell(SlotMapCell cell, double cellFontSize)
        {
            CellVisual visual = new CellVisual();
            visual.Cell = cell;

            // 유닛 ID / 번호 칩 글자 크기 — 구획이 지정하면 그 값, 아니면 기본.
            double unitSize = cellFontSize > 0d ? cellFontSize : 11d;
            double chipSize = cellFontSize > 0d ? cellFontSize - 1d : 10d;

            Border outer = new Border();
            outer.Background = Brushes.Transparent;
            outer.CornerRadius = new CornerRadius(6d);
            outer.Margin = new Thickness(1d);
            outer.Tag = visual;
            outer.MouseLeftButtonDown += this.OnCellMouseDown;
            outer.MouseLeftButtonUp += this.OnCellMouseUp;
            outer.MouseMove += this.OnCellMouseMove;
            visual.Outer = outer;

            if (cell.SubCells == null)
            {
                // 단일 수납 — [고정 번호 칩 | 유닛 토큰] 구조. 번호는 자리에
                // 붙어 있고, ID가 적힌 토큰만 상태(채움/빈/미리보기)를 갖는다.
                Grid content = new Grid();
                content.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                content.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock label;
                Border labelChip = this.BuildNumberChip(cell.Label, chipSize, out label);
                labelChip.Margin = new Thickness(0d, 0d, 6d, 0d);
                content.Children.Add(labelChip);
                visual.LabelChip = labelChip;
                visual.LabelText = label;

                Border token = new Border();
                token.CornerRadius = new CornerRadius(4d);
                token.BorderThickness = new Thickness(1.5d);
                token.Padding = new Thickness(8d, 2d, 8d, 2d);
                token.MinHeight = unitSize + 8d;
                Grid.SetColumn(token, 1);
                visual.Token = token;

                TextBlock unit = new TextBlock();
                unit.FontSize = unitSize;
                unit.FontWeight = FontWeights.SemiBold;
                unit.TextTrimming = TextTrimming.CharacterEllipsis;
                unit.VerticalAlignment = VerticalAlignment.Center;
                token.Child = unit;
                visual.UnitText = unit;

                content.Children.Add(token);
                outer.Child = content;
                outer.ToolTip = !string.IsNullOrEmpty(cell.ToolTip)
                        ? cell.ToolTip
                        : (string.IsNullOrEmpty(cell.UnitId) ? null : (object)cell.UnitId);
                return visual;
            }

            // 복합 수납 — 번호 눈금 + 핑거당 미니 행(이름 도트 + 유닛 ID).
            Border frame = new Border();
            frame.CornerRadius = new CornerRadius(4d);
            frame.BorderThickness = new Thickness(1.5d);
            frame.Padding = new Thickness(4d, 2d, 4d, 2d);
            visual.Token = frame;

            StackPanel content2 = new StackPanel();

            TextBlock label2;
            Border labelChip2 = this.BuildNumberChip(cell.Label, chipSize, out label2);
            labelChip2.HorizontalAlignment = HorizontalAlignment.Left;
            labelChip2.Margin = new Thickness(0d, 0d, 0d, 2d);
            content2.Children.Add(labelChip2);
            visual.LabelChip = labelChip2;
            visual.LabelText = label2;

            visual.DotBorders = new List<Border>();
            visual.DotTexts = new List<TextBlock>();
            visual.SubUnitTexts = new List<TextBlock>();

            foreach (SlotMapSubCell sub in cell.SubCells)
            {
                Grid row = new Grid();
                row.Margin = new Thickness(0d, 0d, 0d, 1d);
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                row.ColumnDefinitions.Add(new ColumnDefinition());

                Grid dotGrid = new Grid();

                Border dot = new Border();
                dot.Width = 15d;
                dot.Height = 15d;
                dot.CornerRadius = new CornerRadius(3d);
                dot.BorderThickness = new Thickness(1d);
                dot.VerticalAlignment = VerticalAlignment.Center;

                TextBlock letter = new TextBlock();
                letter.Text = sub.Name;
                letter.FontSize = 8d;
                letter.HorizontalAlignment = HorizontalAlignment.Center;
                letter.VerticalAlignment = VerticalAlignment.Center;
                dot.Child = letter;
                dotGrid.Children.Add(dot);

                // 삽입 위치 틱 — 도트 가장자리의 2px 액센트 바 (채움 자리만).
                if (!string.IsNullOrEmpty(sub.UnitId) && !string.IsNullOrEmpty(sub.Marker))
                {
                    System.Windows.Shapes.Rectangle tick = new System.Windows.Shapes.Rectangle();
                    tick.Fill = (Brush)this.FindResource("Brush.Accent");
                    tick.IsHitTestVisible = false;

                    if (sub.Marker == "Top")
                    {
                        tick.Height = 2d;
                        tick.VerticalAlignment = VerticalAlignment.Top;
                        tick.Margin = new Thickness(3d, 1d, 3d, 0d);
                    }
                    else if (sub.Marker == "Left")
                    {
                        tick.Width = 2d;
                        tick.HorizontalAlignment = HorizontalAlignment.Left;
                        tick.Margin = new Thickness(1d, 3d, 0d, 3d);
                    }
                    else
                    {
                        tick.Width = 2d;
                        tick.HorizontalAlignment = HorizontalAlignment.Right;
                        tick.Margin = new Thickness(0d, 3d, 1d, 3d);
                    }

                    dotGrid.Children.Add(tick);
                }

                row.Children.Add(dotGrid);

                TextBlock subUnit = new TextBlock();
                subUnit.Text = sub.UnitId;
                subUnit.FontSize = 9d;
                subUnit.TextTrimming = TextTrimming.CharacterEllipsis;
                subUnit.VerticalAlignment = VerticalAlignment.Center;
                subUnit.Margin = new Thickness(4d, 0d, 0d, 0d);
                Grid.SetColumn(subUnit, 1);
                row.Children.Add(subUnit);

                content2.Children.Add(row);
                visual.DotBorders.Add(dot);
                visual.DotTexts.Add(letter);
                visual.SubUnitTexts.Add(subUnit);
            }

            frame.Child = content2;
            outer.Child = frame;
            outer.ToolTip = BuildSubToolTip(cell);
            return visual;
        }

        // 고정 자리 번호 칩 — 파랑 틴트 배경 + Info 텍스트의 작은 배지로
        // 번호에 포인트를 준다. 자리(번호)는 상태와 무관하게 항상 같은
        // 모습이라 "자리는 고정, 유닛이 움직인다"가 읽힌다.
        private Border BuildNumberChip(string label, double fontSize, out TextBlock text)
        {
            // 고정 높이 칩 + 좌우 패딩만. 숫자는 Grid 중앙 정렬로 위아래 정확히
            // 가운데 오게 한다 (TextBlock의 기본 라인 여백으로 위로 치우치던 것 보정).
            Border chip = new Border();
            chip.CornerRadius = new CornerRadius(3d);
            chip.Background = (Brush)this.FindResource("Brush.InfoBackground");
            chip.MinWidth = 22d;
            chip.Height = fontSize + 8d;
            chip.Padding = new Thickness(3d, 0d, 3d, 0d);
            chip.VerticalAlignment = VerticalAlignment.Center;
            chip.SnapsToDevicePixels = true;

            text = new TextBlock();
            text.Text = label;
            text.FontSize = fontSize;
            text.FontWeight = FontWeights.SemiBold;
            text.Foreground = (Brush)this.FindResource("Brush.InfoText");
            text.TextAlignment = TextAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.Padding = new Thickness(0d);
            text.LineHeight = fontSize + 2d;
            text.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            chip.Child = text;

            return chip;
        }

        // 복합 셀 툴팁 — 채워진 하위 자리의 "이름: 유닛 (삽입 위치)" 목록.
        private static object BuildSubToolTip(SlotMapCell cell)
        {
            StringBuilder text = new StringBuilder();

            foreach (SlotMapSubCell sub in cell.SubCells)
            {
                if (string.IsNullOrEmpty(sub.UnitId))
                {
                    continue;
                }

                if (text.Length > 0)
                {
                    text.AppendLine();
                }

                text.Append(sub.Name).Append(": ").Append(sub.UnitId);

                if (!string.IsNullOrEmpty(sub.Marker))
                {
                    text.Append(" (").Append(sub.Marker).Append(")");
                }

                if (!string.IsNullOrEmpty(sub.Detail))
                {
                    text.Append(" — ").Append(sub.Detail);
                }
            }

            return text.Length > 0 ? (object)text.ToString() : null;
        }

        // ===== 선택 + 드래그 시작 =====

        // 마우스 다운은 클릭/드래그 공용 시작점 — 여기서는 후보만 기억한다.
        private void OnCellMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border outer = sender as Border;
            CellVisual visual = outer != null ? outer.Tag as CellVisual : null;

            if (visual == null || !visual.Cell.Filled || string.IsNullOrEmpty(visual.Cell.Key))
            {
                return;
            }

            if (!this.AllowSelection && !this.EnableDragOut)
            {
                return;
            }

            this.pressCandidate = visual;
            this.pressPoint = e.GetPosition(this);
            e.Handled = true;
        }

        // 임계 거리를 넘게 끌면 드래그 시작 — 끌린 셀이 선택에 포함돼 있으면
        // 선택 전체가 페이로드가 되고, 아니면 그 셀 하나만 간다.
        private void OnCellMouseMove(object sender, MouseEventArgs e)
        {
            if (this.pressCandidate == null || !this.EnableDragOut
                    || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point position = e.GetPosition(this);

            if (Math.Abs(position.X - this.pressPoint.X) < SystemParameters.MinimumHorizontalDragDistance
                    && Math.Abs(position.Y - this.pressPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            string[] payload = this.selectedKeys.Contains(this.pressCandidate.Cell.Key)
                    ? this.SelectedKeys
                    : new string[] { this.pressCandidate.Cell.Key };

            this.pressCandidate = null;

            DataObject data = new DataObject(dragDataFormat, payload);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }

        // 드래그로 넘어가지 않고 같은 셀에서 떼면 클릭 — 선택 상태는 바꾸지
        // 않고 CellClicked만 알린다 (선택 표시는 화면이 SetSelectedKeys로 관리).
        private void OnCellMouseUp(object sender, MouseButtonEventArgs e)
        {
            Border outer = sender as Border;
            CellVisual visual = outer != null ? outer.Tag as CellVisual : null;

            if (visual == null || !object.ReferenceEquals(visual, this.pressCandidate))
            {
                this.pressCandidate = null;
                return;
            }

            this.pressCandidate = null;

            if (!this.AllowSelection)
            {
                return;
            }

            EventHandler<SlotMapCellEventArgs> handler = this.CellClicked;

            if (handler != null)
            {
                handler(this, new SlotMapCellEventArgs(visual.Cell.Key));
            }

            e.Handled = true;
        }

        private void RaiseSelectionChanged()
        {
            EventHandler handler = this.SelectionChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // ===== 드롭 수용 =====

        private void OnMapDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(dragDataFormat)
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnMapDrop(object sender, DragEventArgs e)
        {
            string[] keys = e.Data.GetData(dragDataFormat) as string[];

            if (keys == null || keys.Length == 0)
            {
                return;
            }

            // 놓은 지점의 셀(앵커) — 셀 밖이면 빈 문자열(앞에서부터 채움).
            CellVisual anchor = FindCellVisual(e.OriginalSource as DependencyObject);
            string anchorKey = anchor != null ? anchor.Cell.Key : string.Empty;

            EventHandler<SlotMapDropEventArgs> handler = this.UnitsDropped;

            if (handler != null)
            {
                handler(this, new SlotMapDropEventArgs(keys, anchorKey));
            }

            e.Handled = true;
        }

        // 드롭 지점의 시각 요소에서 셀(Outer Border의 Tag)을 찾는다.
        private static CellVisual FindCellVisual(DependencyObject element)
        {
            while (element != null)
            {
                Border border = element as Border;

                if (border != null && border.Tag is CellVisual)
                {
                    return (CellVisual)border.Tag;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        // ===== 상태 칠하기 =====

        // 단일 셀의 미리보기 키 = 자리 키. 하위(LCC 핑거) 미리보기 키 = "셀키|핑거".
        private static string SubPreviewKey(string cellKey, string subName)
        {
            return cellKey + "|" + subName;
        }

        // 이 구획에서 미리보기 맵이 지정한(=이 구획 자리로 계획된) 유닛 수와,
        // 그중 실제로 빈 자리에 놓일 수(previewed)를 센다. 지정 자리가 이미 차
        // 있으면 놓을 수 없어 부족분(shortage)이 된다.
        private void CountSectionPreview(int section, out int requested, out int previewed)
        {
            requested = 0;
            previewed = 0;

            if (this.previewMap == null || this.previewMap.Count == 0)
            {
                return;
            }

            foreach (CellVisual visual in this.sectionVisuals[section])
            {
                SlotMapCell cell = visual.Cell;

                if (string.IsNullOrEmpty(cell.Key))
                {
                    continue;
                }

                if (cell.SubCells == null)
                {
                    if (this.previewMap.ContainsKey(cell.Key))
                    {
                        requested = requested + 1;

                        if (!cell.Filled)
                        {
                            previewed = previewed + 1;
                        }
                    }

                    continue;
                }

                foreach (SlotMapSubCell sub in cell.SubCells)
                {
                    if (!this.previewMap.ContainsKey(SubPreviewKey(cell.Key, sub.Name)))
                    {
                        continue;
                    }

                    requested = requested + 1;

                    if (string.IsNullOrEmpty(sub.UnitId))
                    {
                        previewed = previewed + 1;
                    }
                }
            }
        }

        private void RefreshAllVisuals()
        {
            for (int section = 0; section < this.sectionVisuals.Count; section++)
            {
                foreach (CellVisual visual in this.sectionVisuals[section])
                {
                    this.ApplyCellVisual(visual);
                }

                this.UpdateSectionCount(section);
            }
        }

        // 구획 집계 — "채움 / 용량"에 미리보기(+n)와 부족분(need n more)을 덧붙인다.
        private void UpdateSectionCount(int section)
        {
            if (section >= this.sectionCountTexts.Count || this.sections == null)
            {
                return;
            }

            int filled = 0;
            int capacity = 0;

            foreach (CellVisual visual in this.sectionVisuals[section])
            {
                filled = filled + visual.Cell.UnitCount;
                capacity = capacity + visual.Cell.UnitCount + visual.Cell.EmptyUnitCount;
            }

            int requested;
            int previewed;
            this.CountSectionPreview(section, out requested, out previewed);
            int shortage = requested - previewed;

            TextBlock count = this.sectionCountTexts[section];
            string text = filled.ToString("N0") + " / " + capacity.ToString("N0");

            if (requested > 0)
            {
                text = text + "  ·  +" + previewed.ToString("N0");
            }

            if (shortage > 0)
            {
                text = text + "  ·  need " + shortage.ToString("N0") + " more";
                count.Foreground = (Brush)this.FindResource("Brush.ErrorText");
                count.FontWeight = FontWeights.SemiBold;
            }
            else
            {
                count.Foreground = (Brush)this.FindResource("Brush.TextSecondary");
                count.FontWeight = FontWeights.Normal;
            }

            count.Text = text;
        }

        // 색 배경 위 글자색 — 배경(색 문자열)에서 대비색을 유도한다(배지와
        // 같은 규칙). 색이 없으면 fallback(테마 기본 텍스트색)을 쓴다.
        // 테마 무관: 배경이 밝으면 어두운 글자, 어두우면 밝은 글자가 나온다.
        private Brush DeriveTextBrush(string color, Brush fallback)
        {
            System.Windows.Media.Color parsed;

            if (ChipColorHelper.TryParseColor(color, out parsed))
            {
                SolidColorBrush brush = new SolidColorBrush(ChipColorHelper.DeriveForeground(parsed));
                brush.Freeze();
                return brush;
            }

            return fallback;
        }

        private void ApplyCellVisual(CellVisual visual)
        {
            SlotMapCell cell = visual.Cell;
            // 세 종류의 강조가 겹칠 수 있다: staged(스테이징/미리보기 대상,
            // 강한 액센트) · clicked(마우스로 클릭) · 그 둘의 결합(combined).
            // 이제 clicked는 staged 위에도 표시되며(결합), 결합이면 셀 바깥에
            // 별도의 클릭 링을 둘러 "이미 선택된 자리 + 마우스로도 클릭한 자리"를
            // 함께 나타낸다.
            bool hasKey = !string.IsNullOrEmpty(cell.Key);
            bool staged = hasKey && this.selectedKeys.Contains(cell.Key);
            bool clicked = hasKey && cell.Filled
                    && cell.Key == this.clickKey && !string.IsNullOrEmpty(this.clickKey);
            bool combined = staged && clicked;
            bool selected = staged || clicked;
            bool interactive = (this.AllowSelection || this.EnableDragOut) && cell.Filled && hasKey;

            visual.Outer.Cursor = interactive ? Cursors.Hand : null;

            Brush accent = (Brush)this.FindResource("Brush.Accent");
            // 클릭 강조용 — 액센트보다 약한 색(살짝 다르게).
            Brush clickAccent = (Brush)this.FindResource("Brush.BorderHover");
            Brush borderBrush = (Brush)this.FindResource("Brush.Border");
            Brush borderSubtle = (Brush)this.FindResource("Brush.BorderSubtle");
            Brush surface = (Brush)this.FindResource("Brush.Surface");
            Brush neutral = (Brush)this.FindResource("Brush.NeutralBackground");
            Brush info = (Brush)this.FindResource("Brush.InfoBackground");
            Brush textPrimary = (Brush)this.FindResource("Brush.TextPrimary");
            Brush textDisabled = (Brush)this.FindResource("Brush.DisabledText");

            // 이 셀에 걸린 미리보기 존재 여부(번호 칩 하이라이트 판정용).
            bool previewed = this.CellHasPreview(cell);

            // 결합(staged+clicked)일 때 쓸 유닛 글씨 색 — 스테이징된 셀을
            // 마우스로 클릭하면 셀 바깥 포인트(링) 없이 **글씨 색만** 바꿔
            // "이 스테이징 셀을 클릭했다"를 나타낸다.
            Brush combinedText = (Brush)this.FindResource("Brush.Accent");

            // 강조 색 — staged/미리보기는 액센트 채움, clicked만이면 약한 틴트.
            Brush emphasis = staged || previewed ? accent : clickAccent;

            // 번호 칩 하이라이트. 스테이징+클릭 결합이면 칩 배경(액센트)은 그대로
            // 두고 **번호 글씨만** 연노랑으로 바꾼다 — 액센트 위에서 흰색과
            // 확실히 구분돼 "스테이징된 셀을 다시 클릭했음"을 번호에서도 나타낸다.
            if (staged || previewed)
            {
                visual.LabelChip.Background = accent;
                visual.LabelText.Foreground = combined
                        ? (Brush)this.FindResource("Brush.WarningBackground")
                        : (Brush)this.FindResource("Brush.OnAccent");
            }
            else if (clicked)
            {
                visual.LabelChip.Background = (Brush)this.FindResource("Brush.SelectedBackground");
                visual.LabelText.Foreground = (Brush)this.FindResource("Brush.InfoText");
            }
            else
            {
                visual.LabelChip.Background = info;
                visual.LabelText.Foreground = (Brush)this.FindResource("Brush.InfoText");
            }

            if (cell.SubCells == null)
            {
                string incoming = this.SingleCellPreview(cell);

                // 단일 수납 토큰 — 채움(색 바)/빈 틀/미리보기 세 상태.
                if (cell.Filled)
                {
                    Brush custom = ChipColorHelper.TryCreateBrush(cell.Color);
                    visual.Token.Background = custom != null
                            ? custom
                            : (Brush)this.FindResource("Brush.SelectedBackground");
                    visual.Token.BorderBrush = selected ? emphasis : borderBrush;
                    // 글자는 토큰 배경(아이템 색)에서 대비색을 유도한다 —
                    // 다크 테마에서도 밝은 배경 위에 어두운 글자가 나온다.
                    // 스테이징+클릭 결합이면 글씨 색만 액센트로 바꾼다.
                    visual.UnitText.Foreground = combined
                            ? combinedText
                            : this.DeriveTextBrush(cell.Color, textPrimary);
                    visual.UnitText.Text = cell.UnitId;
                }
                else if (!string.IsNullOrEmpty(incoming))
                {
                    // 들어올 유닛 ID를 그대로 보여준다 — "이 자리에 이게 온다".
                    visual.Token.Background = info;
                    visual.Token.BorderBrush = accent;
                    visual.UnitText.Foreground = accent;
                    visual.UnitText.Text = "→ " + incoming;
                }
                else
                {
                    visual.Token.Background = neutral;
                    visual.Token.BorderBrush = borderSubtle;
                    visual.UnitText.Text = string.Empty;
                }

                return;
            }

            // 복합 수납 — 틀은 중립(미니 행이 상태를 나타냄), 선택만 테두리로.
            visual.Token.Background = surface;
            visual.Token.BorderBrush = selected ? emphasis : (cell.Filled ? borderBrush : borderSubtle);

            for (int index = 0; index < cell.SubCells.Count; index++)
            {
                SlotMapSubCell sub = cell.SubCells[index];
                Border dot = visual.DotBorders[index];
                TextBlock letter = visual.DotTexts[index];
                TextBlock subUnit = visual.SubUnitTexts[index];

                string subIncoming = null;

                if (string.IsNullOrEmpty(sub.UnitId) && this.previewMap != null)
                {
                    this.previewMap.TryGetValue(SubPreviewKey(cell.Key, sub.Name), out subIncoming);
                }

                if (!string.IsNullOrEmpty(sub.UnitId))
                {
                    // 하위 자리 색 → 셀 색 → 토큰 기본색 순 폴백 (아이템별 구분).
                    Brush custom = ChipColorHelper.TryCreateBrush(
                            string.IsNullOrEmpty(sub.Color) ? cell.Color : sub.Color);
                    dot.Background = custom != null
                            ? custom
                            : (Brush)this.FindResource("Brush.SelectedBackground");
                    dot.BorderBrush = borderBrush;
                    // 도트 글자는 도트 배경(아이템 색)에서 대비색 유도. 옆의
                    // 유닛 ID 텍스트는 셀 표면(Surface) 위라 기본 텍스트색.
                    letter.Foreground = this.DeriveTextBrush(
                            string.IsNullOrEmpty(sub.Color) ? cell.Color : sub.Color, textPrimary);
                    // 스테이징+클릭 결합이면 유닛 글씨 색만 액센트로 바꾼다.
                    subUnit.Foreground = combined ? combinedText : textPrimary;
                    subUnit.Text = sub.UnitId;
                }
                else if (!string.IsNullOrEmpty(subIncoming))
                {
                    // 들어올 칩 ID를 계획된 핑거 자리에 그대로 보여준다.
                    dot.Background = info;
                    dot.BorderBrush = accent;
                    letter.Foreground = accent;
                    subUnit.Foreground = accent;
                    subUnit.Text = "→ " + subIncoming;
                }
                else
                {
                    dot.Background = neutral;
                    dot.BorderBrush = borderSubtle;
                    letter.Foreground = textDisabled;
                    subUnit.Foreground = textDisabled;
                    subUnit.Text = string.Empty;
                }
            }
        }

        // 이 셀의 빈 자리에 걸린 미리보기가 하나라도 있는가.
        private bool CellHasPreview(SlotMapCell cell)
        {
            if (this.previewMap == null || this.previewMap.Count == 0 || string.IsNullOrEmpty(cell.Key))
            {
                return false;
            }

            if (cell.SubCells == null)
            {
                return !cell.Filled && this.previewMap.ContainsKey(cell.Key);
            }

            foreach (SlotMapSubCell sub in cell.SubCells)
            {
                if (string.IsNullOrEmpty(sub.UnitId)
                        && this.previewMap.ContainsKey(SubPreviewKey(cell.Key, sub.Name)))
                {
                    return true;
                }
            }

            return false;
        }

        // 단일 셀의 미리보기 유입 ID (없으면 null).
        private string SingleCellPreview(SlotMapCell cell)
        {
            if (this.previewMap == null || cell.Filled || string.IsNullOrEmpty(cell.Key))
            {
                return null;
            }

            string incoming;
            this.previewMap.TryGetValue(cell.Key, out incoming);
            return incoming;
        }
    }
}
