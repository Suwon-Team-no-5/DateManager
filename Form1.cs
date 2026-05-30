using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices; // 이 줄을 코드 맨 위에 추가하세요.

namespace DateManager
{
    public partial class Form1 : Form
    {
        // Form 클래스 내부나 코드 상단에 아래 API 호출을 정의합니다.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private const uint LB_SETSEL = 0x0185; // 리스트박스 선택 메시지 코드

        // 데이터 정제 및 로드를 담당하는 핵심 백엔드 클래스 선언
        private DataProcessor _dataProcessor;

        // AI 백엔드 엔진 클래스 선언
        private Trainer donkeyTrainer = new Trainer();

        // 메모리에 로드된 전체 카탈로그 데이터를 담아둘 마스터 리스트
        private List<DonkeyFrame> _masterFrameList;
        private List<DonkeyFrame> _displayedFrameList;

        private FileRemover _fileRemover;
        // 카탈로그 경로를 저장할 변수
        private string _currentCatalogPath = "";
        private BackupManager _backupManager = new BackupManager();

        private Picture _pictureHandler;
        private readonly System.Windows.Forms.Timer _playbackTimer;
        private readonly double[] _playbackSpeeds = { 0.5, 1.0, 2.0, 4.0, 8.0 };
        private int _playbackSpeedIndex = 1;
        private const int BasePlaybackIntervalMs = 400;// 기본 재생 간격 (1배속일 때 400ms)
        private int _lastSelectedIndex = -1; // 마지막으로 클릭한 인덱스 저장

        private SelectionMode _normalListSelectionMode = SelectionMode.MultiExtended;
        private bool _isPlaybackSelecting = false;
        private bool _playbackSelectionModeSaved = false;
        private int _playIndex = -1;

        private int _currentFrameIndex = 0; // 🌟 [변경] 미래의 데이터를 꺼내오기 위해 현재 인덱스를 저장

        // 클래스 상단에 선언
        private readonly Font _bigFont = new Font("Segoe UI", 32, FontStyle.Bold);
        private readonly Font _smallFont = new Font("Segoe UI", 10, FontStyle.Bold);

