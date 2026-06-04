namespace DateManager
{
    partial class LauncherForm
    {
        private System.ComponentModel.IContainer components = null;

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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "LauncherForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DonkeyCar System Launcher";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Panel pnlAccentLine;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Button btnAutoDrive;
        private System.Windows.Forms.Button btnOpenMainUi;
        private System.Windows.Forms.Label lblFooter;
        private Button btnTitle;
    }
}