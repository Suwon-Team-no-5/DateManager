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

        private List<DonkeyFrame> _trashFrameList = new List<DonkeyFrame>();
        private List<float> lossPoints = new List<float>();

        private bool _isViewingTrash = false;

        private int _lastViewedFrameId = -1;
        private bool _wasPlaying = false; // 💡 [추가] 휴지통 열기 전 재생 중이었는지 저장할 변수

        private bool _isTrainingComplete = false; // 학습 완료 플래그
        private string _modelWinPath = @"\\wsl.localhost\Ubuntu-22.04\home\jaeseo03\mycar\models\mypilot.h5";

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

                    if (logText.Contains("loss:"))
                    {
                        try
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(logText, @"loss:\s*([0-9\.]+)");
                            if (match.Success)
                            {
                                float currentLoss = float.Parse(match.Groups[1].Value);

                                // 리스트에 수치를 채우고, 하단의 전용 커스텀 드로잉 함수를 호출합니다!
                                lossPoints.Add(currentLoss);
                                DrawRealTimeChart();
                            }
                        }
                        catch { }
                    }
                });
            };

            // AI 학습이 완전히 끝났을 때 버튼 복구 이벤트 연결
            donkeyTrainer.TrainingFinished += () =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnStartTraining.Enabled = true;   // ⭕ 시작 버튼 다시 켜고
                });
            };

            // 폼이 완전히 닫힐 때 백그라운드 좀비 프로세스 방지 안전장치 연결
            this.FormClosing += (s, e) => donkeyTrainer.KillProcess();

            this.pbMainCam.Paint += pbMainCam_Paint;
            InitializeFrameDataContextMenu();

        }

        
        // 탭 순서 제어를 위한 컨트롤 리스트
        private List<Control> _focusOrder;


        /// <summary>
        /// 폼이 처음 로드될 때 실행되는 함수입니다. (중복 제거 완료)
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            //lstTrashItems.MouseDown += lstTrashItems_MouseDown;

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

            // 초기 상태는 비활성화 느낌의 어두운 회색으로 설정
            btnRunSimulator.BackColor = Color.FromArgb(62, 62, 66);
            btnRunSimulator.ForeColor = Color.White; // 글자색 반전으로 가독성 확보

            // Trainer의 학습 완료 이벤트 연결 (이미 되어 있다면 중복 작성 X)
            donkeyTrainer.TrainingFinished += DonkeyTrainer_TrainingFinished;

            // 포커스 가능한 컨트롤들에 TabStop 활성화
            foreach (var c in _focusOrder) c.TabStop = true;

            RefreshSimulatorButtonState();
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

            //아래 윤형규가 추가한 코드, 오류 발생 시 우선 주석처리 할 것
            if (!chkFilterThr.Checked && !chkFilterLargeThr.Checked && !chkFilterLargeAngle.Checked)
            {
                RefreshFrameList(_masterFrameList);
                //필터 없으면 원본 리스트 불러옴
            }

            // 마스터 리스트를 복사해서 필터링을 시작할 임시 리스트 생성
            List<DonkeyFrame> filteredList = new List<DonkeyFrame>(_masterFrameList);
            // 1. Thr = 0 체크박스가 켜져있을 때
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
            RefreshFrameList(filteredList);


            MessageBox.Show($"필터링 완료! {filteredList.Count}개의 데이터가 조건에 맞습니다.", "필터 결과");
        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            // 1. 선택된 항목 인덱스 수집
            List<int> selectedIndices = new List<int>();
            for (int i = 0; i < lstFrameData.Items.Count; i++)
            {
                if (lstFrameData.GetSelected(i)) selectedIndices.Add(i);
            }

            if (selectedIndices.Count == 0) return;

            // 💡 [변경점 1] 삭제 전 확인 메시지 박스
            DialogResult result = MessageBox.Show($"{selectedIndices.Count}개의 파일을 휴지통으로 이동하시겠습니까?",
                                                 "데이터 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return; // '아니오'를 누르면 여기서 종료

            // 💡 [변경점 2] 삭제 후 선택할 인덱스 계산을 위해 마지막 인덱스 기억
            // (지운 항목들이 사라지면 뒤에 있던 항목들이 앞으로 당겨오므로, 
            // 지우기 전 마지막 인덱스를 타겟으로 하면 자연스럽게 다음 파일이 선택됩니다.)
            int lastSelectedIndex = selectedIndices.Max();

            // 2. 선택된 프레임 추출
            List<DonkeyFrame> selectedFrames = selectedIndices
                .Select(index => _displayedFrameList[index])
                .ToList();

            // 3. 메모리 리스트에서 제거
            _displayedFrameList.RemoveAll(frame => selectedFrames.Contains(frame));
            _masterFrameList.RemoveAll(frame => selectedFrames.Contains(frame));

            // 4. 휴지통 리스트로 이동
            _trashFrameList.AddRange(selectedFrames);

            // 5. UI 갱신 (기존 함수 호출)
            RefreshFrameList(_displayedFrameList);
            UpdateTrashListUI();

            // 💡 [변경점 3] 삭제 후 다음 항목 선택 로직
            if (lstFrameData.Items.Count > 0)
            {
                // 1. lastSelectedIndex가 현재 리스트 크기보다 크면 맨 끝으로 보정
                int targetIndex = Math.Min(lastSelectedIndex, lstFrameData.Items.Count - 1);

                // 2. 해당 인덱스 선택
                lstFrameData.SelectedIndex = targetIndex;
            }
        }

        private void lstFrameData_SelectedIndexChanged(object sender, EventArgs e) // 리스트박스에서 선택이 바뀔 때마다 해당 프레임을 미리보기로 보여주는 이벤트 핸들러
        {
            if (lstFrameData.SelectedIndex == -1) return;

            // 재생 코드가 선택을 바꾸는 중이면 start/end를 건드리지 않음
            if (_isPlaybackSelecting) return;

            _isViewingTrash = false;
            lstTrashItems.ClearSelected();

            // 💡 [핵심] 정상 화면으로 돌아왔으니 재생 버튼 잠금 해제
            btnPlay.Enabled = true;

            start = lstFrameData.SelectedIndex;
            end = lstFrameData.SelectedIndex;

            DisplayFrame(lstFrameData.SelectedIndex);
        }

        private void InitializeFrameDataContextMenu()
        {
            var menu = new ContextMenuStrip();

            var showFirstItem = new ToolStripMenuItem("선택 첫 프레임 보기", null, (s, e) => ShowSelectedFrame(first: true));
            var showLastItem = new ToolStripMenuItem("선택 마지막 프레임 보기", null, (s, e) => ShowSelectedFrame(first: false));
            //var setRangeItem = new ToolStripMenuItem("선택 범위를 시작/종료 지점으로 설정", null, (s, e) => SetSelectedRangeAsStartEnd());
            //var copyInfoItem = new ToolStripMenuItem("선택 정보 복사", null, (s, e) => CopySelectedFrameInfo());
            var deleteItem = new ToolStripMenuItem("선택 프레임 휴지통으로 이동", null, (s, e) => btnDeleteData_Click(this, EventArgs.Empty));
            var clearSelectionItem = new ToolStripMenuItem("선택 해제", null, (s, e) => lstFrameData.ClearSelected());

            menu.Items.AddRange(new ToolStripItem[]
            {
                showFirstItem,
                showLastItem,
                new ToolStripSeparator(),
                //setRangeItem,
                //copyInfoItem,
                new ToolStripSeparator(),
                deleteItem,
                clearSelectionItem
            });

            menu.Opening += (s, e) =>
            {
                int selectedCount = lstFrameData.SelectedIndices.Count;
                bool hasSelection = selectedCount > 0;

                e.Cancel = !hasSelection;
                showFirstItem.Enabled = hasSelection;
                showLastItem.Enabled = selectedCount > 1;
                //setRangeItem.Enabled = hasSelection;
                //copyInfoItem.Enabled = hasSelection;
                deleteItem.Enabled = hasSelection;
                deleteItem.Text = selectedCount > 1
                    ? $"선택 프레임 {selectedCount}개 휴지통으로 이동"
                    : "선택 프레임 휴지통으로 이동";
                clearSelectionItem.Enabled = hasSelection;
            };

            lstFrameData.ContextMenuStrip = menu;
            lstFrameData.MouseDown += lstFrameData_MouseDown;
        }

        private void lstFrameData_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            int index = lstFrameData.IndexFromPoint(e.Location);
            if (index < 0 || index >= lstFrameData.Items.Count) return;

            if (!lstFrameData.GetSelected(index))
            {
                lstFrameData.ClearSelected();
                lstFrameData.SelectedIndex = index;
            }

            lstFrameData.Focus();
        }

        private List<int> GetSelectedFrameIndices()
        {
            return lstFrameData.SelectedIndices
                .Cast<int>()
                .Where(index => index >= 0 && index < _displayedFrameList.Count)
                .OrderBy(index => index)
                .ToList();
        }

        private void ShowSelectedFrame(bool first)
        {
            List<int> selectedIndices = GetSelectedFrameIndices();
            if (selectedIndices.Count == 0) return;

            int index = first ? selectedIndices.First() : selectedIndices.Last();
            DisplayFrame(index);
        }

        private void SetSelectedRangeAsStartEnd()
        {
            List<int> selectedIndices = GetSelectedFrameIndices();
            if (selectedIndices.Count == 0) return;

            start = selectedIndices.First();
            end = selectedIndices.Last();
            DisplayFrame(start);
        }

        private void CopySelectedFrameInfo()
        {
            List<int> selectedIndices = GetSelectedFrameIndices();
            if (selectedIndices.Count == 0) return;

            string text = string.Join(Environment.NewLine, selectedIndices.Select(index =>
            {
                DonkeyFrame frame = _displayedFrameList[index];
                return $"Frame {frame.FrameIndex}\tAngle {frame.Angle:F3}\tThrottle {frame.Throttle:F3}\t{frame.FullImagePath}";
            }));

            Clipboard.SetText(text);
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

            // 💡 [안전장치] 휴지통을 보고 있을 때는 재생 버튼 로직 실행 무시
            if (_isViewingTrash) return;

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
                // 💡 [안전장치] 휴지통 뷰가 아닐 때만 스페이스바로 재생 가능
                if (!_isViewingTrash)
                {
                    btnPlay.PerformClick();
                }
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
            // 💡 [안전장치] 타이머가 돌고 있어도 휴지통 뷰라면 즉시 정지
            if (_isViewingTrash || !HasDisplayedFrames(false))
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
                lstFrameData.Items.Clear();

                // 다시 변하지 않는 고유 번호인 FrameIndex를 사용합니다.
                string[] items = _displayedFrameList
                    .Select(f => $"{prefix}Frame {f.FrameIndex}")
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

            // 🌟 휴지통에서 넘어왔을 때 잠긴 트랙바를 다시 풀어줍니다.
            trkFrameSlider.Enabled = true;

            DonkeyFrame selectedFrame = _displayedFrameList[index];

            _pictureHandler.LoadImageToPictureBox(pbMainCam, selectedFrame.FullImagePath);

            _currentFrameIndex = index;

            lblAngle.Text = $"방향: {selectedFrame.Angle:F3}";
            lblThrottleTop.Text = $"속도: {selectedFrame.Throttle:F3}";
            lblFrameIndex.Text = $"프레임 인덱스: {index + 1}/{_displayedFrameList.Count} (원본 {selectedFrame.FrameIndex})";
            lblTimestamp.Text = string.IsNullOrWhiteSpace(selectedFrame.SessionId)
                ? selectedFrame.DataTypeSummary
                : selectedFrame.SessionId;

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
            // 1. [핵심] 휴지통에 있는 파일들을 여기서 실제 삭제
            if (_trashFrameList.Count > 0)
            {
                if (MessageBox.Show($"휴지통에 있는 {_trashFrameList.Count}개의 파일을 영구 삭제하고 학습을 시작할까요?",
                                    "최종 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _fileRemover.RemoveFramesFromCatalogs(_masterFrameList, _trashFrameList);
                    _trashFrameList.Clear();
                    UpdateTrashListUI();
                }
                else
                {
                    return; // 사용자가 취소하면 학습 시작 안 함
                }
            }
            lossPoints.Clear();
            if (pbChart != null) pbChart.Image = null;
            // 기존 학습 시작 로직...
            btnStartTraining.Enabled = false;
            btnEndTraining.Enabled = true;

            // 학습 로그 패널은 켜고, 주행 모니터 패널은 끕니다.
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = true;

            btnStartTraining.BackColor = Color.FromArgb(62, 62, 66);
            btnEndTraining.BackColor = Color.FromArgb(211, 47, 47);

            // 버튼 색상 변경
            btnViewLog.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);

            rtbTrainLog.Clear();
            rtbTrainLog.AppendText(" AI 학습 연동을 시작합니다...\r\n");

            string pythonPath = "wsl.exe";
            string mycarDir = " mycar";

            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir));
        }
        private void DrawRealTimeChart()
        {
            // Nullable 잔소리를 완벽하게 회피하기 위해 컴파일러 안심 장치 마련 후 진행
            if (pbChart == null || lossPoints.Count == 0) return;

            Bitmap bmp = new Bitmap(pbChart.Width, pbChart.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float maxLoss = 1.0f;
                foreach (var loss in lossPoints)
                {
                    if (loss > maxLoss) maxLoss = loss;
                }

                float xInterval = (float)pbChart.Width / (lossPoints.Count <= 1 ? 1 : lossPoints.Count - 1);

                using (Pen pen = new Pen(Color.Crimson, 3f))
                {
                    for (int i = 0; i < lossPoints.Count - 1; i++)
                    {
                        float x1 = i * xInterval;
                        float y1 = pbChart.Height - (lossPoints[i] / maxLoss * (pbChart.Height - 20)) - 10;

                        float x2 = (i + 1) * xInterval;
                        float y2 = pbChart.Height - (lossPoints[i + 1] / maxLoss * (pbChart.Height - 20)) - 10;

                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }

            if (pbChart.Image != null) pbChart.Image.Dispose();
            pbChart.Image = bmp;
        }
        private void btnViewMonitor_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = false;
            pnlCamView.Visible = true;
            pnlTrainingLog.Visible = false;

            // 버튼 색상 변경
            btnOpenManual.BackColor = Color.FromArgb(62, 62, 66);
            btnViewMonitor.BackColor = Color.FromArgb(0, 122, 204);
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = false;
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = true;

            // 버튼 색상 변경
            btnOpenManual.BackColor = Color.FromArgb(62, 62, 66);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);
            btnViewLog.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void btnStopTraining_Click(object sender, EventArgs e)
        {
            // 1. 실제 학습 프로세스 강제 종료
            donkeyTrainer.KillProcess();
            btnEndTraining.Visible = true;   // (종료 버튼이 폼에 있다면 보이게 설정)

            // 3. 로그 출력
            rtbTrainLog.AppendText("\r\n🛑 사용자의 요청으로 AI 학습을 일시 중단했습니다...\r\n");
        }

        private async void btnRestartTraining_Click(object sender, EventArgs e)
        {
            // 1. UI 버튼 상태 변경

            // 2. 로그 출력
            rtbTrainLog.AppendText("\r\n▶️ AI 학습을 이어서 재시작합니다...\r\n");

            // 3. 학습 프로세스 다시 실행 (btnStart_Click과 동일한 로직)
            string pythonPath = "wsl.exe";
            string mycarDir = "/home/jaeseo03/mycar";

            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir));
        }

        private void btnEndTraining_Click(object sender, EventArgs e)
        {
            // 1. 혹시라도 프로세스가 켜져 있다면 확실하게 사살 (안전장치)
            donkeyTrainer.KillProcess();

            // 2. UI 버튼 초기 상태로 복구 (처음 화면처럼)
            btnEndTraining.Enabled = false;
            btnStartTraining.Enabled = true; // 시작 버튼 다시 깨우기

            btnStartTraining.BackColor = Color.FromArgb(0, 122, 204);
            btnEndTraining.BackColor = Color.FromArgb(62, 62, 66);

            // 3. 로그 출력
            rtbTrainLog.AppendText("\r\n⏹️ AI 학습 연동이 완전히 종료되었습니다. 새로운 학습을 준비합니다.\r\n");
        }

        private void btnRestoreData_Click(object sender, EventArgs e)
        {
            List<int> selectedIndices = new List<int>();
            for (int i = 0; i < lstTrashItems.Items.Count; i++)
            {
                if (lstTrashItems.GetSelected(i)) selectedIndices.Add(i);
            }

            if (selectedIndices.Count == 0) return;

            List<DonkeyFrame> framesToRestore = selectedIndices
                .Select(index => _trashFrameList[index])
                .ToList();

            // 1. 데이터 복원 및 리스트 갱신
            _trashFrameList.RemoveAll(f => framesToRestore.Contains(f));
            _masterFrameList.AddRange(framesToRestore);
            _masterFrameList = _masterFrameList.OrderBy(f => f.FrameIndex).ToList();
            _displayedFrameList = _masterFrameList;

            RefreshFrameList(_displayedFrameList);
            UpdateTrashListUI();

            // 2. [핵심] 복원한 파일 중 하나(첫 번째)를 '마지막으로 본 파일'로 지정
            _lastViewedFrameId = framesToRestore[0].FrameIndex;

            // 3. 복원 후 즉시 휴지통을 닫는 로직 (btnCloseTrash_Click 내용을 여기서 처리)
            pnlTrash.Visible = false;
            _isViewingTrash = false;

            // 4. 위치 복구 및 재생 상태 복구
            int newIndex = _displayedFrameList.FindIndex(f => f.FrameIndex == _lastViewedFrameId);
            if (newIndex >= 0)
            {
                lstFrameData.SelectedIndex = newIndex;
                DisplayFrame(newIndex);
            }

            if (_wasPlaying) btnPlay.PerformClick();
            else btnPlay.Enabled = true;

            pbMainCam.Invalidate();
        }

        // 휴지통 리스트박스 갱신용 함수 (추가하세요)
        private void UpdateTrashListUI()
        {
            lstTrashItems.BeginUpdate();
            lstTrashItems.Items.Clear();
            foreach (var frame in _trashFrameList)
            {
                lstTrashItems.Items.Add($"Frame {frame.FrameIndex}");
            }
            lstTrashItems.EndUpdate();
        }

        // 🌟🌟🌟 궤적 부드러움 보정 & HUD (휴지통 시각 효과 포함) 🌟🌟🌟
        private void pbMainCam_Paint(object sender, PaintEventArgs e)
        {
            if (pbMainCam.Image == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float width = pbMainCam.Width;
            float height = pbMainCam.Height;

            // =========================================================
            // 💡 [핵심] 휴지통 이미지를 볼 때는 HUD 궤적을 끄고 경고 문구만 크게 띄움
            // =========================================================
            if (_isViewingTrash)
            {
                string warningText = "삭제 대기 중인 항목입니다";

                // 그리기 작업 안에서만 사용할 중간 크기(14pt) 폰트 생성 (자동 리소스 해제)
                using (Font mediumFont = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    SizeF wSize = g.MeasureString(warningText, mediumFont);

                    // 우측 상단 배치를 위한 좌표 계산 (우측 여백 10px, 상단 여백 10px)
                    float x = width - wSize.Width - 10;
                    float y = 10;

                    // 글씨 가독성을 위해 글씨 크기에 딱 맞는 작은 반투명 검은색 배경 박스 그리기
                    RectangleF bgRect = new RectangleF(x - 5, y - 3, wSize.Width + 10, wSize.Height + 6);
                    using (SolidBrush bgBoxBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                    {
                        g.FillRectangle(bgBoxBrush, bgRect);
                    }

                    // 토마토(붉은색 계열) 색상으로 우측 상단에 글씨 그리기
                    g.DrawString(warningText, mediumFont, Brushes.Tomato, x, y);
                }

                return; // 💥 여기서 return하여 아래의 테슬라 궤적과 속도계를 그리지 않음!
            }

            // =========================================================
            // 아래는 일반 화면일 때만 그려지는 테슬라 궤적 및 속도계 로직
            // =========================================================
            if (_displayedFrameList == null || _displayedFrameList.Count == 0) return;

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

            float throttle = (float)_displayedFrameList[_currentFrameIndex].Throttle;
            int displaySpeed = (int)Math.Round(Math.Abs(throttle) * 100);
            float hudRight = width - 50;
            float hudBottom = height - 20;
            string speedText = displaySpeed.ToString();
            SizeF size = g.MeasureString(speedText, _bigFont);

            float textX = hudRight - size.Width;
            float textY = hudBottom - size.Height - 15;

            g.DrawString(speedText, _bigFont, _shadowBrush, new PointF(textX + 2, textY + 2));
            g.DrawString(speedText, _bigFont, Brushes.White, new PointF(textX, textY));
            g.DrawString("Km/h", _smallFont, Brushes.LightGray, new PointF(textX + size.Width - 10, textY + 15));

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

        // 메인 화면 버튼 클릭 시
        private void btnOpenTrash_Click(object sender, EventArgs e)
        {
            // 1. 상태 저장 및 재생 정지
            _wasPlaying = _playbackTimer.Enabled;
            if (_wasPlaying) StopPlayback();

            // 2. [변경] 인덱스가 아니라 고유 ID를 저장
            if (lstFrameData.SelectedIndex >= 0 && lstFrameData.SelectedIndex < _displayedFrameList.Count)
            {
                _lastViewedFrameId = _displayedFrameList[lstFrameData.SelectedIndex].FrameIndex;
            }

            // 3. 휴지통 패널 표시
            pnlTrash.Visible = true;
            pnlTrash.BringToFront();

            if (lstTrashItems.Items.Count > 0)
            {
                lstTrashItems.SelectedIndex = 0;
                // 선택 이벤트 강제 호출 (이미 잘 작성하셨습니다)
                lstTrashItems_SelectedIndexChanged(null, null);
            }
            else
            {
                ClearFramePreview();
            }
        }

        // 휴지통 내부 닫기 버튼 클릭 시
        private void btnCloseTrash_Click(object sender, EventArgs e)
        {
            // 1. 휴지통 패널 숨기기
            pnlTrash.Visible = false;
            _isViewingTrash = false;

            // 2. [변경] 저장된 ID로 현재 리스트에서 위치를 다시 찾기
            if (_lastViewedFrameId != -1)
            {
                int foundIndex = _displayedFrameList.FindIndex(f => f.FrameIndex == _lastViewedFrameId);

                if (foundIndex >= 0)
                {
                    lstFrameData.SelectedIndex = foundIndex;
                    DisplayFrame(foundIndex); // 화면 다시 그리기
                }
            }

            // 3. 재생 상태 복구
            if (_wasPlaying)
            {
                btnPlay.PerformClick();
            }
            else
            {
                btnPlay.Enabled = true;
            }

            pbMainCam.Invalidate();
        }

        // 🌟 [새로 추가] 휴지통 리스트박스 클릭 이벤트
        private void lstTrashItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTrashItems.SelectedIndex == -1) return;

            // 휴지통 뷰 모드로 전환하고 메인 리스트 선택 해제
            _isViewingTrash = true;
            lstFrameData.ClearSelected();

            // 💡 [핵심] 재생 중이었다면 멈추고, 재생 버튼 클릭 불가능하게 잠금
            if (_playbackTimer.Enabled) StopPlayback();
            btnPlay.Enabled = false;

            DisplayTrashFrame(lstTrashItems.SelectedIndex);
        }

        // 🌟 [새로 추가] 휴지통 전용 이미지 표시 함수
        private void DisplayTrashFrame(int index)
        {
            if (_trashFrameList == null || index < 0 || index >= _trashFrameList.Count) return;

            DonkeyFrame trashFrame = _trashFrameList[index];

            // 사진 로드
            _pictureHandler.LoadImageToPictureBox(pbMainCam, trashFrame.FullImagePath);

            // 라벨들도 붉은색 느낌이나 [휴지통] 이라는 텍스트를 추가하여 명확히 구별
            lblAngle.Text = $"방향: {trashFrame.Angle:F3}";
            lblThrottleTop.Text = $"속도: {trashFrame.Throttle:F3}";
            lblFrameIndex.Text = $"[삭제 대기] 프레임 인덱스: {index + 1}/{_trashFrameList.Count} (원본 {trashFrame.FrameIndex})";
            lblTimestamp.Text = "휴지통 항목";

            // 트랙바 조작 방지를 위해 잠시 비활성화 하거나 값 0 처리 (선택사항)
            trkFrameSlider.Enabled = false;

            pbMainCam.Invalidate(); // Paint 이벤트 호출
        }

        private void DonkeyTrainer_TrainingFinished()
        {
            // 백그라운드 스레드에서 UI를 건드리면 터지므로 Invoke 사용
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(DonkeyTrainer_TrainingFinished));
                return;
            }

            // 학습이 완료되면 아주 잠깐의 파일 저장 시간을 고려해 새로고침 호출
            RefreshSimulatorButtonState();

            rtbTrainLog.AppendText("💡 자율주행 모니터 버튼이 활성화되었습니다! 버튼을 눌러 바로 확인해보세요.\r\n");


        }

        private void btnRunSimulator_Click(object sender, EventArgs e)
        {
            // 1. 모델 파일이 저장되는 WSL 우분투 경로 지정
            string wslModelsRoot = @"\\wsl.localhost\Ubuntu-22.04\home\jaeseo03\mycar\models\";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "자율주행에 사용할 모델 파일(.h5)을 선택하세요";
                ofd.Filter = "DonkeyCar Model (*.h5)|*.h5";

                // 기존에 학습된 폴더가 실제로 존재한다면 탐색기 기본 위치로 지정
                // (학습 전이라 폴더가 비어있어도 창은 정상적으로 열립니다.)
                if (System.IO.Directory.Exists(wslModelsRoot))
                {
                    ofd.InitialDirectory = wslModelsRoot;
                }

                // 사용자가 파일 선택을 완료하고 '열기'를 눌렀을 때만 구동 프로세스 진입
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 경로를 제외한 순수 파일명만 추출 (예: mypilot.h5, model_202604.h5 등)
                    string selectedFileName = System.IO.Path.GetFileName(ofd.FileName);

                    try
                    {
                        rtbTrainLog.AppendText($"\r\n🚗 자율주행 모니터링 시스템을 구동합니다... (선택된 모델: {selectedFileName})\r\n");

                        // -------------------------------------------------------------
                        // [신규 로직] 0. 좀비 프로세스 자동 청소 (포트 8887 초기화)
                        // -------------------------------------------------------------
                        rtbTrainLog.AppendText("0. 이전 주행 서버 포트(8887)를 초기화합니다...\r\n");
                        System.Diagnostics.ProcessStartInfo killInfo = new System.Diagnostics.ProcessStartInfo();
                        killInfo.FileName = "wsl.exe";
                        killInfo.Arguments = "-d Ubuntu-22.04 -e bash -c \"fuser -k 8887/tcp || true\"";
                        killInfo.CreateNoWindow = true; // 검은 창 숨기기
                        killInfo.UseShellExecute = false;

                        using (var killProc = System.Diagnostics.Process.Start(killInfo))
                        {
                            // 포트가 완전히 닫히고 다음 서버가 켜질 수 있도록 넉넉히 1.5초(1500ms) 대기합니다.
                            killProc.WaitForExit(1500);
                        }

                        // -------------------------------------------------------------
                        // ① 우분투(WSL2) 자율주행 drive 서버 실행 (터미널 창 표시)
                        // -------------------------------------------------------------
                        System.Diagnostics.ProcessStartInfo wslInfo = new System.Diagnostics.ProcessStartInfo();
                        wslInfo.FileName = "wsl.exe";

                        // 💡 하드코딩된 mypilot.h5 대신 사용자가 선택한 {selectedFileName}을 동적으로 매핑합니다.
                        wslInfo.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '/home/jaeseo03/mycar' && export PYTHONUNBUFFERED=1 && /home/jaeseo03/miniconda3/envs/e2e_env/bin/python manage.py drive --model=./models/{selectedFileName} --type=linear\"";
                        wslInfo.CreateNoWindow = false;
                        wslInfo.UseShellExecute = true;

                        System.Diagnostics.Process.Start(wslInfo);
                        rtbTrainLog.AppendText($"1. WSL2 자율주행 주행 서버 구동 시작 ({selectedFileName}).\r\n");

                        // -------------------------------------------------------------
                        // ② 웹사이트(크롬/엣지 등 기본 브라우저) 자동 팝업
                        // -------------------------------------------------------------
                        System.Diagnostics.ProcessStartInfo browserInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "http://localhost:8887",
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(browserInfo);
                        rtbTrainLog.AppendText("2. 웹 컨트롤러 브라우저 화면 팝업 완료.\r\n");

                        rtbTrainLog.AppendText("🎉 모든 모니터링 장치가 준비되었습니다! 시뮬레이터에서 'Full Auto'로 변경하세요.\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"구동 중 치명적 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                }
            }
        }

        private void RefreshSimulatorButtonState()
        {
            // 백그라운드 스레드에서 호출될 경우를 대비한 Invoke 처리
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshSimulatorButtonState));
                return;
            }

            // 윈도우 경로 기준으로 리눅스 안에 실제 파일이 존재하는지 검사
            if (System.IO.File.Exists(_modelWinPath))
            {
                _isTrainingComplete = true;
                btnRunSimulator.BackColor = Color.FromArgb(0, 122, 204); // 활성화: 파란색
            }
            else
            {
                _isTrainingComplete = false;
                btnRunSimulator.BackColor = Color.FromArgb(62, 62, 66);   // 비활성화: 회색
            }
        }

        private void btnOpenManual_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = true;
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = false;

            // 버튼 색상 변경
            btnOpenManual.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnGoHome_Click(object sender, EventArgs e)
        {
            // 현재 메모리에 켜져 있는 런처 폼을 검색합니다.
            Form launcherForm = Application.OpenForms["LauncherForm"];

            if (launcherForm != null)
            {
                // 💡 홈으로 돌아갈 때도 창의 위치를 동기화하여 자연스러운 전환 유도
                launcherForm.Location = this.Location;
                launcherForm.Show();
            }
            else
            {
                // 혹시나 런처가 메모리에서 해제되었다면 새로 생성해서 현재 위치에 띄움
                LauncherForm newLauncher = new LauncherForm();
                newLauncher.StartPosition = FormStartPosition.Manual;
                newLauncher.Location = this.Location;
                newLauncher.Show();
            }

            // 현재 메인 관리 시스템 창(Form1)은 닫습니다.
            this.Close();

        }

        private void lstTrashItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) // 우클릭 시
            {
                int index = lstTrashItems.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    // 우클릭한 항목이 이미 선택된 상태가 아니라면 새로 선택
                    if (!lstTrashItems.GetSelected(index))
                    {
                        lstTrashItems.ClearSelected();
                        lstTrashItems.SetSelected(index, true);
                    }
                }
            }
        }
    }
}