        private readonly SolidBrush _shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0));
        private readonly SolidBrush _bgBrush = new SolidBrush(Color.FromArgb(100, 200, 200, 200));
        private readonly SolidBrush _barBlueBrush = new SolidBrush(Color.DeepSkyBlue);
        private readonly SolidBrush _barOrangeBrush = new SolidBrush(Color.Orange);

        public Form1()
        {
            // UI 컴포넌트를 초기화합니다. (디자인 창의 요소를 불러옴)
            InitializeComponent();
            this.DoubleBuffered = true;

            // 폼에서 키 입력을 전역으로 처리하도록 설정
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            // 탭 순서를 명시적으로 관리하기 위한 리스트 초기화
            _focusOrder = new List<Control>();

            // 프로그램이 켜질 때 객체들을 초기화해 줍니다.
            _dataProcessor = new DataProcessor();
            _fileRemover = new FileRemover();
            _masterFrameList = new List<DonkeyFrame>();
            _displayedFrameList = new List<DonkeyFrame>();
            _pictureHandler = new Picture();
            _playbackTimer = new System.Windows.Forms.Timer(); //재생하면서 넘길 타이머
            _playbackTimer.Tick += PlaybackTimer_Tick;
            UpdatePlaybackInterval();// 타이머 간격을 초기 배속에 맞게 설정

            // 프로그램 켜질 때 AI 실시간 로그 이벤트 연결
            donkeyTrainer.LogReceived += (logText) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    rtbTrainLog.AppendText(logText);
                    rtbTrainLog.SelectionStart = rtbTrainLog.TextLength;
                    rtbTrainLog.ScrollToCaret();
                });
            };

            // AI 학습이 완전히 끝났을 때 버튼 복구 이벤트 연결
            donkeyTrainer.TrainingFinished += () =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnStartTraining.Enabled = true;   // ⭕ 시작 버튼 다시 켜고
                    btnStopTraining.Enabled = false;  // ❌ 중단 버튼은 다시 잠그기
                });
            };

            // 폼이 완전히 닫힐 때 백그라운드 좀비 프로세스 방지 안전장치 연결
            this.FormClosing += (s, e) => donkeyTrainer.KillProcess();

            this.pbMainCam.Paint += pbMainCam_Paint;

        }

        // 탭 순서 제어를 위한 컨트롤 리스트
        private List<Control> _focusOrder;


        /// <summary>
        /// 폼이 처음 로드될 때 실행되는 함수입니다. (중복 제거 완료)
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 필요한 경우 여기에 초기화 코드를 넣습니다.
            // 요청된 탭 순서: 설정 파일 로드 -> 학습 데이터 로드 -> AI 학습 시작 -> 시작지점 -> 종료지점 -> 필터 적용 -> 삭제 -> 재생 -> 정지 -> 배속
            _focusOrder.Clear();
            _focusOrder.Add(btnLoadTub);       // 학습 데이터 로드
            _focusOrder.Add(btnStartTraining); // AI 학습 시작
            _focusOrder.Add(btnApplyFilter);   // 필터 적용
            _focusOrder.Add(btnDeleteData);    // 삭제
            _focusOrder.Add(btnPlay);          // 재생
            _focusOrder.Add(btnStop);          // 정지
            _focusOrder.Add(btnSpeed);         // 배속

            // 포커스 가능한 컨트롤들에 TabStop 활성화
            foreach (var c in _focusOrder) c.TabStop = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 리소스 정리 (중요!)
            _bigFont?.Dispose();
            _smallFont?.Dispose();
            _shadowBrush?.Dispose();
            _bgBrush?.Dispose();
            _barBlueBrush?.Dispose();
            _barOrangeBrush?.Dispose();

            donkeyTrainer.KillProcess();
        }


        private async void btnLoadTub_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Donkeycar 데이터(Tub) 폴더를 선택하세요.";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _currentCatalogPath = fbd.SelectedPath; //(경로 저장)
                    _backupManager.CurrentCatalogPath = _currentCatalogPath;
                    string selectedPath = fbd.SelectedPath;

                    // 로딩 중임을 사용자에게 알림 (UI가 멈추지 않음)
                    this.Cursor = Cursors.WaitCursor;
                    lstFrameData.Items.Clear();
                    //lstFrameData.Items.Add("데이터 로드 중... 잠시만 기다려주세요.");

                    try
                    {
                        // 💡 비동기 작업으로 무거운 로드 작업을 별도 스레드에서 수행
                        _masterFrameList = await Task.Run(() => _dataProcessor.LoadCatalogData(selectedPath));

                        // 로드 완료 후 UI 업데이트: 마스터 리스트를 바로 표시 리스트로 반영
                        if (_masterFrameList != null && _masterFrameList.Count > 0)
                        {
                            RefreshFrameList(_masterFrameList);
                            MessageBox.Show($"총 {_masterFrameList.Count}개의 데이터를 로드했습니다!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"데이터 로드 중 오류가 발생했습니다: {ex.Message}");
                    }
                    finally
                    {
                        // 커서 복구
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (_masterFrameList == null || _masterFrameList.Count == 0)
            {
                MessageBox.Show("먼저 데이터를 로드해 주세요!", "알림!");
                return;
            }

            // 마스터 리스트를 복사해서 필터링을 시작할 임시 리스트 생성
            List<DonkeyFrame> filteredList = new List<DonkeyFrame>(_masterFrameList);

            // 1. Thr > 0 체크박스가 켜져있을 때
            if (chkFilterThr.Checked)
            {
                filteredList = filteredList.FindAll(frame => frame.Throttle == 0);
            }

            if (chkFilterLargeAngle.Checked)
            {
                filteredList = filteredList.FindAll(frame => Math.Abs(frame.Angle) >= 0.5);
            }

            if (chkFilterLargeThr.Checked)
            {
                filteredList = filteredList.FindAll(frame => frame.Throttle >= 0.5);
            }

            // 3. 필터링된 결과를 우측 리스트박스(lstFrameData)에 다시 업데이트
            RefreshFrameList(filteredList, "[필터됨] ");


            MessageBox.Show($"필터링 완료! {filteredList.Count}개의 데이터가 조건에 맞습니다.", "필터 결과");

            //아래 윤형규가 추가한 코드, 오류 발생 시 우선 주석처리 할 것
            if (!chkFilterThr.Checked && !chkFilterLargeThr.Checked && !chkFilterLargeAngle.Checked)
            {
                RefreshFrameList(_masterFrameList);
                //필터 없으면 원본 리스트 불러옴
            }

        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            // 1. 선택된 데이터 확인
            if (lstFrameData.SelectedIndices.Count == 0)
            {
                MessageBox.Show("삭제할 프레임을 리스트에서 선택해주세요!", "선택 필요");
                return;
            }

            int firstSelectedIndex = lstFrameData.SelectedIndices.Cast<int>().Min();
            List<DonkeyFrame> selectedFrames = lstFrameData.SelectedIndices
                .Cast<int>()
                .Where(index => index >= 0 && index < _displayedFrameList.Count)
                .Select(index => _displayedFrameList[index])
                .ToList();

            if (selectedFrames.Count == 0) return;

            if (MessageBox.Show("선택된 데이터를 삭제할까요?", "삭제 확인", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                pbMainCam.Image?.Dispose();
                pbMainCam.Image = null;

                DeleteResult deleteResult = _fileRemover.RemoveFramesFromCatalogs(_masterFrameList, selectedFrames);
                _displayedFrameList.RemoveAll(frame => selectedFrames.Contains(frame));

                // 1. 리스트 갱신
                RefreshFrameList(_displayedFrameList);

                // 2. 삭제 후 선택 위치 조정 (범위 초과 방지)
                if (_displayedFrameList.Count > 0)
                {
                    int nextIndex = Math.Min(firstSelectedIndex, _displayedFrameList.Count - 1);
                    SelectFrame(nextIndex); // 💡 여기서 인덱스 설정 및 DisplayFrame 호출 발생
                }

                MessageBox.Show($"{deleteResult.DeletedCount}개 삭제 완료.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"삭제 오류: {ex.Message}");
            }

        }

        private void lstFrameData_SelectedIndexChanged(object sender, EventArgs e) // 리스트박스에서 선택이 바뀔 때마다 해당 프레임을 미리보기로 보여주는 이벤트 핸들러
        {
            if (lstFrameData.SelectedIndex == -1) return;

            // 재생 코드가 선택을 바꾸는 중이면 start/end를 건드리지 않음
            if (_isPlaybackSelecting)
                return;

            start = lstFrameData.SelectedIndex;
            end = lstFrameData.SelectedIndex;

            DisplayFrame(lstFrameData.SelectedIndex);
        }

        private void trkFrameSlider_Scroll(object sender, EventArgs e)
        {
            // 트랙바의 현재 값(Value)을 리스트박스의 인덱스로 설정
            if (trkFrameSlider.Value >= 0 && trkFrameSlider.Value < _displayedFrameList.Count)
            {
                SelectFrame(trkFrameSlider.Value);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            MoveFrame(-100);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            MoveFrame(100);
        }

        private void btnFastRewind_Click(object sender, EventArgs e)
        {
            MoveFrame(-1);
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            MoveFrame(1);
        }

        private void btnPlay_Click(object sender, EventArgs e) // 재생 버튼을 눌렀을 때 타이머를 시작하거나 멈추는 토글 기능
        {
            if (!HasDisplayedFrames()) return;

            if (_playbackTimer.Enabled)
            {
                StopPlayback();
                return;
            }

            // 재생중일 때 다중선택 잠시 끄기
            _playIndex = lstFrameData.SelectedIndex >= 0 ? lstFrameData.SelectedIndex : 0;
            SelectFrame(_playIndex);

            _playbackTimer.Start();
            btnPlay.Text = "⏸ 일시정지";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPlayback();
            if (HasDisplayedFrames(false))
            {
                _playIndex = 0;
                SelectFrame(0);
            }
        }

        private void btnSpeed_Click(object sender, EventArgs e)
        {
            _playbackSpeedIndex = (_playbackSpeedIndex + 1) % _playbackSpeeds.Length;
            UpdatePlaybackInterval();
        }

        // 폼 전체에 대한 키보드 단축키 핸들러
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // 스페이스: 재생/일시정지 토글
            if (e.KeyCode == Keys.Space)
            {
                btnPlay.PerformClick();
                e.Handled = true;
            }

            // 화살표 위/아래: 포커스 이동
            if (e.KeyCode == Keys.Up)
            {
                MoveFocus(-1);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                MoveFocus(1);
                e.Handled = true;
            }

            // Home/End: 첫 프레임 / 마지막 프레임으로 이동
            if (e.KeyCode == Keys.Home)
            {
                if (_displayedFrameList != null && _displayedFrameList.Count > 0)
                {
                    SelectFrame(0);
                }
                e.Handled = true;
            }

            if (e.KeyCode == Keys.End)
            {
                if (_displayedFrameList != null && _displayedFrameList.Count > 0)
                {
                    SelectFrame(_displayedFrameList.Count - 1);
                }
                e.Handled = true;
            }

            // Enter: 포커스가 올라간 버튼을 클릭 처리
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Control ctrl = this.ActiveControl;
                    // 컨테이너 내부에 포커스된 자식 컨트롤이 있는 경우 가장 깊은 ActiveControl을 찾음
                    while (ctrl is ContainerControl container && container.ActiveControl != null)
                    {
                        ctrl = container.ActiveControl;
                    }

                    if (ctrl is Button btn)
                    {
                        btn.PerformClick();
                        e.Handled = true;
                    }
                }
                catch { }
            }
        }

        private void MoveFocus(int delta)
        {
            if (_focusOrder == null || _focusOrder.Count == 0) return;
            Control active = this.ActiveControl ?? _focusOrder[0];
            int idx = _focusOrder.IndexOf(active);
            if (idx == -1) idx = 0;
            int next = (idx + delta + _focusOrder.Count) % _focusOrder.Count;
            _focusOrder[next].Focus();
        }

        private void PlaybackTimer_Tick(object? sender, EventArgs e) // 타이머가 틱할 때마다 다음 프레임으로 이동하는 이벤트 핸들러
        {
            if (!HasDisplayedFrames(false))
            {
                StopPlayback();
                return;
            }

            _playIndex++;

            if (_playIndex >= _displayedFrameList.Count)
            {
                StopPlayback();
                return;
            }

            SelectFrame(_playIndex);
        }

        private void RefreshFrameList(IEnumerable<DonkeyFrame> frames, string prefix = "")
        {
            StopPlayback();
            _displayedFrameList = frames?.ToList() ?? new List<DonkeyFrame>();

            lstFrameData.BeginUpdate();
            try
            {
                string[] items = _displayedFrameList
            .Select(f => $"{prefix}Frame {f.FrameIndex}") // 여기를 수정했습니다.
            .ToArray();
                lstFrameData.Items.AddRange(items);
            }
            finally
            {
                lstFrameData.EndUpdate();
            }

            trkFrameSlider.Minimum = 0;
            trkFrameSlider.Maximum = Math.Max(0, _displayedFrameList.Count - 1);
            trkFrameSlider.Value = 0;
            trkFrameSlider.Enabled = _displayedFrameList.Count > 0;

            if (_displayedFrameList.Count > 0)
            {
                SelectFrame(0);
                try { DisplayFrame(0); } catch { }
            }
            else
            {
                ClearFramePreview();
            }
            UpdateControlsAfterLoad();
        }

        // 데이터가 로드되거나 리스트가 갱신된 후에 각종 컨트롤(재생, 정지, 탐색 버튼 등)을
        // 현재 표시 중인 데이터의 유무에 따라 적절히 활성/비활성화합니다.
        private void UpdateControlsAfterLoad()
        {
            bool has = _displayedFrameList != null && _displayedFrameList.Count > 0;

            // 탐색 관련
            btnPlay.Enabled = has;
            btnStop.Enabled = has;
            btnPrev.Enabled = has;
            btnNext.Enabled = has;
            btnFastForward.Enabled = has;
            btnFastRewind.Enabled = has;
            trkFrameSlider.Enabled = has;

            if (!has)
            {
                StopPlayback();
            }
            else
            {
                // 기본 재생 버튼 텍스트 및 상태 초기화
                btnPlay.Text = _playbackTimer.Enabled ? "⏸ 일시정지" : "▶ 재생";
            }
        }

        private void SelectFrame(int index)
        {
            if (!HasDisplayedFrames(false)) return;

            int safeIndex = Math.Max(0, Math.Min(index, _displayedFrameList.Count - 1));

            _isPlaybackSelecting = true;

            try
            {
                lstFrameData.ClearSelected();
                lstFrameData.SelectedIndex = safeIndex;

                int visibleCount = Math.Max(1, lstFrameData.ClientSize.Height / Math.Max(1, lstFrameData.ItemHeight));
                int topIndex = Math.Max(0, safeIndex - visibleCount / 2);

                if (topIndex < lstFrameData.Items.Count)
                    lstFrameData.TopIndex = topIndex;
            }
            finally
            {
                _isPlaybackSelecting = false;
            }

            DisplayFrame(safeIndex);
        }

        private void MoveFrame(int offset)
        {
            if (!HasDisplayedFrames()) return;

            int currentIndex = lstFrameData.SelectedIndex < 0 ? 0 : lstFrameData.SelectedIndex;
            SelectFrame(currentIndex + offset);
        }

        private void DisplayFrame(int index)
        {
            if (_displayedFrameList == null || index < 0 || index >= _displayedFrameList.Count) return;

            DonkeyFrame selectedFrame = _displayedFrameList[index];

            _pictureHandler.LoadImageToPictureBox(pbMainCam, selectedFrame.FullImagePath);

            _currentFrameIndex = index;

            lblAngle.Text = $"방향: {selectedFrame.Angle:F3}";
            lblThrottleTop.Text = $"속도: {selectedFrame.Throttle:F3}";
            lblFrameIndex.Text = $"프레임 인덱스: {index + 1}/{_displayedFrameList.Count} (원본 {selectedFrame.FrameIndex})";
            lblTimestamp.Text = string.IsNullOrWhiteSpace(selectedFrame.SessionId)
                ? selectedFrame.DataTypeSummary
                : selectedFrame.SessionId;

            prgThrottle.Value = Math.Max(0, Math.Min(100, (int)Math.Round(selectedFrame.Throttle * 100)));

            if (trkFrameSlider.Value != index)
            {
                trkFrameSlider.Value = index;
            }

            pbMainCam.Invalidate();
        }

        private void ClearFramePreview()
        {
            if (pbMainCam.Image != null)
            {
                pbMainCam.Image.Dispose();
                pbMainCam.Image = null;
            }

            _currentFrameIndex = 0; // 🌟 [추가] 초기화 시 각도도 0으로 복구
            pbMainCam.Invalidate();      // 🌟 [추가] 그려진 선 지우기

            lblAngle.Text = "방향: +0.0";
            lblThrottleTop.Text = "속도: +0.0";
            lblFrameIndex.Text = "프레임 번호 0/0";
            lblTimestamp.Text = "기록 시간";
        }

        private void StopPlayback()
        {
            _playbackTimer.Stop();
            btnPlay.Text = "▶ 재생";
        }

        private void UpdatePlaybackInterval()
        {
            double speed = _playbackSpeeds[_playbackSpeedIndex];
            _playbackTimer.Interval = Math.Max(50, (int)Math.Round(BasePlaybackIntervalMs / speed));
            btnSpeed.Text = $"배속 x {speed:0.0}";
        }

        private bool HasDisplayedFrames(bool showMessage = true)
        {
            bool hasFrames = _displayedFrameList != null && _displayedFrameList.Count > 0;
            if (!hasFrames && showMessage)
            {
                MessageBox.Show("먼저 데이터를 로드해 주세요!", "알림");
            }

            return hasFrames;
        }
        public int start, end = 0;
        private void btnSetLeft_Click(object sender, EventArgs e)
        {
            start = lstFrameData.SelectedIndex;
        }

        private void btnSetRight_Click(object sender, EventArgs e)
        {
            end = lstFrameData.SelectedIndex;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStartTraining.Visible = false;
            btnStopTraining.Visible = true;
            rtbTrainLog.Clear();
            rtbTrainLog.AppendText(" AI 학습 연동을 시작합니다...\r\n");

            // 버튼 제어
            btnStartTraining.Enabled = false; // 시작 버튼 잠그기
            btnStopTraining.Enabled = true;   // 중단 버튼 깨우기

            string pythonPath = "wsl.exe";
            string mycarDir = "/home/jinchul04/mycar";

            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir));
        }

        private void btnViewMonitor_Click(object sender, EventArgs e)
        {
            // 주행 모니터 패널은 켜고, 학습 로그 패널은 끕니다.
            pnlCamView.Visible = true;
            pnlTrainingLog.Visible = false;

            // 버튼 색상 변경
            btnViewMonitor.BackColor = Color.FromArgb(0, 122, 204);
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            // 학습 로그 패널은 켜고, 주행 모니터 패널은 끕니다.
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = true;

            // 버튼 색상 변경
            btnViewLog.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnStopTraining_Click(object sender, EventArgs e)
        {
            btnStopTraining.Visible = false;
            btnRestartTraining.Visible = true;
            rtbTrainLog.AppendText("\r\n🛑 사용자의 요청으로 AI 학습을 강제 중단합니다...\r\n");

            // 버튼 중복 클릭 방지 차단
            btnStopTraining.Enabled = false;

            // Trainer.cs에 만들어 둔 리눅스 좀비 프로세스 중지 함수 호출
            donkeyTrainer.KillProcess();

        }

        private void btnRestartTraining_Click(object sender, EventArgs e)
        {
            btnRestartTraining.Visible = false;
            btnStopTraining.Visible = true;
        }

        private void btnEndTraining_Click(object sender, EventArgs e)
        {
            btnRestartTraining.Visible = false;
            btnStopTraining.Visible = false;
            btnStartTraining.Visible = true;
        }

        private void btnRestoreData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentCatalogPath)) return;

            int restoreIndex = lstFrameData.SelectedIndex >= 0 ? lstFrameData.SelectedIndex : start;
            if (restoreIndex < 0) restoreIndex = 0;

            OpenFileDialog ofd = new OpenFileDialog();
            string backupDir = Path.Combine(_currentCatalogPath, "backup");
            if (Directory.Exists(backupDir)) ofd.InitialDirectory = backupDir;
            ofd.Filter = "Catalog Files (*.catalog)|*.catalog";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _backupManager.RestoreFromBackup(ofd.FileName, _currentCatalogPath);
                    _masterFrameList = _dataProcessor.LoadCatalogData(_currentCatalogPath);

                    RefreshFrameList(_masterFrameList);

                    if (_displayedFrameList.Count > 0)
                    {
                        int nextIndex = Math.Min(restoreIndex, _displayedFrameList.Count - 1);
                        SelectFrame(nextIndex);

                        if (nextIndex < lstFrameData.Items.Count)
                            lstFrameData.TopIndex = nextIndex;
                    }

                    MessageBox.Show("복원이 완료되었습니다!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"복원 실패: {ex.Message}");
                }
            }

        // 🌟🌟🌟 궤적 부드러움 보정 & HUD 우측 하단 배치 버전 🌟🌟🌟
        private void pbMainCam_Paint(object sender, PaintEventArgs e)
        {
            if (_displayedFrameList == null || _displayedFrameList.Count == 0 || pbMainCam.Image == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float width = pbMainCam.Width;
            float height = pbMainCam.Height;

            // =========================================================
            // 1. 테슬라 스타일 AR 궤적 (10개 세그먼트 고정)
            // =========================================================
            int segments = 10;
            int lookAheadStep = 2;
            float pathHeight = height * 0.6f;
            float bottomWidth = 140f;
            float topWidth = 20f;
            float curX = width / 2.0f;
            float angleSum = 0f;
            float maxDeflection = width * 0.012f;

            PointF[] leftEdge = new PointF[segments + 1];
            PointF[] rightEdge = new PointF[segments + 1];

            leftEdge[0] = new PointF(curX - bottomWidth / 2, height);
            rightEdge[0] = new PointF(curX + bottomWidth / 2, height);

            float smoothedAngle = 0f;

            for (int i = 1; i <= segments; i++)
            {
                int futureIndex = Math.Min(_currentFrameIndex + (i * lookAheadStep), _displayedFrameList.Count - 1);
                float targetAngle = (float)_displayedFrameList[futureIndex].Angle;
                smoothedAngle = (smoothedAngle * 0.7f) + (targetAngle * 0.3f);
                angleSum += smoothedAngle;

                float progress = i / (float)segments;
                float nextY = height - (pathHeight * progress);
                float nextX = curX + (angleSum * maxDeflection);
                curX = nextX;
                float currentWidth = bottomWidth - ((bottomWidth - topWidth) * progress);
                leftEdge[i] = new PointF(curX - currentWidth / 2, nextY);
                rightEdge[i] = new PointF(curX + currentWidth / 2, nextY);
            }

            for (int i = 0; i < segments; i++)
            {
                float gap = 5f;
                PointF[] poly = { leftEdge[i], rightEdge[i], new PointF(rightEdge[i + 1].X, rightEdge[i + 1].Y + gap), new PointF(leftEdge[i + 1].X, leftEdge[i + 1].Y + gap) };
                int alpha = 180 - (int)(150 * (i / (float)segments));

                using (SolidBrush pathBrush = new SolidBrush(Color.FromArgb(alpha, 30, 144, 255)))
                {
                    g.FillPolygon(pathBrush, poly);
                }
            }

            // =========================================================
            // 2. HUD를 오른쪽 아래로 이동 (Segoe UI 사용)
            // =========================================================
            float throttle = (float)_displayedFrameList[_currentFrameIndex].Throttle;
            int displaySpeed = (int)Math.Round(Math.Abs(throttle) * 100);
            float hudRight = width - 50;
            float hudBottom = height - 20;
            string speedText = displaySpeed.ToString();
            SizeF size = g.MeasureString(speedText, _bigFont);

            float textX = hudRight - size.Width;
            float textY = hudBottom - size.Height - 15;

            // 멤버 변수 브러시 사용
            g.DrawString(speedText, _bigFont, _shadowBrush, new PointF(textX + 2, textY + 2));
            g.DrawString(speedText, _bigFont, Brushes.White, new PointF(textX, textY));
            g.DrawString("Km/h", _smallFont, Brushes.LightGray, new PointF(textX + size.Width - 10, textY + 15));

            // 게이지 바
            int barWidth = 100;
            int barHeight = 6;
            int fillWidth = (int)(barWidth * Math.Min(Math.Abs(throttle), 1.0f));
            float barX = hudRight - barWidth;
            float barY = hudBottom - 10;

            g.FillRectangle(_bgBrush, barX, barY, barWidth, barHeight);

            SolidBrush barColorBrush = throttle >= 0 ? _barBlueBrush : _barOrangeBrush;
            g.FillRectangle(barColorBrush, barX, barY, fillWidth, barHeight);
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (lstFrameData.Items.Count == 0) return;

            // 1. 이벤트 구독 해제 (혹시 모를 이벤트 발생 방지)
            lstFrameData.SelectedIndexChanged -= lstFrameData_SelectedIndexChanged;

            // 2. 화면 갱신 멈춤
            lstFrameData.BeginUpdate();

            try
            {
                // 3. 윈도우 API를 사용하여 리스트박스에 '전체 선택' 명령을 직접 보냅니다.
                // wParam: 1 (선택), lParam: -1 (전체 항목)
                SendMessage(lstFrameData.Handle, LB_SETSEL, 1, -1);
            }
            finally
            {
                lstFrameData.EndUpdate();
                // 4. 이벤트 구독 복구
                lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            }

            // 마지막으로 화면에 반영
            DisplayFrame(lstFrameData.Items.Count - 1);

        }
    }
}
