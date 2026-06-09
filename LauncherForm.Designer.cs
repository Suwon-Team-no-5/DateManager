using System;
using System.Drawing;
using System.Windows.Forms;

namespace DateManager
{
    partial class LauncherForm
    {
        private System.ComponentModel.IContainer components = null;

        private const int BaseClientWidth = 1295;
        private const int BaseClientHeight = 805;
        private bool responsiveLayoutReady = false;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblSub = new Label();
            pnlAccentLine = new Panel();
            btnCollect = new Button();
            btnAutoDrive = new Button();
            btnOpenMainUi = new Button();
            lblFooter = new Label();
            btnTitle = new Button();
            SuspendLayout();
            // 
            // lblSub
            // 
            lblSub.Font = new Font("맑은 고딕", 13F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblSub.ForeColor = Color.FromArgb(170, 170, 175);
            lblSub.Location = new Point(0, 180);
            lblSub.Margin = new Padding(4, 0, 4, 0);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(1295, 35);
            lblSub.TabIndex = 1;
            lblSub.Text = "자율주행 데이터 수집, AI 학습, 자율주행 시스템";
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlAccentLine
            // 
            pnlAccentLine.BackColor = Color.FromArgb(0, 120, 215);
            pnlAccentLine.Location = new Point(597, 235);
            pnlAccentLine.Name = "pnlAccentLine";
            pnlAccentLine.Size = new Size(100, 4);
            pnlAccentLine.TabIndex = 6;
            // 
            // btnCollect
            // 
            btnCollect.BackColor = Color.FromArgb(45, 45, 48);
            btnCollect.BackgroundImageLayout = ImageLayout.Zoom;
            btnCollect.Cursor = Cursors.Hand;
            btnCollect.FlatAppearance.BorderSize = 0;
            btnCollect.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            btnCollect.FlatStyle = FlatStyle.Flat;
            btnCollect.Font = new Font("맑은 고딕", 16F, FontStyle.Bold, GraphicsUnit.Point, 129);
            btnCollect.ForeColor = Color.White;
            btnCollect.Image = Properties.Resources.joystick1;
            btnCollect.ImageAlign = ContentAlignment.MiddleRight;
            btnCollect.Location = new Point(347, 300);
            btnCollect.Margin = new Padding(4);
            btnCollect.Name = "btnCollect";
            btnCollect.Size = new Size(600, 90);
            btnCollect.TabIndex = 2;
            btnCollect.Text = "수동 주행 (데이터 수집)";
            btnCollect.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCollect.UseVisualStyleBackColor = false;
            btnCollect.Click += BtnCollect_Click;
            // 
            // btnAutoDrive
            // 
            btnAutoDrive.BackColor = Color.FromArgb(10, 132, 255);
            btnAutoDrive.BackgroundImageLayout = ImageLayout.Zoom;
            btnAutoDrive.Cursor = Cursors.Hand;
            btnAutoDrive.FlatAppearance.BorderSize = 0;
            btnAutoDrive.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 150, 255);
            btnAutoDrive.FlatStyle = FlatStyle.Flat;
            btnAutoDrive.Font = new Font("맑은 고딕", 16F, FontStyle.Bold, GraphicsUnit.Point, 129);
            btnAutoDrive.ForeColor = Color.White;
            btnAutoDrive.Image = Properties.Resources.rocket1;
            btnAutoDrive.ImageAlign = ContentAlignment.MiddleRight;
            btnAutoDrive.Location = new Point(347, 420);
            btnAutoDrive.Margin = new Padding(4);
            btnAutoDrive.Name = "btnAutoDrive";
            btnAutoDrive.Size = new Size(600, 90);
            btnAutoDrive.TabIndex = 3;
            btnAutoDrive.Text = "자율 주행 (AI 드라이빙)";
            btnAutoDrive.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAutoDrive.UseVisualStyleBackColor = false;
            btnAutoDrive.Click += BtnAutoDrive_Click;
            // 
            // btnOpenMainUi
            // 
            btnOpenMainUi.BackColor = Color.FromArgb(45, 45, 48);
            btnOpenMainUi.BackgroundImageLayout = ImageLayout.Zoom;
            btnOpenMainUi.Cursor = Cursors.Hand;
            btnOpenMainUi.FlatAppearance.BorderSize = 0;
            btnOpenMainUi.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            btnOpenMainUi.FlatStyle = FlatStyle.Flat;
            btnOpenMainUi.Font = new Font("맑은 고딕", 16F, FontStyle.Bold, GraphicsUnit.Point, 129);
            btnOpenMainUi.ForeColor = Color.White;
            btnOpenMainUi.Image = Properties.Resources.statistics2;
            btnOpenMainUi.ImageAlign = ContentAlignment.MiddleRight;
            btnOpenMainUi.Location = new Point(347, 540);
            btnOpenMainUi.Margin = new Padding(4);
            btnOpenMainUi.Name = "btnOpenMainUi";
            btnOpenMainUi.Size = new Size(600, 90);
            btnOpenMainUi.TabIndex = 4;
            btnOpenMainUi.Text = "Donkey UI 실행 (데이터 관리)";
            btnOpenMainUi.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnOpenMainUi.UseVisualStyleBackColor = false;
            btnOpenMainUi.Click += BtnOpenMainUi_Click;
            // 
            // lblFooter
            // 
            lblFooter.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFooter.ForeColor = Color.FromArgb(100, 100, 105);
            lblFooter.Location = new Point(0, 720);
            lblFooter.Margin = new Padding(4, 0, 4, 0);
            lblFooter.Name = "lblFooter";
            lblFooter.Size = new Size(1295, 30);
            lblFooter.TabIndex = 5;
            lblFooter.Text = "© 2026 프로그래밍언어 및 실습 5팀 | Donkeycar Project";
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnTitle
            // 
            btnTitle.FlatAppearance.BorderColor = Color.FromArgb(28, 28, 30);
            btnTitle.FlatStyle = FlatStyle.Flat;
            btnTitle.Font = new Font("Segoe UI Black", 32F, FontStyle.Bold);
            btnTitle.ForeColor = Color.White;
            btnTitle.Image = Properties.Resources.ai;
            btnTitle.ImageAlign = ContentAlignment.MiddleRight;
            btnTitle.Location = new Point(-12, 93);
            btnTitle.Name = "btnTitle";
            btnTitle.Size = new Size(1307, 84);
            btnTitle.TabIndex = 7;
            btnTitle.Text = "DONKEY UI : TEAM 5";
            btnTitle.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnTitle.UseVisualStyleBackColor = true;
            btnTitle.Click += btnTitle_Click;
            // 
            // LauncherForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 30);
            ClientSize = new Size(1295, 805);
            Controls.Add(btnTitle);
            Controls.Add(pnlAccentLine);
            Controls.Add(lblFooter);
            Controls.Add(btnOpenMainUi);
            Controls.Add(btnAutoDrive);
            Controls.Add(btnCollect);
            Controls.Add(lblSub);
            FormBorderStyle = FormBorderStyle.Sizable;
            Margin = new Padding(4);
            MaximizeBox = true;
            MinimizeBox = true;
            MinimumSize = new Size(900, 600);
            Name = "LauncherForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DonkeyCar System Launcher";
            Shown += LauncherForm_Shown;
            Resize += LauncherForm_Resize;
            ResumeLayout(false);
        }

