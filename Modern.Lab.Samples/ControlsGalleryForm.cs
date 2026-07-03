using System;
using System.Drawing;
using System.Windows.Forms;
using Modern.Lab.Controls.Wpf.Display;
using Modern.Lab.Controls.Wpf.Input;
using Modern.Lab.WinForms.Controls.Display;
using Modern.Lab.WinForms.Controls.Input;

namespace Modern.Lab.Samples
{
    /// <summary>
    /// Smoke-test gallery: every control gets one row here as it is added to the
    /// library, so runtime rendering and events can be checked at a glance.
    /// </summary>
    public class ControlsGalleryForm : Form
    {
        private FlowLayoutPanel flowPanel;
        private ModernTextBox echoTextBox;
        private ModernLabel echoLabel;

        public ControlsGalleryForm()
        {
            this.InitializeLayout();
        }

        private void InitializeLayout()
        {
            this.Text = "Controls Gallery";
            this.BackColor = Color.White;

            this.flowPanel = new FlowLayoutPanel();
            this.flowPanel.Dock = DockStyle.Fill;
            this.flowPanel.FlowDirection = FlowDirection.TopDown;
            this.flowPanel.WrapContents = false;
            this.flowPanel.AutoScroll = true;
            this.flowPanel.Padding = new Padding(24);
            this.Controls.Add(this.flowPanel);

            this.AddTitle("ModernLabel");
            this.AddLabelSamples();

            this.AddTitle("ModernButton");
            this.AddButtonSamples();

            this.AddTitle("ModernTextBox");
            this.AddTextBoxSamples();
        }

        private void AddTitle(string title)
        {
            ModernLabel titleLabel = new ModernLabel();
            titleLabel.Text = title;
            titleLabel.Kind = LabelKind.Title;
            titleLabel.Size = new Size(400, 32);
            titleLabel.Margin = new Padding(0, 16, 0, 8);
            this.flowPanel.Controls.Add(titleLabel);
        }

        private void AddLabelSamples()
        {
            FlowLayoutPanel row = this.CreateRow();

            ModernLabel bodyLabel = new ModernLabel();
            bodyLabel.Text = "Body 본문 텍스트";
            bodyLabel.Kind = LabelKind.Body;
            bodyLabel.Size = new Size(140, 24);
            row.Controls.Add(bodyLabel);

            ModernLabel fieldLabel = new ModernLabel();
            fieldLabel.Text = "Label 필드 라벨";
            fieldLabel.Kind = LabelKind.Label;
            fieldLabel.Size = new Size(140, 24);
            row.Controls.Add(fieldLabel);

            ModernLabel helperLabel = new ModernLabel();
            helperLabel.Text = "Helper 보조 설명";
            helperLabel.Kind = LabelKind.Helper;
            helperLabel.Size = new Size(140, 24);
            row.Controls.Add(helperLabel);

            this.flowPanel.Controls.Add(row);
        }

        private void AddButtonSamples()
        {
            FlowLayoutPanel row = this.CreateRow();

            ModernButton primaryButton = new ModernButton();
            primaryButton.Text = "조회";
            primaryButton.Kind = ButtonKind.Primary;
            primaryButton.Click += this.OnAnyButtonClick;
            row.Controls.Add(primaryButton);

            ModernButton secondaryButton = new ModernButton();
            secondaryButton.Text = "초기화";
            secondaryButton.Kind = ButtonKind.Secondary;
            secondaryButton.Click += this.OnAnyButtonClick;
            row.Controls.Add(secondaryButton);

            ModernButton dangerButton = new ModernButton();
            dangerButton.Text = "삭제";
            dangerButton.Kind = ButtonKind.Danger;
            dangerButton.Click += this.OnAnyButtonClick;
            row.Controls.Add(dangerButton);

            ModernButton disabledButton = new ModernButton();
            disabledButton.Text = "비활성";
            disabledButton.Kind = ButtonKind.Primary;
            disabledButton.Enabled = false;
            row.Controls.Add(disabledButton);

            this.flowPanel.Controls.Add(row);
        }

        private void AddTextBoxSamples()
        {
            FlowLayoutPanel row = this.CreateRow();

            this.echoTextBox = new ModernTextBox();
            this.echoTextBox.PlaceholderText = "이름 또는 사번 입력 후 Enter";
            this.echoTextBox.Size = new Size(240, 32);
            this.echoTextBox.TextChanged += this.OnEchoTextChanged;
            this.echoTextBox.EnterPressed += this.OnEchoEnterPressed;
            row.Controls.Add(this.echoTextBox);

            ModernTextBox readOnlyTextBox = new ModernTextBox();
            readOnlyTextBox.Text = "읽기 전용";
            readOnlyTextBox.ReadOnly = true;
            readOnlyTextBox.Size = new Size(160, 32);
            row.Controls.Add(readOnlyTextBox);

            this.echoLabel = new ModernLabel();
            this.echoLabel.Text = "(TextChanged echo)";
            this.echoLabel.Kind = LabelKind.Helper;
            this.echoLabel.Size = new Size(280, 32);
            row.Controls.Add(this.echoLabel);

            this.flowPanel.Controls.Add(row);
        }

        private FlowLayoutPanel CreateRow()
        {
            FlowLayoutPanel row = new FlowLayoutPanel();
            row.FlowDirection = FlowDirection.LeftToRight;
            row.WrapContents = false;
            row.AutoSize = true;
            row.Margin = new Padding(0, 0, 0, 8);
            return row;
        }

        private void OnAnyButtonClick(object sender, EventArgs e)
        {
            ModernButton button = (ModernButton)sender;
            MessageBox.Show(this, button.Text + " 버튼이 눌렸습니다.", "Click");
        }

        private void OnEchoTextChanged(object sender, EventArgs e)
        {
            this.echoLabel.Text = this.echoTextBox.Text;
        }

        private void OnEchoEnterPressed(object sender, EventArgs e)
        {
            this.echoLabel.Text = "[Enter] " + this.echoTextBox.Text;
        }
    }
}
