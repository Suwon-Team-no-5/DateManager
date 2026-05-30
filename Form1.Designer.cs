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
        /// <param name="disposing">true if managed resources disposed; otherwise, false.</param>
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
            lblFrameIndex = new Label();
            lblAngle = new Label();
            lblThrottleTop = new Label();
            lblTimestamp = new Label();
            pnlCamView = new Panel();
            lblCamViewTitle = new Label();
            btnViewLog = new Button();
            btnViewMonitor = new Button();
            pnlNavigation = new Panel();
            lblNavTitle = new Label();
            btnSpeed = new Button();
            btnPrev = new Button();
            btnFastRewind = new Button();
            btnPlay = new Button();
            btnStop = new Button();
            btnFastForward = new Button();
            btnNext = new Button();
            trkFrameSlider = new TrackBar();
            pnlFrameList = new Panel();
            lblFrameListTitle = new Label();
            lstFrameData = new ListBox();
            pnlSystemOps = new Panel();
            btnRestartTraining = new Button();
            btnStopTraining = new Button();
            btnEndTraining = new Button();
            lblSystemOpsTitle = new Label();
            btnStartTraining = new Button();
            btnLoadTub = new Button();
            pnlDataManagement = new Panel();
            lblDataMgmtTitle = new Label();
            btnApplyFilter = new Button();
            pnlFilterOptions = new Panel();
            lblFilterTitle = new Label();
            chkFilterLargeAngle = new CheckBox();
            chkFilterLargeThr = new CheckBox();
            chkFilterThr = new CheckBox();
            btnDeleteData = new Button();
            pnlTrainingLog = new Panel();
            lblLogTitle = new Label();
            rtbTrainLog = new RichTextBox();
            lblTitle = new Label();
            panel1 = new Panel();
            btnSelectAll = new Button();
            btnRestoreData = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pbMainCam).BeginInit();
            pnlCamView.SuspendLayout();
            pnlNavigation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameSlider).BeginInit();
            pnlFrameList.SuspendLayout();
            pnlSystemOps.SuspendLayout();
            pnlDataManagement.SuspendLayout();
            pnlFilterOptions.SuspendLayout();
            pnlTrainingLog.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pbMainCam
            // 
            pbMainCam.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbMainCam.BackColor = Color.FromArgb(30, 30, 30);
            pbMainCam.Location = new Point(22, 128);
            pbMainCam.Margin = new Padding(6);
            pbMainCam.Name = "pbMainCam";
            pbMainCam.Size = new Size(1266, 691);
            pbMainCam.TabIndex = 0;
            pbMainCam.TabStop = false;
            // 
            // lblFrameIndex
            // 
            lblFrameIndex.AutoSize = true;
            lblFrameIndex.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblFrameIndex.ForeColor = Color.White;
            lblFrameIndex.Location = new Point(22, 72);
            lblFrameIndex.Margin = new Padding(5, 0, 5, 0);
            lblFrameIndex.Name = "lblFrameIndex";
            lblFrameIndex.Size = new Size(207, 37);
            lblFrameIndex.TabIndex = 2;
            lblFrameIndex.Text = "프레임 번호 0/0";
            // 
            // lblAngle
            // 
            lblAngle.AutoSize = true;
            lblAngle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblAngle.ForeColor = Color.White;
            lblAngle.Location = new Point(516, 72);
            lblAngle.Margin = new Padding(5, 0, 5, 0);
            lblAngle.Name = "lblAngle";
            lblAngle.Size = new Size(141, 37);
            lblAngle.TabIndex = 3;
            lblAngle.Text = "방향: +0.0";
            // 
            // lblThrottleTop
            // 
            lblThrottleTop.AutoSize = true;
            lblThrottleTop.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblThrottleTop.ForeColor = Color.White;
            lblThrottleTop.Location = new Point(806, 72);
            lblThrottleTop.Margin = new Padding(5, 0, 5, 0);
            lblThrottleTop.Name = "lblThrottleTop";
            lblThrottleTop.Size = new Size(141, 37);
            lblThrottleTop.TabIndex = 4;
            lblThrottleTop.Text = "속도: +0.0";
            // 
            // lblTimestamp
            // 
            lblTimestamp.AutoSize = true;
            lblTimestamp.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTimestamp.ForeColor = Color.DarkGray;
            lblTimestamp.Location = new Point(1052, 72);
            lblTimestamp.Margin = new Padding(5, 0, 5, 0);
            lblTimestamp.Name = "lblTimestamp";
            lblTimestamp.Size = new Size(132, 37);
            lblTimestamp.TabIndex = 5;
            lblTimestamp.Text = "기록 시간";
            // 
            // pnlCamView
            // 
            pnlCamView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlCamView.BackColor = Color.FromArgb(45, 45, 48);
            pnlCamView.Controls.Add(lblCamViewTitle);
            pnlCamView.Controls.Add(pbMainCam);
            pnlCamView.Controls.Add(lblTimestamp);
            pnlCamView.Controls.Add(lblFrameIndex);
            pnlCamView.Controls.Add(lblThrottleTop);
            pnlCamView.Controls.Add(lblAngle);
            pnlCamView.Location = new Point(48, 114);
            pnlCamView.Margin = new Padding(5);
            pnlCamView.Name = "pnlCamView";
            pnlCamView.Size = new Size(1314, 846);
            pnlCamView.TabIndex = 6;
            // 
            // lblCamViewTitle
            // 
            lblCamViewTitle.AutoSize = true;
            lblCamViewTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblCamViewTitle.ForeColor = Color.DeepSkyBlue;
            lblCamViewTitle.Location = new Point(22, 19);
            lblCamViewTitle.Margin = new Padding(5, 0, 5, 0);
            lblCamViewTitle.Name = "lblCamViewTitle";
            lblCamViewTitle.Size = new Size(165, 38);
            lblCamViewTitle.TabIndex = 6;
            lblCamViewTitle.Text = "주행 모니터";
            // 
            // btnViewLog
            // 
            btnViewLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
            btnViewLog.FlatAppearance.BorderSize = 0;
            btnViewLog.FlatStyle = FlatStyle.Flat;
            btnViewLog.Font = new Font("Segoe UI Semibold", 9F);
            btnViewLog.ForeColor = Color.White;
            btnViewLog.Location = new Point(1188, 53);
            btnViewLog.Margin = new Padding(5);
            btnViewLog.Name = "btnViewLog";
            btnViewLog.Size = new Size(174, 51);
            btnViewLog.TabIndex = 8;
            btnViewLog.Text = "📝 학습화면";
            btnViewLog.UseVisualStyleBackColor = false;
            btnViewLog.Click += btnViewLog_Click;
            // 
            // btnViewMonitor
            // 
            btnViewMonitor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnViewMonitor.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.FlatAppearance.BorderSize = 0;
            btnViewMonitor.FlatStyle = FlatStyle.Flat;
            btnViewMonitor.Font = new Font("Segoe UI Semibold", 9F);
            btnViewMonitor.ForeColor = Color.White;
            btnViewMonitor.Location = new Point(1014, 53);
            btnViewMonitor.Margin = new Padding(5);
            btnViewMonitor.Name = "btnViewMonitor";
            btnViewMonitor.Size = new Size(165, 51);
            btnViewMonitor.TabIndex = 7;
            btnViewMonitor.Text = "📊 모니터";
            btnViewMonitor.UseVisualStyleBackColor = false;
            btnViewMonitor.Click += btnViewMonitor_Click;
            // 
            // pnlNavigation
            // 
            pnlNavigation.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlNavigation.BackColor = Color.FromArgb(45, 45, 48);
            pnlNavigation.Controls.Add(lblNavTitle);
            pnlNavigation.Controls.Add(btnSpeed);
            pnlNavigation.Controls.Add(btnPrev);
            pnlNavigation.Controls.Add(btnFastRewind);
            pnlNavigation.Controls.Add(btnPlay);
            pnlNavigation.Controls.Add(btnStop);
            pnlNavigation.Controls.Add(btnFastForward);
            pnlNavigation.Controls.Add(btnNext);
            pnlNavigation.Controls.Add(trkFrameSlider);
            pnlNavigation.Location = new Point(48, 984);
            pnlNavigation.Margin = new Padding(5);
            pnlNavigation.Name = "pnlNavigation";
            pnlNavigation.Size = new Size(1314, 256);
            pnlNavigation.TabIndex = 7;
            // 
            // lblNavTitle
            // 
            lblNavTitle.AutoSize = true;
            lblNavTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblNavTitle.ForeColor = Color.DeepSkyBlue;
            lblNavTitle.Location = new Point(22, 19);
            lblNavTitle.Margin = new Padding(5, 0, 5, 0);
            lblNavTitle.Name = "lblNavTitle";
            lblNavTitle.Size = new Size(341, 38);
            lblNavTitle.TabIndex = 8;
            lblNavTitle.Text = "데이터프레임 탐색 컨트롤";
            // 
            // btnSpeed
            // 
            btnSpeed.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSpeed.BackColor = Color.FromArgb(62, 62, 66);
            btnSpeed.FlatAppearance.BorderSize = 0;
            btnSpeed.FlatStyle = FlatStyle.Flat;
            btnSpeed.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSpeed.ForeColor = Color.White;
            btnSpeed.Location = new Point(22, 168);
            btnSpeed.Margin = new Padding(5);
            btnSpeed.Name = "btnSpeed";
            btnSpeed.Size = new Size(187, 67);
            btnSpeed.TabIndex = 7;
            btnSpeed.Text = "배속 x 1.0";
            btnSpeed.UseVisualStyleBackColor = false;
            btnSpeed.Click += btnSpeed_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPrev.BackColor = Color.FromArgb(62, 62, 66);
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Segoe UI Emoji", 12F);
            btnPrev.ForeColor = Color.White;
            btnPrev.Location = new Point(218, 168);
            btnPrev.Margin = new Padding(5);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(156, 67);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "⏮";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += btnPrev_Click;
            // 
            // btnFastRewind
            // 
            btnFastRewind.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnFastRewind.BackColor = Color.FromArgb(62, 62, 66);
            btnFastRewind.FlatAppearance.BorderSize = 0;
            btnFastRewind.FlatStyle = FlatStyle.Flat;
            btnFastRewind.Font = new Font("Segoe UI Emoji", 12F);
            btnFastRewind.ForeColor = Color.White;
            btnFastRewind.Location = new Point(392, 168);
            btnFastRewind.Margin = new Padding(5);
            btnFastRewind.Name = "btnFastRewind";
            btnFastRewind.Size = new Size(156, 67);
            btnFastRewind.TabIndex = 5;
            btnFastRewind.Text = "⏪";
            btnFastRewind.UseVisualStyleBackColor = false;
            btnFastRewind.Click += btnFastRewind_Click;
            // 
            // btnPlay
            // 
            btnPlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPlay.BackColor = Color.FromArgb(0, 122, 204);
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnPlay.ForeColor = Color.White;
            btnPlay.Location = new Point(566, 168);
            btnPlay.Margin = new Padding(5);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(156, 67);
            btnPlay.TabIndex = 2;
            btnPlay.Text = "▶ 재생";
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnStop.BackColor = Color.FromArgb(211, 47, 47);
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            btnStop.ForeColor = Color.White;
            btnStop.Location = new Point(740, 168);
            btnStop.Margin = new Padding(5);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(156, 67);
            btnStop.TabIndex = 0;
            btnStop.Text = "⏹ 초기화";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnFastForward
            // 
            btnFastForward.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnFastForward.BackColor = Color.FromArgb(62, 62, 66);
            btnFastForward.FlatAppearance.BorderSize = 0;
            btnFastForward.FlatStyle = FlatStyle.Flat;
            btnFastForward.Font = new Font("Segoe UI Emoji", 12F);
            btnFastForward.ForeColor = Color.White;
            btnFastForward.Location = new Point(915, 168);
            btnFastForward.Margin = new Padding(5);
            btnFastForward.Name = "btnFastForward";
            btnFastForward.Size = new Size(156, 67);
            btnFastForward.TabIndex = 6;
            btnFastForward.Text = "⏩";
            btnFastForward.UseVisualStyleBackColor = false;
            btnFastForward.Click += btnFastForward_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnNext.BackColor = Color.FromArgb(62, 62, 66);
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI Emoji", 12F);
            btnNext.ForeColor = Color.White;
            btnNext.Location = new Point(1089, 168);
            btnNext.Margin = new Padding(5);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(156, 67);
            btnNext.TabIndex = 4;
            btnNext.Text = "⏭";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // trkFrameSlider
            // 
            trkFrameSlider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkFrameSlider.Location = new Point(22, 77);
            trkFrameSlider.Margin = new Padding(5);
            trkFrameSlider.Name = "trkFrameSlider";
            trkFrameSlider.Size = new Size(1266, 90);
            trkFrameSlider.TabIndex = 1;
            trkFrameSlider.Scroll += trkFrameSlider_Scroll;
            // 
            // pnlFrameList
            // 
            pnlFrameList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pnlFrameList.BackColor = Color.FromArgb(45, 45, 48);
            pnlFrameList.Controls.Add(lblFrameListTitle);
            pnlFrameList.Controls.Add(lstFrameData);
            pnlFrameList.Location = new Point(1389, 114);
            pnlFrameList.Margin = new Padding(5);
            pnlFrameList.Name = "pnlFrameList";
            pnlFrameList.Size = new Size(294, 504);
            pnlFrameList.TabIndex = 8;
            // 
            // lblFrameListTitle
            // 
            lblFrameListTitle.AutoSize = true;
            lblFrameListTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblFrameListTitle.ForeColor = Color.DeepSkyBlue;
            lblFrameListTitle.Location = new Point(17, 19);
            lblFrameListTitle.Margin = new Padding(5, 0, 5, 0);
            lblFrameListTitle.Name = "lblFrameListTitle";
            lblFrameListTitle.Size = new Size(165, 38);
            lblFrameListTitle.TabIndex = 7;
            lblFrameListTitle.Text = "프레임 목록";
            // 
            // lstFrameData
            // 
            lstFrameData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFrameData.BackColor = Color.FromArgb(30, 30, 30);
            lstFrameData.BorderStyle = BorderStyle.None;
            lstFrameData.ForeColor = Color.White;
            lstFrameData.FormattingEnabled = true;
            lstFrameData.Location = new Point(22, 72);
            lstFrameData.Margin = new Padding(5);
            lstFrameData.Name = "lstFrameData";
            lstFrameData.SelectionMode = SelectionMode.MultiExtended;
            lstFrameData.Size = new Size(250, 384);
            lstFrameData.TabIndex = 0;
            lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            // 
            // pnlSystemOps
            // 
            pnlSystemOps.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlSystemOps.BackColor = Color.FromArgb(45, 45, 48);
            pnlSystemOps.Controls.Add(btnRestartTraining);
            pnlSystemOps.Controls.Add(btnStopTraining);
            pnlSystemOps.Controls.Add(btnEndTraining);
            pnlSystemOps.Controls.Add(lblSystemOpsTitle);
            pnlSystemOps.Controls.Add(btnStartTraining);
            pnlSystemOps.Controls.Add(btnLoadTub);
            pnlSystemOps.Location = new Point(1705, 114);
            pnlSystemOps.Margin = new Padding(5);
            pnlSystemOps.Name = "pnlSystemOps";
            pnlSystemOps.Size = new Size(272, 504);
            pnlSystemOps.TabIndex = 9;
            // 
            // btnRestartTraining
            // 
            btnRestartTraining.BackColor = Color.FromArgb(0, 122, 204);
            btnRestartTraining.FlatAppearance.BorderSize = 0;
            btnRestartTraining.FlatStyle = FlatStyle.Flat;
            btnRestartTraining.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRestartTraining.ForeColor = Color.White;
            btnRestartTraining.Location = new Point(22, 170);
            btnRestartTraining.Margin = new Padding(5);
            btnRestartTraining.Name = "btnRestartTraining";
            btnRestartTraining.Size = new Size(229, 194);
            btnRestartTraining.TabIndex = 11;
            btnRestartTraining.Text = "AI 학습\r\n 재시작";
            btnRestartTraining.UseVisualStyleBackColor = false;
            btnRestartTraining.Visible = false;
            btnRestartTraining.Click += btnRestartTraining_Click;
            // 
            // btnStopTraining
            // 
            btnStopTraining.BackColor = Color.FromArgb(255, 160, 0);
            btnStopTraining.FlatAppearance.BorderSize = 0;
            btnStopTraining.FlatStyle = FlatStyle.Flat;
            btnStopTraining.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStopTraining.ForeColor = Color.White;
            btnStopTraining.Location = new Point(22, 170);
            btnStopTraining.Margin = new Padding(5);
            btnStopTraining.Name = "btnStopTraining";
            btnStopTraining.Size = new Size(229, 194);
            btnStopTraining.TabIndex = 10;
            btnStopTraining.Text = "AI 학습\r\n 정지";
            btnStopTraining.UseVisualStyleBackColor = false;
            btnStopTraining.Visible = false;
            btnStopTraining.Click += btnStopTraining_Click;
            // 
            // btnEndTraining
            // 
            btnEndTraining.BackColor = Color.FromArgb(211, 47, 47);
            btnEndTraining.FlatAppearance.BorderSize = 0;
            btnEndTraining.FlatStyle = FlatStyle.Flat;
            btnEndTraining.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnEndTraining.ForeColor = Color.White;
            btnEndTraining.Location = new Point(22, 373);
            btnEndTraining.Margin = new Padding(5);
            btnEndTraining.Name = "btnEndTraining";
            btnEndTraining.Size = new Size(229, 99);
            btnEndTraining.TabIndex = 9;
            btnEndTraining.Text = "AI 학습\r\n 종료";
            btnEndTraining.UseVisualStyleBackColor = false;
            btnEndTraining.Click += btnEndTraining_Click;
            // 
            // lblSystemOpsTitle
            // 
            lblSystemOpsTitle.AutoSize = true;
            lblSystemOpsTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblSystemOpsTitle.ForeColor = Color.DeepSkyBlue;
            lblSystemOpsTitle.Location = new Point(17, 19);
            lblSystemOpsTitle.Margin = new Padding(5, 0, 5, 0);
            lblSystemOpsTitle.Name = "lblSystemOpsTitle";
            lblSystemOpsTitle.Size = new Size(165, 38);
            lblSystemOpsTitle.TabIndex = 8;
            lblSystemOpsTitle.Text = "데이터 로드";
            // 
            // btnStartTraining
            // 
            btnStartTraining.BackColor = Color.FromArgb(0, 150, 136);
            btnStartTraining.FlatAppearance.BorderSize = 0;
            btnStartTraining.FlatStyle = FlatStyle.Flat;
            btnStartTraining.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStartTraining.ForeColor = Color.White;
            btnStartTraining.Location = new Point(22, 170);
            btnStartTraining.Margin = new Padding(5);
            btnStartTraining.Name = "btnStartTraining";
            btnStartTraining.Size = new Size(229, 194);
            btnStartTraining.TabIndex = 2;
            btnStartTraining.Text = "AI 학습\r\n시작";
            btnStartTraining.UseVisualStyleBackColor = false;
            btnStartTraining.Click += btnStart_Click;
            // 
            // btnLoadTub
            // 
            btnLoadTub.BackColor = Color.FromArgb(62, 62, 66);
            btnLoadTub.FlatAppearance.BorderSize = 0;
            btnLoadTub.FlatStyle = FlatStyle.Flat;
            btnLoadTub.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnLoadTub.ForeColor = Color.White;
            btnLoadTub.Location = new Point(22, 61);
            btnLoadTub.Margin = new Padding(5);
            btnLoadTub.Name = "btnLoadTub";
            btnLoadTub.Size = new Size(229, 99);
            btnLoadTub.TabIndex = 1;
            btnLoadTub.Text = "학습 데이터 \r\n로드";
            btnLoadTub.UseVisualStyleBackColor = false;
            btnLoadTub.Click += btnLoadTub_Click;
            // 
            // pnlDataManagement
            // 
            pnlDataManagement.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pnlDataManagement.BackColor = Color.FromArgb(45, 45, 48);
            pnlDataManagement.Controls.Add(lblDataMgmtTitle);
            pnlDataManagement.Controls.Add(btnApplyFilter);
            pnlDataManagement.Controls.Add(pnlFilterOptions);
            pnlDataManagement.Location = new Point(1389, 638);
            pnlDataManagement.Margin = new Padding(5);
            pnlDataManagement.Name = "pnlDataManagement";
            pnlDataManagement.Size = new Size(588, 322);
            pnlDataManagement.TabIndex = 10;
            // 
            // lblDataMgmtTitle
            // 
            lblDataMgmtTitle.AutoSize = true;
            lblDataMgmtTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblDataMgmtTitle.ForeColor = Color.DeepSkyBlue;
            lblDataMgmtTitle.Location = new Point(17, 16);
            lblDataMgmtTitle.Margin = new Padding(5, 0, 5, 0);
            lblDataMgmtTitle.Name = "lblDataMgmtTitle";
            lblDataMgmtTitle.Size = new Size(137, 38);
            lblDataMgmtTitle.TabIndex = 9;
            lblDataMgmtTitle.Text = "필터 설정";
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.BackColor = Color.FromArgb(0, 122, 204);
            btnApplyFilter.FlatAppearance.BorderSize = 0;
            btnApplyFilter.FlatStyle = FlatStyle.Flat;
            btnApplyFilter.Font = new Font("Segoe UI", 9.5F);
            btnApplyFilter.ForeColor = Color.White;
            btnApplyFilter.Location = new Point(22, 227);
            btnApplyFilter.Margin = new Padding(5);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(544, 67);
            btnApplyFilter.TabIndex = 3;
            btnApplyFilter.Text = "필터 적용";
            btnApplyFilter.UseVisualStyleBackColor = false;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // pnlFilterOptions
            // 
            pnlFilterOptions.BackColor = Color.FromArgb(30, 30, 30);
            pnlFilterOptions.Controls.Add(lblFilterTitle);
            pnlFilterOptions.Controls.Add(chkFilterLargeAngle);
            pnlFilterOptions.Controls.Add(chkFilterLargeThr);
            pnlFilterOptions.Controls.Add(chkFilterThr);
            pnlFilterOptions.Location = new Point(22, 72);
            pnlFilterOptions.Margin = new Padding(5);
            pnlFilterOptions.Name = "pnlFilterOptions";
            pnlFilterOptions.Size = new Size(544, 139);
            pnlFilterOptions.TabIndex = 0;
            // 
            // lblFilterTitle
            // 
            lblFilterTitle.AutoSize = true;
            lblFilterTitle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold);
            lblFilterTitle.ForeColor = Color.DarkGray;
            lblFilterTitle.Location = new Point(8, 8);
            lblFilterTitle.Margin = new Padding(5, 0, 5, 0);
            lblFilterTitle.Name = "lblFilterTitle";
            lblFilterTitle.Size = new Size(107, 30);
            lblFilterTitle.TabIndex = 10;
            lblFilterTitle.Text = "필터 옵션";
            // 
            // chkFilterLargeAngle
            // 
            chkFilterLargeAngle.AutoSize = true;
            chkFilterLargeAngle.ForeColor = Color.White;
            chkFilterLargeAngle.Location = new Point(8, 98);
            chkFilterLargeAngle.Margin = new Padding(5);
            chkFilterLargeAngle.Name = "chkFilterLargeAngle";
            chkFilterLargeAngle.Size = new Size(198, 36);
            chkFilterLargeAngle.TabIndex = 2;
            chkFilterLargeAngle.Text = "급커브 데이터";
            chkFilterLargeAngle.UseVisualStyleBackColor = true;
            // 
            // chkFilterLargeThr
            // 
            chkFilterLargeThr.AutoSize = true;
            chkFilterLargeThr.ForeColor = Color.White;
            chkFilterLargeThr.Location = new Point(213, 48);
            chkFilterLargeThr.Margin = new Padding(5);
            chkFilterLargeThr.Name = "chkFilterLargeThr";
            chkFilterLargeThr.Size = new Size(317, 36);
            chkFilterLargeThr.TabIndex = 1;
            chkFilterLargeThr.Text = "과속 데이터(속도 >= 0.5)";
            chkFilterLargeThr.UseVisualStyleBackColor = true;
            // 
            // chkFilterThr
            // 
            chkFilterThr.AutoSize = true;
            chkFilterThr.ForeColor = Color.White;
            chkFilterThr.Location = new Point(8, 48);
            chkFilterThr.Margin = new Padding(5);
            chkFilterThr.Name = "chkFilterThr";
            chkFilterThr.Size = new Size(124, 36);
            chkFilterThr.TabIndex = 0;
            chkFilterThr.Text = "속도=0";
            chkFilterThr.UseVisualStyleBackColor = true;
            // 
            // btnDeleteData
            // 
            btnDeleteData.BackColor = Color.FromArgb(211, 47, 47);
            btnDeleteData.FlatAppearance.BorderSize = 0;
            btnDeleteData.FlatStyle = FlatStyle.Flat;
            btnDeleteData.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDeleteData.ForeColor = Color.White;
            btnDeleteData.Location = new Point(23, 171);
            btnDeleteData.Margin = new Padding(5);
            btnDeleteData.Name = "btnDeleteData";
            btnDeleteData.Size = new Size(249, 67);
            btnDeleteData.TabIndex = 6;
            btnDeleteData.Text = "데이터 삭제";
            btnDeleteData.UseVisualStyleBackColor = false;
            btnDeleteData.Click += btnDeleteData_Click;
            // 
            // pnlTrainingLog
            // 
            pnlTrainingLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlTrainingLog.BackColor = Color.FromArgb(45, 45, 48);
            pnlTrainingLog.Controls.Add(lblLogTitle);
            pnlTrainingLog.Controls.Add(rtbTrainLog);
            pnlTrainingLog.Location = new Point(48, 114);
            pnlTrainingLog.Margin = new Padding(5);
            pnlTrainingLog.Name = "pnlTrainingLog";
            pnlTrainingLog.Size = new Size(1314, 846);
            pnlTrainingLog.TabIndex = 11;
            pnlTrainingLog.Visible = false;
            // 
            // lblLogTitle
            // 
            lblLogTitle.AutoSize = true;
            lblLogTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            lblLogTitle.ForeColor = Color.DeepSkyBlue;
            lblLogTitle.Location = new Point(17, 19);
            lblLogTitle.Margin = new Padding(5, 0, 5, 0);
            lblLogTitle.Name = "lblLogTitle";
            lblLogTitle.Size = new Size(174, 38);
            lblLogTitle.TabIndex = 10;
            lblLogTitle.Text = "AI 학습 로그";
            // 
            // rtbTrainLog
            // 
            rtbTrainLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbTrainLog.BackColor = Color.FromArgb(30, 30, 30);
            rtbTrainLog.BorderStyle = BorderStyle.None;
            rtbTrainLog.ForeColor = Color.LightGreen;
            rtbTrainLog.Location = new Point(22, 72);
            rtbTrainLog.Margin = new Padding(5);
            rtbTrainLog.Name = "rtbTrainLog";
            rtbTrainLog.ReadOnly = true;
            rtbTrainLog.Size = new Size(1269, 747);
            rtbTrainLog.TabIndex = 1;
            rtbTrainLog.Text = "";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(48, 14);
            lblTitle.Margin = new Padding(5, 0, 5, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(324, 65);
            lblTitle.TabIndex = 12;
            lblTitle.Text = "5팀 동키카 UI";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel1.BackColor = Color.FromArgb(45, 45, 48);
            panel1.Controls.Add(btnSelectAll);
            panel1.Controls.Add(btnRestoreData);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnDeleteData);
            panel1.Location = new Point(1389, 984);
            panel1.Margin = new Padding(5);
            panel1.Name = "panel1";
            panel1.Size = new Size(588, 256);
            panel1.TabIndex = 11;
            // 
            // btnSelectAll
            // 
            btnSelectAll.BackColor = Color.FromArgb(62, 62, 66);
            btnSelectAll.FlatAppearance.BorderSize = 0;
            btnSelectAll.FlatStyle = FlatStyle.Flat;
            btnSelectAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnSelectAll.ForeColor = Color.White;
            btnSelectAll.Location = new Point(23, 77);
            btnSelectAll.Margin = new Padding(5);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(249, 67);
            btnSelectAll.TabIndex = 11;
            btnSelectAll.Text = "리스트 전체 선택";
            btnSelectAll.UseVisualStyleBackColor = false;
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // btnRestoreData
            // 
            btnRestoreData.BackColor = Color.FromArgb(0, 122, 204);
            btnRestoreData.FlatAppearance.BorderSize = 0;
            btnRestoreData.FlatStyle = FlatStyle.Flat;
            btnRestoreData.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnRestoreData.ForeColor = Color.White;
            btnRestoreData.Location = new Point(316, 171);
            btnRestoreData.Margin = new Padding(5);
            btnRestoreData.Name = "btnRestoreData";
            btnRestoreData.Size = new Size(249, 67);
            btnRestoreData.TabIndex = 10;
            btnRestoreData.Text = "삭제 데이터 복원";
            btnRestoreData.UseVisualStyleBackColor = false;
            btnRestoreData.Click += btnRestoreData_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            label1.ForeColor = Color.DeepSkyBlue;
            label1.Location = new Point(17, 16);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(165, 38);
            label1.TabIndex = 9;
            label1.Text = "데이터 삭제";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(2014, 1288);
            Controls.Add(panel1);
            Controls.Add(btnViewLog);
            Controls.Add(lblTitle);
            Controls.Add(pnlCamView);
            Controls.Add(btnViewMonitor);
            Controls.Add(pnlTrainingLog);
            Controls.Add(pnlDataManagement);
            Controls.Add(pnlSystemOps);
            Controls.Add(pnlFrameList);
            Controls.Add(pnlNavigation);
            Margin = new Padding(6);
            MinimumSize = new Size(2023, 1305);
            Name = "Form1";
            Text = "MoveArt Donkeycar 데이터 관리 시스템";
            ((System.ComponentModel.ISupportInitialize)pbMainCam).EndInit();
            pnlCamView.ResumeLayout(false);
            pnlCamView.PerformLayout();
            pnlNavigation.ResumeLayout(false);
            pnlNavigation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkFrameSlider).EndInit();
            pnlFrameList.ResumeLayout(false);
            pnlFrameList.PerformLayout();
            pnlSystemOps.ResumeLayout(false);
            pnlSystemOps.PerformLayout();
            pnlDataManagement.ResumeLayout(false);
            pnlDataManagement.PerformLayout();
            pnlFilterOptions.ResumeLayout(false);
            pnlFilterOptions.PerformLayout();
            pnlTrainingLog.ResumeLayout(false);
            pnlTrainingLog.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbMainCam;
        private Label lblFrameIndex;
        private Label lblAngle;
        private Label lblThrottleTop;
        private Label lblTimestamp;

        private Panel pnlCamView;
        private Panel pnlNavigation;
        private Panel pnlFrameList;
        private Panel pnlSystemOps;
        private Panel pnlDataManagement;
        private Panel pnlFilterOptions;
        private Panel pnlTrainingLog;

        private Label lblCamViewTitle;
        private Label lblNavTitle;
        private Label lblFrameListTitle;
        private Label lblSystemOpsTitle;
        private Label lblDataMgmtTitle;
        private Label lblLogTitle;
        private Label lblFilterTitle;

        private TrackBar trkFrameSlider;
        private Button btnStop;
        private ListBox lstFrameData;
        private Label lblTitle;
        private Button btnFastForward;
        private Button btnFastRewind;
        private Button btnNext;
        private Button btnPrev;
        private Button btnPlay;
        private Button btnStartTraining;
        private Button btnLoadTub;
        private CheckBox chkFilterLargeAngle;
        private CheckBox chkFilterLargeThr;
        private CheckBox chkFilterThr;
        private Button btnApplyFilter;
        private Button btnDeleteData;
        private Button btnSpeed;
        private RichTextBox rtbTrainLog;
        private Button btnViewLog;
        private Button btnViewMonitor;
        private Button btnEndTraining;
        private Panel panel1;
        private Label label1;
        private Button btnStopTraining;
        private Button btnRestoreData;
        private Button btnRestartTraining;
        private Button btnSelectAll;
    }
}