        #endregion

        private void LauncherForm_Shown(object sender, EventArgs e)
        {
            responsiveLayoutReady = true;
            ApplyResponsiveLayout();
        }

        private void LauncherForm_Resize(object sender, EventArgs e)
        {
            if (!responsiveLayoutReady)
            {
                return;
            }

            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            float scaleX = ClientSize.Width / (float)BaseClientWidth;
            float scaleY = ClientSize.Height / (float)BaseClientHeight;
            float scale = Math.Min(scaleX, scaleY);

            int layoutWidth = (int)Math.Round(BaseClientWidth * scale);
            int layoutHeight = (int)Math.Round(BaseClientHeight * scale);
            int offsetX = (ClientSize.Width - layoutWidth) / 2;
            int offsetY = (ClientSize.Height - layoutHeight) / 2;

            SuspendLayout();

            SetScaledBounds(lblSub, 0, 180, 1295, 35, scale, offsetX, offsetY);
            SetScaledBounds(pnlAccentLine, 597, 235, 100, 4, scale, offsetX, offsetY);
            SetScaledBounds(btnCollect, 347, 300, 600, 90, scale, offsetX, offsetY);
            SetScaledBounds(btnAutoDrive, 347, 420, 600, 90, scale, offsetX, offsetY);
            SetScaledBounds(btnOpenMainUi, 347, 540, 600, 90, scale, offsetX, offsetY);
            SetScaledBounds(lblFooter, 0, 720, 1295, 30, scale, offsetX, offsetY);
            SetScaledBounds(btnTitle, -12, 93, 1307, 84, scale, offsetX, offsetY);

            SetScaledFont(lblSub, 13F, scale);
            SetScaledFont(btnCollect, 16F, scale);
            SetScaledFont(btnAutoDrive, 16F, scale);
            SetScaledFont(btnOpenMainUi, 16F, scale);
            SetScaledFont(lblFooter, 10F, scale);
            SetScaledFont(btnTitle, 32F, scale);

            ResumeLayout(false);
        }

        private void SetScaledBounds(Control control, int x, int y, int width, int height, float scale, int offsetX, int offsetY)
        {
            control.SetBounds(
                offsetX + (int)Math.Round(x * scale),
                offsetY + (int)Math.Round(y * scale),
                (int)Math.Round(width * scale),
                (int)Math.Round(height * scale)
            );
        }

        private void SetScaledFont(Control control, float baseFontSize, float scale)
        {
            float fontSize = Math.Max(8F, baseFontSize * scale);

            if (Math.Abs(control.Font.Size - fontSize) < 0.2F)
            {
                return;
            }

            control.Font = new Font(control.Font.FontFamily, fontSize, control.Font.Style, GraphicsUnit.Point);
        }

        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel pnlAccentLine;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Button btnAutoDrive;
        private System.Windows.Forms.Button btnOpenMainUi;
        private System.Windows.Forms.Label lblFooter;
        private Button btnTitle;
    }
}