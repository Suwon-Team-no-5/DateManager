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

        private Picture _pictureHandler;
        private readonly System.Windows.Forms.Timer _playbackTimer;
        private readonly double[] _playbackSpeeds = { 0.5, 1.0, 2.0, 4.0 };
        private int _playbackSpeedIndex = 1;
        private const int BasePlaybackIntervalMs = 400;// 기본 재생 간격 (1배속일 때 400ms)

        public Form1()
        {
            // UI 컴포넌트를 초기화합니다. (디자인 창의 요소를 불러옴)
            InitializeComponent();

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

        /// <summary>
        /// 폼이 처음 로드될 때 실행되는 함수입니다. (중복 제거 완료)
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 필요한 경우 여기에 초기화 코드를 넣습니다.
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

        private void btnLoadTub_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 💡 파일 1개가 아니라 폴더 전체 경로를 넘김
                    _masterFrameList = _dataProcessor.LoadCatalogData(fbd.SelectedPath);

                    if (_masterFrameList != null && _masterFrameList.Count > 0)
                    {
                        RefreshFrameList(_masterFrameList);

                        MessageBox.Show($"총 {_masterFrameList.Count}개의 프레임 데이터 로드 완료!");
                    }
                }
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (_masterFrameList == null || _masterFrameList.Count == 0)
            {
                MessageBox.Show("먼저 데이터를 로드해 주세요!", "알림");
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
            if (chkFilterAngleZero.Checked)
            {
                filteredList = filteredList.FindAll(frame => frame.Angle == 0);
            }

            if (chkFilterLargeAngle.Checked)
            {
                filteredList = filteredList.FindAll(frame => Math.Abs(frame.Angle) >= 0.5);
            }

            // 3. 필터링된 결과를 우측 리스트박스(lstFrameData)에 다시 업데이트
            RefreshFrameList(filteredList, "[필터됨] ");


            MessageBox.Show($"필터링 완료! {filteredList.Count}개의 데이터가 조건에 맞습니다.", "필터 결과");

            //아래 윤형규가 추가한 코드, 오류 발생 시 우선 주석처리 할 것
            if (!chkFilterThr.Checked && !chkFilterAngleZero.Checked && !chkFilterLargeAngle.Checked)
            {
                RefreshFrameList(_masterFrameList);
                //필터 없으면 원본 리스트 불러옴
            }

        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            // 1. 선택된 데이터가 있는지 확인// 1. 선택된 데이터가 있는지 확인 (리스트박스에서 선택된 항목이 없으면 -1 반환)
            if (lstFrameData.SelectedIndex == -1)
            {
                MessageBox.Show("삭제할 프레임을 리스트에서 선택해주세요!", "선택 필요");
                return;
            }

            if (!HasDisplayedFrames(false))
            {
                MessageBox.Show("삭제할 데이터가 없습니다!", "알림");
                return;
            }

            // 2. 삭제할 프레임 객체 가져오기
            int selectedIndex = lstFrameData.SelectedIndex;
            DonkeyFrame targetFrame = _displayedFrameList[selectedIndex];

            // 3. 사용자 확인 절차
            DialogResult result = MessageBox.Show($"Frame {targetFrame.FrameIndex}번 데이터를 삭제할까요?\n이 작업은 되돌릴 수 없습니다.",
                                                  "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return; // 사용자가 삭제를 취소한 경우 함수 종료

            //!!!!!!아래 다중 삭제 로직 윤형규가 작성, 오류 발생 시 우선 주석처리 할 것
            if (Math.Max(start, end) - Math.Min(start, end) > 0)
            {
                DialogResult rangeResult = MessageBox.Show($"선택된 범위 ({start}, {end})의 데이터를 모두 삭제할까요?\n이 작업은 되돌릴 수 없습니다.",
                                                  "범위 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (rangeResult == DialogResult.Yes)
                {
                    try
                    {
                        _fileRemover.RemoveFrames(_masterFrameList, _masterFrameList[Math.Min(start, end)], _masterFrameList[Math.Max(start, end)]);
                        start = 0; end = 0;
                        lblSetRange.Text = "(0, 0)";
                        //_displayedFrameList.RemoveAll(frame => frame.FrameIndex >= Math.Min(start, end) && frame.FrameIndex <= Math.Max(start, end));
                        RefreshFrameList(_masterFrameList);

                        MessageBox.Show("선택된 범위의 데이터가 성공적으로 삭제되었습니다.", "범위 삭제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"범위 삭제 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            }

            // 4. 새로운 클래스(FileRemover)를 사용하여 파일 및 리스트 삭제 호출
            try
            {
                // 💡 여기서 분리한 클래스의 메서드를 호출합니다.
                // _fileRemover는 Form1 생성자에서 미리 선언 및 초기화해 두어야 합니다.
                _fileRemover.RemoveFrame(_masterFrameList, targetFrame);

                // 5. UI 업데이트: 리스트박스에서 해당 항목 제거
                _displayedFrameList.Remove(targetFrame);
                RefreshFrameList(_displayedFrameList);
                if (_displayedFrameList.Count > 0)
                {
                    SelectFrame(Math.Min(selectedIndex, _displayedFrameList.Count - 1));
                }

                MessageBox.Show("삭제가 성공적으로 완료되었습니다.", "정제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // 삭제 과정에서 발생한 에러 처리
                MessageBox.Show($"삭제 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstFrameData_SelectedIndexChanged(object sender, EventArgs e) // 리스트박스에서 선택이 바뀔 때마다 해당 프레임을 미리보기로 보여주는 이벤트 핸들러
        {
            int index = lstFrameData.SelectedIndex;
            if (index >= 0 && index < _displayedFrameList.Count)
            {
                DisplayFrame(index);
            }
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

            _playbackTimer.Start();
            btnPlay.Text = "⏸ 일시정지";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPlayback();
            if (HasDisplayedFrames(false))
            {
                SelectFrame(0);
            }
        }

        private void btnSpeed_Click(object sender, EventArgs e)
        {
            _playbackSpeedIndex = (_playbackSpeedIndex + 1) % _playbackSpeeds.Length;
            UpdatePlaybackInterval();
        }

        private void PlaybackTimer_Tick(object? sender, EventArgs e) // 타이머가 틱할 때마다 다음 프레임으로 이동하는 이벤트 핸들러
        {
            if (!HasDisplayedFrames(false))
            {
                StopPlayback();
                return;
            }

            int currentIndex = lstFrameData.SelectedIndex < 0 ? 0 : lstFrameData.SelectedIndex; // 현재 선택된 프레임의 인덱스 가져오기 (선택된 항목이 없으면 0으로 간주)
            if (currentIndex >= _displayedFrameList.Count - 1)
            {
                StopPlayback();
                return;
            }

            SelectFrame(currentIndex + 1);
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
                SelectFrame(0);
            }
            else
            {
                ClearFramePreview();
            }
        }

        private void SelectFrame(int index)
        {
            if (!HasDisplayedFrames(false)) return;

            int safeIndex = Math.Max(0, Math.Min(index, _displayedFrameList.Count - 1));
            if (lstFrameData.SelectedIndex != safeIndex)
            {
                lstFrameData.SelectedIndex = safeIndex;
            }
            else
            {
                DisplayFrame(safeIndex);
            }
        }

        private void MoveFrame(int offset)
        {
            if (!HasDisplayedFrames()) return;

            int currentIndex = lstFrameData.SelectedIndex < 0 ? 0 : lstFrameData.SelectedIndex;
            SelectFrame(currentIndex + offset);
        }

        private void DisplayFrame(int index)
        {
            DonkeyFrame selectedFrame = _displayedFrameList[index];
            _pictureHandler.LoadImageToPictureBox(pbMainCam, selectedFrame.FullImagePath);

            lblAngle.Text = $"조향값(앵글): {selectedFrame.Angle:F3}";
            lblThrottleTop.Text = $"출력(스레틀): {selectedFrame.Throttle:F3}";
            lblFrameIndex.Text = $"프레임 인덱스: {index + 1}/{_displayedFrameList.Count} (원본 {selectedFrame.FrameIndex})";
            lblTimestamp.Text = string.IsNullOrWhiteSpace(selectedFrame.SessionId) ? selectedFrame.DataTypeSummary : selectedFrame.SessionId;
            prgThrottle.Value = Math.Max(0, Math.Min(100, (int)Math.Round(selectedFrame.Throttle * 100)));

            if (trkFrameSlider.Value != index)
            {
                trkFrameSlider.Value = index;
            }
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
            lblSetRange.Text = "(" + start + ", " + end + ")";
        }

        private void btnSetRight_Click(object sender, EventArgs e)
        {
            end = lstFrameData.SelectedIndex;
            lblSetRange.Text = "(" + start + ", " + end + ")";
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            rtbTrainLog.Clear();
            rtbTrainLog.AppendText(" AI 학습 연동을 시작합니다...\r\n");

            // 버튼 제어
            btnStartTraining.Enabled = false; // 시작 버튼 잠그기
            btnStopTraining.Enabled = true;   // 중단 버튼 깨우기

            string pythonPath = "wsl.exe";
            string mycarDir = "/home/giju/mycar";

            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir));
        }
        private void btnStopTraining_Click(object sender, EventArgs e)
        {
            rtbTrainLog.AppendText("\r\n🛑 사용자의 요청으로 AI 학습을 강제 중단합니다...\r\n");

            // 버튼 중복 클릭 방지 차단
            btnStopTraining.Enabled = false;

            // Trainer.cs에 만들어 둔 리눅스 좀비 프로세스 중지 함수 호출
            donkeyTrainer.KillProcess();
        }
    }
}
