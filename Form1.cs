using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DateManager
{
    public partial class Form1 : Form
    {
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

        public Form1()
        {
            // UI 컴포넌트를 초기화합니다. (디자인 창의 요소를 불러옴)
            InitializeComponent();

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

        /// <summary>
        /// 디자이너 창에서 추가한 테스트 버튼(button1)을 더블클릭했을 때 실행되는 함수입니다.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //// 💡 진철님 컴퓨터의 실제 WSL Ubuntu 경로 설정 완료!
            //string catalogPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\catalog_0.catalog";
            //string imagesFolderPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\images";

            //// 1. 테스트 시작 알림
            //MessageBox.Show("진철님의 데이터 엔진으로 파일 읽기를 시작합니다!", "테스트 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //// 2. data.cs에 구현해 둔 가공(파싱) 및 자동 분류 함수 호출
            //_masterFrameList = _dataProcessor.LoadCatalogData(catalogPath, imagesFolderPath);

            //// 3. 데이터가 성공적으로 로드되었다면 신규/기존 데이터 분리 결과 최종 검증
            //if (_masterFrameList != null && _masterFrameList.Count > 0)
            //{
            //    // IsNewData 깃발이 true인 새로 수집된 불량 데이터만 골라내기
            //    List<DonkeyFrame> newDataOnly = _masterFrameList.FindAll(frame => frame.IsNewData == true);

            //    string resultReport = $"🏁 [검증 결과 최종 리포트]\n\n" +
            //                          $"📊 전체 데이터 수: {_masterFrameList.Count}개\n" +
            //                          $"❌ 새로 수집된 불량 데이터(정제 대상): {newDataOnly.Count}개\n\n" +
            //                          $"자동 구분이 정상적으로 작동합니다. 이제 UI 팀원에게 코드를 넘겨도 좋습니다!";

            //    MessageBox.Show(resultReport, "엔진 검증 완료", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // 라벨 클릭 이벤트가 필요 없다면 비워둡니다.
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
                    lstFrameData.Items.Add("데이터 로드 중... 잠시만 기다려주세요.");

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
                // Throttle 값이 0보다 큰 것만 남기기 (변수명은 진철님 DonkeyFrame 구조에 맞춤)
                filteredList = filteredList.FindAll(frame => frame.Throttle > 0);
            }

            // 2. Angle == 0 체크박스가 켜져있을 때 (직진 데이터만 필터링)
            if (chkFilterLargeThr.Checked)
            {
                filteredList = filteredList.FindAll(frame => frame.Angle == 0);
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

            lstFrameData.Items.Clear();
            foreach (DonkeyFrame frame in _displayedFrameList)
            {
                lstFrameData.Items.Add($"{prefix}Frame {frame.FrameIndex} | Angle: {frame.Angle:F2} | Thr: {frame.Throttle:F2}");
            }

            trkFrameSlider.Minimum = 0;
            trkFrameSlider.Maximum = Math.Max(0, _displayedFrameList.Count - 1);
            trkFrameSlider.Value = 0;
            trkFrameSlider.Enabled = _displayedFrameList.Count > 0;

            if (_displayedFrameList.Count > 0)
            {
                // 선택 인덱스를 0으로 설정하고 직접 프레임 표시를 한 번 강제합니다.
                SelectFrame(0);
                // SelectedIndexChanged 이벤트가 정상적으로 호출되지 않는 환경을 대비해 직접 호출
                try { DisplayFrame(0); } catch { }
            }
            else
            {
                ClearFramePreview();
            }

            // 로드 후 UI 컨트롤 상태를 일관되게 유지
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

            lblFrameIndex.Text = $"프레임 번호: {index + 1}/{_displayedFrameList.Count}";
            lblAngle.Text = $"조향각: {selectedFrame.Angle:F3}";
            lblThrottleTop.Text = $"출력: {selectedFrame.Throttle:F3}";
            lblTimestamp.Text = string.IsNullOrWhiteSpace(selectedFrame.SessionId)
                ? selectedFrame.DataTypeSummary
                : selectedFrame.SessionId;

            prgThrottle.Value = Math.Max(0, Math.Min(100, (int)Math.Round(selectedFrame.Throttle * 100)));

            if (trkFrameSlider.Value != index)
                trkFrameSlider.Value = index;
        }

        private void ClearFramePreview()
        {
            if (pbMainCam.Image != null)
            {
                pbMainCam.Image.Dispose();
                pbMainCam.Image = null;
            }

            lblAngle.Text = "조향각: +0.0";
            lblThrottleTop.Text = "출력: +0.0";
            lblFrameIndex.Text = "프레임 번호 0/0";
            lblTimestamp.Text = "기록 시간";
            prgThrottle.Value = 0;
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
        }
    }
}
