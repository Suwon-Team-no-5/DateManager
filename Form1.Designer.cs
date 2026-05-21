namespace DateManager
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pbMainCam = new PictureBox();
            lblThrottleBottom = new Label();
            lblFrameIndex = new Label();
            lblAngle = new Label();
            lblThrottleTop = new Label();
            lblTimestamp = new Label();
            gbCamView = new GroupBox();
            gbNavigation = new GroupBox();
            btnSpeed = new Button();
            btnFastForward = new Button();
            btnFastRewind = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            btnPlay = new Button();
            trkFrameSlider = new TrackBar();
            btnStop = new Button();
            gbFrameList = new GroupBox();
            lstFrameData = new ListBox();
            gbSystemOps = new GroupBox();
            btnStartTraining = new Button();
            btnLoadTub = new Button();
            btnLoadConfig = new Button();
            gbDataManagement = new GroupBox();
            lblSetRange = new Label();
            btnSetLeft = new Button();
            btnDeleteData = new Button();
            btnSetRight = new Button();
            btnApplyFilter = new Button();
            gbFilterOptions = new GroupBox();
            chkFilterLargeAngle = new CheckBox();
            chkFilterAngleZero = new CheckBox();
            chkFilterThr = new CheckBox();
            gbTrainingLog = new GroupBox();
            rtbTrainLog = new RichTextBox();
            lblTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)pbMainCam).BeginInit();
            gbCamView.SuspendLayout();
            gbNavigation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameSlider).BeginInit();
            gbFrameList.SuspendLayout();
            gbSystemOps.SuspendLayout();
            gbDataManagement.SuspendLayout();
            gbFilterOptions.SuspendLayout();
            gbTrainingLog.SuspendLayout();
            SuspendLayout();
            // 
            // pbMainCam
            // 
            pbMainCam.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbMainCam.Location = new Point(12, 107);
            pbMainCam.Margin = new Padding(6);
            pbMainCam.Name = "pbMainCam";
            pbMainCam.Size = new Size(1290, 728);
            pbMainCam.TabIndex = 0;
            pbMainCam.TabStop = false;
            // 
            // lblThrottleBottom
            // 
            lblThrottleBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblThrottleBottom.AutoSize = true;
            lblThrottleBottom.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblThrottleBottom.ForeColor = Color.White;
            lblThrottleBottom.Location = new Point(975, 760);
            lblThrottleBottom.Margin = new Padding(6, 0, 6, 0);
            lblThrottleBottom.Name = "lblThrottleBottom";
            lblThrottleBottom.Size = new Size(230, 45);
            lblThrottleBottom.TabIndex = 1;
            lblThrottleBottom.Text = "Throttle: +0.0";
            // 
            // lblFrameIndex
            // 
            lblFrameIndex.AutoSize = true;
            lblFrameIndex.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblFrameIndex.ForeColor = Color.White;
            lblFrameIndex.Location = new Point(26, 50);
            lblFrameIndex.Margin = new Padding(6, 0, 6, 0);
            lblFrameIndex.Name = "lblFrameIndex";
            lblFrameIndex.Size = new Size(202, 32);
            lblFrameIndex.TabIndex = 2;
            lblFrameIndex.Text = "Frame Index 0/0";
            // 
            // lblAngle
            // 
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblAngle.ForeColor = Color.White;
            lblAngle.Location = new Point(411, 50);
            lblAngle.Margin = new Padding(6, 0, 6, 0);
            lblAngle.Name = "lblAngle";
            lblAngle.Size = new Size(146, 32);
            lblAngle.TabIndex = 3;
            lblAngle.Text = "Angle: +0.0";
            // 
            // lblThrottleTop
            // 
            lblThrottleTop.AutoSize = true;
            lblThrottleTop.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblThrottleTop.ForeColor = Color.White;
            lblThrottleTop.Location = new Point(709, 50);
            lblThrottleTop.Margin = new Padding(6, 0, 6, 0);
            lblThrottleTop.Name = "lblThrottleTop";
            lblThrottleTop.Size = new Size(170, 32);
            lblThrottleTop.TabIndex = 4;
            lblThrottleTop.Text = "Throttle: +0.0";
            // 
            // lblTimestamp
            // 
            lblTimestamp.AutoSize = true;
            lblTimestamp.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblTimestamp.ForeColor = Color.White;
            lblTimestamp.Location = new Point(1010, 50);
            lblTimestamp.Margin = new Padding(6, 0, 6, 0);
            lblTimestamp.Name = "lblTimestamp";
            lblTimestamp.Size = new Size(140, 32);
            lblTimestamp.TabIndex = 5;
            lblTimestamp.Text = "Timestamp";
            // 
            // gbCamView
            // 
            gbCamView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbCamView.Controls.Add(lblThrottleBottom);
            gbCamView.Controls.Add(pbMainCam);
            gbCamView.Controls.Add(lblTimestamp);
            gbCamView.Controls.Add(lblFrameIndex);
            gbCamView.Controls.Add(lblThrottleTop);
            gbCamView.Controls.Add(lblAngle);
            gbCamView.ForeColor = Color.White;
            gbCamView.Location = new Point(48, 114);
            gbCamView.Margin = new Padding(6);
            gbCamView.Name = "gbCamView";
            gbCamView.Padding = new Padding(6);
            gbCamView.Size = new Size(1314, 846);
            gbCamView.TabIndex = 6;
            gbCamView.TabStop = false;
            gbCamView.Text = "CAM IMAGE VIEW";
            // 
            // gbNavigation
            // 
            gbNavigation.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbNavigation.Controls.Add(btnSpeed);
            gbNavigation.Controls.Add(btnFastForward);
            gbNavigation.Controls.Add(btnFastRewind);
            gbNavigation.Controls.Add(btnNext);
            gbNavigation.Controls.Add(btnPrev);
            gbNavigation.Controls.Add(btnPlay);
            gbNavigation.Controls.Add(trkFrameSlider);
            gbNavigation.Controls.Add(btnStop);
            gbNavigation.ForeColor = Color.White;
            gbNavigation.Location = new Point(48, 973);
            gbNavigation.Margin = new Padding(6);
            gbNavigation.Name = "gbNavigation";
            gbNavigation.Padding = new Padding(6);
            gbNavigation.Size = new Size(1314, 267);
            gbNavigation.TabIndex = 7;
            gbNavigation.TabStop = false;
            gbNavigation.Text = "DATAFRAME NAVIGATION";
            // 
            // btnSpeed
            // 
            btnSpeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSpeed.BackColor = Color.Silver;
            btnSpeed.ForeColor = Color.Black;
            btnSpeed.Location = new Point(709, 174);
            btnSpeed.Margin = new Padding(6);
            btnSpeed.Name = "btnSpeed";
            btnSpeed.Size = new Size(188, 62);
            btnSpeed.TabIndex = 7;
            btnSpeed.Text = "1.0";
            btnSpeed.UseVisualStyleBackColor = false;
            // 
            // btnFastForward
            // 
            btnFastForward.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFastForward.BackColor = Color.Silver;
            btnFastForward.ForeColor = Color.Black;
            btnFastForward.Location = new Point(1114, 174);
            btnFastForward.Margin = new Padding(6);
            btnFastForward.Name = "btnFastForward";
            btnFastForward.Size = new Size(188, 62);
            btnFastForward.TabIndex = 6;
            btnFastForward.Text = ">>>";
            btnFastForward.UseVisualStyleBackColor = false;
            // 
            // btnFastRewind
            // 
            btnFastRewind.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFastRewind.BackColor = Color.Silver;
            btnFastRewind.ForeColor = Color.Black;
            btnFastRewind.Location = new Point(915, 174);
            btnFastRewind.Margin = new Padding(6);
            btnFastRewind.Name = "btnFastRewind";
            btnFastRewind.Size = new Size(188, 62);
            btnFastRewind.TabIndex = 5;
            btnFastRewind.Text = "<<<";
            btnFastRewind.UseVisualStyleBackColor = false;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.BackColor = Color.Silver;
            btnNext.ForeColor = Color.Black;
            btnNext.Location = new Point(1114, 101);
            btnNext.Margin = new Padding(6);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(188, 62);
            btnNext.TabIndex = 4;
            btnNext.Text = ">";
            btnNext.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.BackColor = Color.Silver;
            btnPrev.ForeColor = Color.Black;
            btnPrev.Location = new Point(915, 101);
            btnPrev.Margin = new Padding(6);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(188, 62);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "<";
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnPlay
            // 
            btnPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlay.BackColor = Color.Silver;
            btnPlay.ForeColor = Color.Black;
            btnPlay.Location = new Point(915, 26);
            btnPlay.Margin = new Padding(6);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(188, 62);
            btnPlay.TabIndex = 2;
            btnPlay.Text = "Play";
            btnPlay.UseVisualStyleBackColor = false;
            // 
            // trkFrameSlider
            // 
            trkFrameSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkFrameSlider.Location = new Point(6, 50);
            trkFrameSlider.Margin = new Padding(6);
            trkFrameSlider.Name = "trkFrameSlider";
            trkFrameSlider.Size = new Size(891, 90);
            trkFrameSlider.TabIndex = 1;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.BackColor = Color.Silver;
            btnStop.ForeColor = Color.Black;
            btnStop.Location = new Point(1114, 26);
            btnStop.Margin = new Padding(6);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(188, 62);
            btnStop.TabIndex = 0;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            // 
            // gbFrameList
            // 
            gbFrameList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            gbFrameList.Controls.Add(lstFrameData);
            gbFrameList.ForeColor = Color.White;
            gbFrameList.Location = new Point(1374, 114);
            gbFrameList.Margin = new Padding(6);
            gbFrameList.Name = "gbFrameList";
            gbFrameList.Padding = new Padding(6);
            gbFrameList.Size = new Size(294, 504);
            gbFrameList.TabIndex = 8;
            gbFrameList.TabStop = false;
            gbFrameList.Text = "FRAME LIST";
            // 
            // lstFrameData
            // 
            lstFrameData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFrameData.FormattingEnabled = true;
            lstFrameData.Location = new Point(12, 46);
            lstFrameData.Margin = new Padding(6);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.Size = new Size(265, 420);
            lstFrameData.TabIndex = 0;
            // 
            // gbSystemOps
            // 
            gbSystemOps.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            gbSystemOps.Controls.Add(btnStartTraining);
            gbSystemOps.Controls.Add(btnLoadTub);
            gbSystemOps.Controls.Add(btnLoadConfig);
            gbSystemOps.ForeColor = Color.White;
            gbSystemOps.Location = new Point(1680, 114);
            gbSystemOps.Margin = new Padding(6);
            gbSystemOps.Name = "gbSystemOps";
            gbSystemOps.Padding = new Padding(6);
            gbSystemOps.Size = new Size(310, 504);
            gbSystemOps.TabIndex = 9;
            gbSystemOps.TabStop = false;
            gbSystemOps.Text = "SYSTEM OPERATIONS";
            // 
            // btnStartTraining
            // 
            btnStartTraining.BackColor = Color.MediumSeaGreen;
            btnStartTraining.Font = new Font("맑은 고딕", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnStartTraining.ForeColor = Color.Yellow;
            btnStartTraining.Location = new Point(12, 248);
            btnStartTraining.Margin = new Padding(6);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(286, 237);
            btnStartTraining.TabIndex = 2;
            btnStartTraining.Text = "Start AI Model\r\nTraining";
            btnStartTraining.UseVisualStyleBackColor = false;
            // 
            // btnLoadTub
            // 
            btnLoadTub.BackColor = Color.CornflowerBlue;
            btnLoadTub.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnLoadTub.ForeColor = Color.White;
            btnLoadTub.Location = new Point(12, 133);
            btnLoadTub.Margin = new Padding(6);
            btnLoadTub.Name = "btnLoadTub";
            btnLoadTub.Size = new Size(286, 102);
            btnLoadTub.TabIndex = 1;
            btnLoadTub.Text = "Load tub";
            btnLoadTub.UseVisualStyleBackColor = false;
            btnLoadTub.Click += btnLoadTub_Click;
            // 
            // btnLoadConfig
            // 
            btnLoadConfig.BackColor = Color.CornflowerBlue;
            btnLoadConfig.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnLoadConfig.ForeColor = Color.White;
            btnLoadConfig.Location = new Point(12, 46);
            btnLoadConfig.Margin = new Padding(6);
            btnLoadConfig.Name = "btnLoadConfig";
            btnLoadConfig.Size = new Size(286, 72);
            btnLoadConfig.TabIndex = 0;
            btnLoadConfig.Text = "Load config";
            btnLoadConfig.UseVisualStyleBackColor = false;
            // 
            // gbDataManagement
            // 
            gbDataManagement.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            gbDataManagement.Controls.Add(lblSetRange);
            gbDataManagement.Controls.Add(btnSetLeft);
            gbDataManagement.Controls.Add(btnDeleteData);
            gbDataManagement.Controls.Add(btnSetRight);
            gbDataManagement.Controls.Add(btnApplyFilter);
            gbDataManagement.Controls.Add(gbFilterOptions);
            gbDataManagement.ForeColor = Color.White;
            gbDataManagement.Location = new Point(1374, 629);
            gbDataManagement.Margin = new Padding(6);
            gbDataManagement.Name = "gbDataManagement";
            gbDataManagement.Padding = new Padding(6);
            gbDataManagement.Size = new Size(616, 331);
            gbDataManagement.TabIndex = 10;
            gbDataManagement.TabStop = false;
            gbDataManagement.Text = "DATA MANAGEMENT && FILTERING";
            // 
            // lblSetRange
            // 
            lblSetRange.AutoSize = true;
            lblSetRange.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblSetRange.ForeColor = Color.White;
            lblSetRange.Location = new Point(401, 109);
            lblSetRange.Margin = new Padding(6, 0, 6, 0);
            lblSetRange.Name = "lblSetRange";
            lblSetRange.Size = new Size(94, 45);
            lblSetRange.TabIndex = 8;
            lblSetRange.Text = "(0, 0)";
            // 
            // btnSetLeft
            // 
            btnSetLeft.BackColor = Color.Silver;
            btnSetLeft.ForeColor = Color.Black;
            btnSetLeft.Location = new Point(306, 168);
            btnSetLeft.Margin = new Padding(6);
            btnSetLeft.Name = "btnSetLeft";
            btnSetLeft.Size = new Size(134, 62);
            btnSetLeft.TabIndex = 7;
            btnSetLeft.Text = "Set L";
            btnSetLeft.UseVisualStyleBackColor = false;
            // 
            // btnDeleteData
            // 
            btnDeleteData.BackColor = Color.Red;
            btnDeleteData.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnDeleteData.ForeColor = Color.White;
            btnDeleteData.Location = new Point(306, 243);
            btnDeleteData.Margin = new Padding(6);
            btnDeleteData.Name = "btnDeleteData";
            btnDeleteData.Size = new Size(275, 62);
            btnDeleteData.TabIndex = 6;
            btnDeleteData.Text = "Delete";
            btnDeleteData.UseVisualStyleBackColor = false;
            btnDeleteData.Click += btnDeleteData_Click;
            // 
            // btnSetRight
            // 
            btnSetRight.BackColor = Color.Silver;
            btnSetRight.ForeColor = Color.Black;
            btnSetRight.Location = new Point(448, 168);
            btnSetRight.Margin = new Padding(6);
            btnSetRight.Name = "btnSetRight";
            btnSetRight.Size = new Size(134, 62);
            btnSetRight.TabIndex = 5;
            btnSetRight.Text = "Set R";
            btnSetRight.UseVisualStyleBackColor = false;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.BackColor = Color.CornflowerBlue;
            btnApplyFilter.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnApplyFilter.ForeColor = Color.White;
            btnApplyFilter.Location = new Point(306, 32);
            btnApplyFilter.Margin = new Padding(6);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(275, 62);
            btnApplyFilter.TabIndex = 3;
            btnApplyFilter.Text = "Apply Filter";
            btnApplyFilter.UseVisualStyleBackColor = false;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // gbFilterOptions
            // 
            gbFilterOptions.Controls.Add(chkFilterLargeAngle);
            gbFilterOptions.Controls.Add(chkFilterAngleZero);
            gbFilterOptions.Controls.Add(chkFilterThr);
            gbFilterOptions.ForeColor = Color.White;
            gbFilterOptions.Location = new Point(12, 46);
            gbFilterOptions.Margin = new Padding(6);
            gbFilterOptions.Name = "gbFilterOptions";
            gbFilterOptions.Padding = new Padding(6);
            gbFilterOptions.Size = new Size(282, 258);
            gbFilterOptions.TabIndex = 0;
            gbFilterOptions.TabStop = false;
            gbFilterOptions.Text = "Filter Options";
            // 
            // chkFilterLargeAngle
            // 
            chkFilterLargeAngle.AutoSize = true;
            chkFilterLargeAngle.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterLargeAngle.ForeColor = Color.White;
            chkFilterLargeAngle.Location = new Point(12, 168);
            chkFilterLargeAngle.Margin = new Padding(6);
            chkFilterLargeAngle.Name = "chkFilterLargeAngle";
            chkFilterLargeAngle.Size = new Size(176, 36);
            chkFilterLargeAngle.TabIndex = 2;
            chkFilterLargeAngle.Text = "Large Angle";
            chkFilterLargeAngle.UseVisualStyleBackColor = true;
            // 
            // chkFilterAngleZero
            // 
            chkFilterAngleZero.AutoSize = true;
            chkFilterAngleZero.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterAngleZero.ForeColor = Color.White;
            chkFilterAngleZero.Location = new Point(12, 115);
            chkFilterAngleZero.Margin = new Padding(6);
            chkFilterAngleZero.Name = "chkFilterAngleZero";
            chkFilterAngleZero.Size = new Size(172, 36);
            chkFilterAngleZero.TabIndex = 1;
            chkFilterAngleZero.Text = "Angle == 0";
            chkFilterAngleZero.UseVisualStyleBackColor = true;
            // 
            // chkFilterThr
            // 
            chkFilterThr.AutoSize = true;
            chkFilterThr.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterThr.ForeColor = Color.White;
            chkFilterThr.Location = new Point(12, 62);
            chkFilterThr.Margin = new Padding(6);
            chkFilterThr.Name = "chkFilterThr";
            chkFilterThr.Size = new Size(127, 36);
            chkFilterThr.TabIndex = 0;
            chkFilterThr.Text = "Thr > 0";
            chkFilterThr.UseVisualStyleBackColor = true;
            // 
            // gbTrainingLog
            // 
            gbTrainingLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            gbTrainingLog.Controls.Add(rtbTrainLog);
            gbTrainingLog.ForeColor = Color.White;
            gbTrainingLog.Location = new Point(1374, 973);
            gbTrainingLog.Margin = new Padding(6);
            gbTrainingLog.Name = "gbTrainingLog";
            gbTrainingLog.Padding = new Padding(6);
            gbTrainingLog.Size = new Size(616, 267);
            gbTrainingLog.TabIndex = 11;
            gbTrainingLog.TabStop = false;
            gbTrainingLog.Text = "TRAINING LOG";
            // 
            // rtbTrainLog
            // 
            rtbTrainLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbTrainLog.Location = new Point(12, 46);
            rtbTrainLog.Margin = new Padding(6);
            rtbTrainLog.Name = "rtbTrainLog";
            rtbTrainLog.Size = new Size(587, 202);
            rtbTrainLog.TabIndex = 1;
            rtbTrainLog.Text = "";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe Script", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.Red;
            lblTitle.Location = new Point(48, 19);
            lblTitle.Margin = new Padding(6, 0, 6, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(501, 73);
            lblTitle.TabIndex = 12;
            lblTitle.Text = "Team 05 Donkey UI";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(2014, 1288);
            Controls.Add(lblTitle);
            Controls.Add(gbTrainingLog);
            Controls.Add(gbDataManagement);
            Controls.Add(gbSystemOps);
            Controls.Add(gbFrameList);
            Controls.Add(gbNavigation);
            Controls.Add(gbCamView);
            ForeColor = SystemColors.ControlText;
            Margin = new Padding(6);
            MinimumSize = new Size(2023, 1305);
            Name = "Form1";
            Text = "MoveArt Donkeycar Data Manager";
            ((System.ComponentModel.ISupportInitialize)pbMainCam).EndInit();
            gbCamView.ResumeLayout(false);
            gbCamView.PerformLayout();
            gbNavigation.ResumeLayout(false);
            gbNavigation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameSlider).EndInit();
            gbFrameList.ResumeLayout(false);
            gbSystemOps.ResumeLayout(false);
            gbDataManagement.ResumeLayout(false);
            gbDataManagement.PerformLayout();
            gbFilterOptions.ResumeLayout(false);
            gbFilterOptions.PerformLayout();
            gbTrainingLog.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbMainCam;
        private Label lblThrottleBottom;
        private Label lblFrameIndex;
        private Label lblAngle;
        private Label lblThrottleTop;
        private Label lblTimestamp;
        private GroupBox gbCamView; // 변수 타입들 모두 GroupBox로 변경
        private GroupBox gbNavigation;
        private TrackBar trkFrameSlider;
        private Button btnStop;
        private GroupBox gbFrameList;
        private ListBox lstFrameData;
        private GroupBox gbSystemOps;
        private GroupBox gbDataManagement;
        private GroupBox gbFilterOptions;
        private GroupBox gbTrainingLog;
        private Label lblTitle;
        private Button btnFastForward;
        private Button btnFastRewind;
        private Button btnNext;
        private Button btnPrev;
        private Button btnPlay;
        private Button btnLoadConfig;
        private Button btnStartTraining;
        private Button btnLoadTub;
        private CheckBox chkFilterLargeAngle;
        private CheckBox chkFilterAngleZero;
        private CheckBox chkFilterThr;
        private Button btnApplyFilter;
        private Button btnSetLeft;
        private Button btnDeleteData;
        private Button btnSetRight;
        private Button btnSpeed;
        private Label lblSetRange;
        private RichTextBox rtbTrainLog;
    }
}