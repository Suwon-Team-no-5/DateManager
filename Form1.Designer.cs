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
            pbMainCam.Location = new Point(8, 67);
            pbMainCam.Margin = new Padding(4, 4, 4, 4);
            pbMainCam.Name = "pbMainCam";
            pbMainCam.Size = new Size(829, 455);
            pbMainCam.TabIndex = 0;
            pbMainCam.TabStop = false;
            // 
            // lblThrottleBottom
            // 
            lblThrottleBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblThrottleBottom.AutoSize = true;
            lblThrottleBottom.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblThrottleBottom.ForeColor = Color.White;
            lblThrottleBottom.Location = new Point(627, 475);
            lblThrottleBottom.Margin = new Padding(4, 0, 4, 0);
            lblThrottleBottom.Name = "lblThrottleBottom";
            lblThrottleBottom.Size = new Size(143, 28);
            lblThrottleBottom.TabIndex = 1;
            lblThrottleBottom.Text = "Throttle: +0.0";
            // 
            // lblFrameIndex
            // 
            lblFrameIndex.AutoSize = true;
            lblFrameIndex.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblFrameIndex.ForeColor = Color.White;
            lblFrameIndex.Location = new Point(17, 31);
            lblFrameIndex.Margin = new Padding(4, 0, 4, 0);
            lblFrameIndex.Name = "lblFrameIndex";
            lblFrameIndex.Size = new Size(127, 20);
            lblFrameIndex.TabIndex = 2;
            lblFrameIndex.Text = "Frame Index 0/0";
            // 
            // lblAngle
            // 
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblAngle.ForeColor = Color.White;
            lblAngle.Location = new Point(264, 31);
            lblAngle.Margin = new Padding(4, 0, 4, 0);
            lblAngle.Name = "lblAngle";
            lblAngle.Size = new Size(92, 20);
            lblAngle.TabIndex = 3;
            lblAngle.Text = "Angle: +0.0";
            // 
            // lblThrottleTop
            // 
            lblThrottleTop.AutoSize = true;
            lblThrottleTop.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblThrottleTop.ForeColor = Color.White;
            lblThrottleTop.Location = new Point(456, 31);
            lblThrottleTop.Margin = new Padding(4, 0, 4, 0);
            lblThrottleTop.Name = "lblThrottleTop";
            lblThrottleTop.Size = new Size(108, 20);
            lblThrottleTop.TabIndex = 4;
            lblThrottleTop.Text = "Throttle: +0.0";
            // 
            // lblTimestamp
            // 
            lblTimestamp.AutoSize = true;
            lblTimestamp.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lblTimestamp.ForeColor = Color.White;
            lblTimestamp.Location = new Point(649, 31);
            lblTimestamp.Margin = new Padding(4, 0, 4, 0);
            lblTimestamp.Name = "lblTimestamp";
            lblTimestamp.Size = new Size(88, 20);
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
            gbCamView.Location = new Point(31, 71);
            gbCamView.Margin = new Padding(4, 4, 4, 4);
            gbCamView.Name = "gbCamView";
            gbCamView.Padding = new Padding(4, 4, 4, 4);
            gbCamView.Size = new Size(845, 529);
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
            gbNavigation.Location = new Point(31, 608);
            gbNavigation.Margin = new Padding(4, 4, 4, 4);
            gbNavigation.Name = "gbNavigation";
            gbNavigation.Padding = new Padding(4, 4, 4, 4);
            gbNavigation.Size = new Size(845, 167);
            gbNavigation.TabIndex = 7;
            gbNavigation.TabStop = false;
            gbNavigation.Text = "DATAFRAME NAVIGATION";
            // 
            // btnSpeed
            // 
            btnSpeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSpeed.BackColor = Color.Silver;
            btnSpeed.ForeColor = Color.Black;
            btnSpeed.Location = new Point(456, 109);
            btnSpeed.Margin = new Padding(4, 4, 4, 4);
            btnSpeed.Name = "btnSpeed";
            btnSpeed.Size = new Size(121, 39);
            btnSpeed.TabIndex = 7;
            btnSpeed.Text = "1.0";
            btnSpeed.UseVisualStyleBackColor = false;
            // 
            // btnFastForward
            // 
            btnFastForward.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFastForward.BackColor = Color.Silver;
            btnFastForward.ForeColor = Color.Black;
            btnFastForward.Location = new Point(716, 109);
            btnFastForward.Margin = new Padding(4, 4, 4, 4);
            btnFastForward.Name = "btnFastForward";
            btnFastForward.Size = new Size(121, 39);
            btnFastForward.TabIndex = 6;
            btnFastForward.Text = ">>>";
            btnFastForward.UseVisualStyleBackColor = false;
            // 
            // btnFastRewind
            // 
            btnFastRewind.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFastRewind.BackColor = Color.Silver;
            btnFastRewind.ForeColor = Color.Black;
            btnFastRewind.Location = new Point(588, 109);
            btnFastRewind.Margin = new Padding(4, 4, 4, 4);
            btnFastRewind.Name = "btnFastRewind";
            btnFastRewind.Size = new Size(121, 39);
            btnFastRewind.TabIndex = 5;
            btnFastRewind.Text = "<<<";
            btnFastRewind.UseVisualStyleBackColor = false;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.BackColor = Color.Silver;
            btnNext.ForeColor = Color.Black;
            btnNext.Location = new Point(716, 63);
            btnNext.Margin = new Padding(4, 4, 4, 4);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(121, 39);
            btnNext.TabIndex = 4;
            btnNext.Text = ">";
            btnNext.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.BackColor = Color.Silver;
            btnPrev.ForeColor = Color.Black;
            btnPrev.Location = new Point(588, 63);
            btnPrev.Margin = new Padding(4, 4, 4, 4);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(121, 39);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "<";
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnPlay
            // 
            btnPlay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPlay.BackColor = Color.Silver;
            btnPlay.ForeColor = Color.Black;
            btnPlay.Location = new Point(588, 16);
            btnPlay.Margin = new Padding(4, 4, 4, 4);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(121, 39);
            btnPlay.TabIndex = 2;
            btnPlay.Text = "Play";
            btnPlay.UseVisualStyleBackColor = false;
            // 
            // trkFrameSlider
            // 
            trkFrameSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkFrameSlider.Location = new Point(4, 31);
            trkFrameSlider.Margin = new Padding(4, 4, 4, 4);
            trkFrameSlider.Name = "trkFrameSlider";
            trkFrameSlider.Size = new Size(573, 56);
            trkFrameSlider.TabIndex = 1;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStop.BackColor = Color.Silver;
            btnStop.ForeColor = Color.Black;
            btnStop.Location = new Point(716, 16);
            btnStop.Margin = new Padding(4, 4, 4, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(121, 39);
            btnStop.TabIndex = 0;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            // 
            // gbFrameList
            // 
            gbFrameList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            gbFrameList.Controls.Add(lstFrameData);
            gbFrameList.ForeColor = Color.White;
            gbFrameList.Location = new Point(883, 71);
            gbFrameList.Margin = new Padding(4, 4, 4, 4);
            gbFrameList.Name = "gbFrameList";
            gbFrameList.Padding = new Padding(4, 4, 4, 4);
            gbFrameList.Size = new Size(189, 315);
            gbFrameList.TabIndex = 8;
            gbFrameList.TabStop = false;
            gbFrameList.Text = "FRAME LIST";
            // 
            // lstFrameData
            // 
            lstFrameData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFrameData.FormattingEnabled = true;
            lstFrameData.Location = new Point(8, 29);
            lstFrameData.Margin = new Padding(4, 4, 4, 4);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.Size = new Size(172, 264);
            lstFrameData.TabIndex = 0;
            // 
            // gbSystemOps
            // 
            gbSystemOps.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            gbSystemOps.Controls.Add(btnStartTraining);
            gbSystemOps.Controls.Add(btnLoadTub);
            gbSystemOps.Controls.Add(btnLoadConfig);
            gbSystemOps.ForeColor = Color.White;
            gbSystemOps.Location = new Point(1080, 71);
            gbSystemOps.Margin = new Padding(4, 4, 4, 4);
            gbSystemOps.Name = "gbSystemOps";
            gbSystemOps.Padding = new Padding(4, 4, 4, 4);
            gbSystemOps.Size = new Size(199, 315);
            gbSystemOps.TabIndex = 9;
            gbSystemOps.TabStop = false;
            gbSystemOps.Text = "SYSTEM OPERATIONS";
            // 
            // btnStartTraining
            // 
            btnStartTraining.BackColor = Color.MediumSeaGreen;
            btnStartTraining.Font = new Font("맑은 고딕", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnStartTraining.ForeColor = Color.Yellow;
            btnStartTraining.Location = new Point(8, 155);
            btnStartTraining.Margin = new Padding(4, 4, 4, 4);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(184, 148);
            btnStartTraining.TabIndex = 2;
            btnStartTraining.Text = "Start AI Model\r\nTraining";
            btnStartTraining.UseVisualStyleBackColor = false;
            // 
            // btnLoadTub
            // 
            btnLoadTub.BackColor = Color.CornflowerBlue;
            btnLoadTub.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnLoadTub.ForeColor = Color.White;
            btnLoadTub.Location = new Point(8, 83);
            btnLoadTub.Margin = new Padding(4, 4, 4, 4);
            btnLoadTub.Name = "btnLoadTub";
            btnLoadTub.Size = new Size(184, 64);
            btnLoadTub.TabIndex = 1;
            btnLoadTub.Text = "Load tub";
            btnLoadTub.UseVisualStyleBackColor = false;
            // 
            // btnLoadConfig
            // 
            btnLoadConfig.BackColor = Color.CornflowerBlue;
            btnLoadConfig.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnLoadConfig.ForeColor = Color.White;
            btnLoadConfig.Location = new Point(8, 29);
            btnLoadConfig.Margin = new Padding(4, 4, 4, 4);
            btnLoadConfig.Name = "btnLoadConfig";
            btnLoadConfig.Size = new Size(184, 45);
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
            gbDataManagement.Location = new Point(883, 393);
            gbDataManagement.Margin = new Padding(4, 4, 4, 4);
            gbDataManagement.Name = "gbDataManagement";
            gbDataManagement.Padding = new Padding(4, 4, 4, 4);
            gbDataManagement.Size = new Size(396, 207);
            gbDataManagement.TabIndex = 10;
            gbDataManagement.TabStop = false;
            gbDataManagement.Text = "DATA MANAGEMENT && FILTERING";
            // 
            // lblSetRange
            // 
            lblSetRange.AutoSize = true;
            lblSetRange.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point, 129);
            lblSetRange.ForeColor = Color.White;
            lblSetRange.Location = new Point(258, 68);
            lblSetRange.Margin = new Padding(4, 0, 4, 0);
            lblSetRange.Name = "lblSetRange";
            lblSetRange.Size = new Size(57, 28);
            lblSetRange.TabIndex = 8;
            lblSetRange.Text = "(0, 0)";
            // 
            // btnSetLeft
            // 
            btnSetLeft.BackColor = Color.Silver;
            btnSetLeft.ForeColor = Color.Black;
            btnSetLeft.Location = new Point(197, 105);
            btnSetLeft.Margin = new Padding(4, 4, 4, 4);
            btnSetLeft.Name = "btnSetLeft";
            btnSetLeft.Size = new Size(86, 39);
            btnSetLeft.TabIndex = 7;
            btnSetLeft.Text = "Set L";
            btnSetLeft.UseVisualStyleBackColor = false;
            // 
            // btnDeleteData
            // 
            btnDeleteData.BackColor = Color.Red;
            btnDeleteData.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnDeleteData.ForeColor = Color.White;
            btnDeleteData.Location = new Point(197, 152);
            btnDeleteData.Margin = new Padding(4, 4, 4, 4);
            btnDeleteData.Name = "btnDeleteData";
            btnDeleteData.Size = new Size(177, 39);
            btnDeleteData.TabIndex = 6;
            btnDeleteData.Text = "Delete";
            btnDeleteData.UseVisualStyleBackColor = false;
            // 
            // btnSetRight
            // 
            btnSetRight.BackColor = Color.Silver;
            btnSetRight.ForeColor = Color.Black;
            btnSetRight.Location = new Point(288, 105);
            btnSetRight.Margin = new Padding(4, 4, 4, 4);
            btnSetRight.Name = "btnSetRight";
            btnSetRight.Size = new Size(86, 39);
            btnSetRight.TabIndex = 5;
            btnSetRight.Text = "Set R";
            btnSetRight.UseVisualStyleBackColor = false;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.BackColor = Color.CornflowerBlue;
            btnApplyFilter.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            btnApplyFilter.ForeColor = Color.White;
            btnApplyFilter.Location = new Point(197, 20);
            btnApplyFilter.Margin = new Padding(4, 4, 4, 4);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(177, 39);
            btnApplyFilter.TabIndex = 3;
            btnApplyFilter.Text = "Apply Filter";
            btnApplyFilter.UseVisualStyleBackColor = false;
            // 
            // gbFilterOptions
            // 
            gbFilterOptions.Controls.Add(chkFilterLargeAngle);
            gbFilterOptions.Controls.Add(chkFilterAngleZero);
            gbFilterOptions.Controls.Add(chkFilterThr);
            gbFilterOptions.ForeColor = Color.White;
            gbFilterOptions.Location = new Point(8, 29);
            gbFilterOptions.Margin = new Padding(4, 4, 4, 4);
            gbFilterOptions.Name = "gbFilterOptions";
            gbFilterOptions.Padding = new Padding(4, 4, 4, 4);
            gbFilterOptions.Size = new Size(181, 161);
            gbFilterOptions.TabIndex = 0;
            gbFilterOptions.TabStop = false;
            gbFilterOptions.Text = "Filter Options";
            // 
            // chkFilterLargeAngle
            // 
            chkFilterLargeAngle.AutoSize = true;
            chkFilterLargeAngle.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterLargeAngle.ForeColor = Color.White;
            chkFilterLargeAngle.Location = new Point(8, 105);
            chkFilterLargeAngle.Margin = new Padding(4, 4, 4, 4);
            chkFilterLargeAngle.Name = "chkFilterLargeAngle";
            chkFilterLargeAngle.Size = new Size(113, 24);
            chkFilterLargeAngle.TabIndex = 2;
            chkFilterLargeAngle.Text = "Large Angle";
            chkFilterLargeAngle.UseVisualStyleBackColor = true;
            // 
            // chkFilterAngleZero
            // 
            chkFilterAngleZero.AutoSize = true;
            chkFilterAngleZero.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterAngleZero.ForeColor = Color.White;
            chkFilterAngleZero.Location = new Point(8, 72);
            chkFilterAngleZero.Margin = new Padding(4, 4, 4, 4);
            chkFilterAngleZero.Name = "chkFilterAngleZero";
            chkFilterAngleZero.Size = new Size(111, 24);
            chkFilterAngleZero.TabIndex = 1;
            chkFilterAngleZero.Text = "Angle == 0";
            chkFilterAngleZero.UseVisualStyleBackColor = true;
            // 
            // chkFilterThr
            // 
            chkFilterThr.AutoSize = true;
            chkFilterThr.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point, 129);
            chkFilterThr.ForeColor = Color.White;
            chkFilterThr.Location = new Point(8, 39);
            chkFilterThr.Margin = new Padding(4, 4, 4, 4);
            chkFilterThr.Name = "chkFilterThr";
            chkFilterThr.Size = new Size(82, 24);
            chkFilterThr.TabIndex = 0;
            chkFilterThr.Text = "Thr > 0";
            chkFilterThr.UseVisualStyleBackColor = true;
            // 
            // gbTrainingLog
            // 
            gbTrainingLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            gbTrainingLog.Controls.Add(rtbTrainLog);
            gbTrainingLog.ForeColor = Color.White;
            gbTrainingLog.Location = new Point(883, 608);
            gbTrainingLog.Margin = new Padding(4, 4, 4, 4);
            gbTrainingLog.Name = "gbTrainingLog";
            gbTrainingLog.Padding = new Padding(4, 4, 4, 4);
            gbTrainingLog.Size = new Size(396, 167);
            gbTrainingLog.TabIndex = 11;
            gbTrainingLog.TabStop = false;
            gbTrainingLog.Text = "TRAINING LOG";
            // 
            // rtbTrainLog
            // 
            rtbTrainLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbTrainLog.Location = new Point(8, 29);
            rtbTrainLog.Margin = new Padding(4, 4, 4, 4);
            rtbTrainLog.Name = "rtbTrainLog";
            rtbTrainLog.Size = new Size(379, 128);
            rtbTrainLog.TabIndex = 1;
            rtbTrainLog.Text = "";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe Script", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.Red;
            lblTitle.Location = new Point(31, 12);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(316, 46);
            lblTitle.TabIndex = 12;
            lblTitle.Text = "Team 05 Donkey UI";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(1295, 805);
            Controls.Add(lblTitle);
            Controls.Add(gbTrainingLog);
            Controls.Add(gbDataManagement);
            Controls.Add(gbSystemOps);
            Controls.Add(gbFrameList);
            Controls.Add(gbNavigation);
            Controls.Add(gbCamView);
            ForeColor = SystemColors.ControlText;
            Margin = new Padding(4, 4, 4, 4);
            MinimumSize = new Size(1310, 842);
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