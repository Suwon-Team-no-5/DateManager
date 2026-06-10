#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices; // 이 줄을 코드 맨 위에 추가하세요.
using System.Windows.Forms;
using System.Diagnostics;  // 💡 Process 및 ProcessStartInfo를 쓰기 위해 필요!
using System.Text.Json;    // 💡 JsonSerializerOptions를 쓰기 위해 필요!
using System.IO;           // 💡 StreamReader를 쓰기 위해 필요!


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
        // ==========================================
        // ✨ [추가] AI 예측 시각화를 위한 멤버 변수
        // ==========================================
        public class AiPredictFrame
        {
            public int FrameIndex { get; set; } = -1; // 식별자
            public float Angle { get; set; }
            public float Throttle { get; set; }
        }

        private string ConvertToLinuxPath(string winPath)
        {
            if (string.IsNullOrWhiteSpace(winPath)) return winPath ?? "";
            try
            {
                // 통일: 역슬래시 -> 슬래시
                string p = winPath.Replace("\\", "/");

                // WSL UNC 형태: //wsl.localhost/Ubuntu-22.04/home/...
                if (p.StartsWith("//wsl.localhost/", StringComparison.OrdinalIgnoreCase) ||
                    p.StartsWith("/wsl.localhost/", StringComparison.OrdinalIgnoreCase))
                {
                    // 제거: //wsl.localhost/<distro>
                    string rest = p.StartsWith("//wsl.localhost/", StringComparison.OrdinalIgnoreCase)
                        ? p.Substring("//wsl.localhost/".Length)
                        : p.Substring("/wsl.localhost/".Length);

                    int idx = rest.IndexOf('/');
                    if (idx >= 0)
                    {
                        // rest = "Ubuntu-22.04/home/jaeseo03/..."
                        // 반환: "/home/jaeseo03/..."
                        return rest.Substring(idx);
                    }
                    return "/" + rest;
                }

                // 드라이브 문자 형식 C:/...
                if (p.Length >= 2 && char.IsLetter(p[0]) && p[1] == ':')
                {
                    char drive = char.ToLowerInvariant(p[0]);
                    string rest = p.Substring(2);
                    return $"/mnt/{drive}{rest}".Replace("//", "/");
                }

                return p;
            }
            catch
            {
                return winPath ?? "";
            }
        }

        private List<AiPredictFrame> _aiPredictedList = new List<AiPredictFrame>();
        private readonly SolidBrush _barLimeBrush = new SolidBrush(Color.LimeGreen); // AI 속도바용 브러시

        private List<DonkeyFrame> _trashFrameList = new List<DonkeyFrame>();
        private List<float> lossPoints = new List<float>();

        private bool _isViewingTrash = false;

        private int _lastViewedFrameId = -1;
        private bool _wasPlaying = false; // 💡 [추가] 휴지통 열기 전 재생 중이었는지 저장할 변수

        private bool _isTrainingComplete = false; // 학습 완료 플래그
        private string _modelWinPath = @"\\wsl.localhost\Ubuntu-22.04\home\jaeseo03\mycar\models\mypilot.h5";

        private bool _isSelectingStart = true; // 시작을 입력할 차례인지, 끝을 입력할 차례인지 구분

        private string _selectedModelPath = "";

        // 클래스 필드 쪽에 추가
        private Dictionary<int, AiPredictFrame> _aiPredictedMap = new Dictionary<int, AiPredictFrame>();

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
        /// 폼이 처음 로드될 때 실행되는 함수입니다.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
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

            // Trainer의 학습 완료 이벤트 연결
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


        private List<AiPredictFrame> PredictAllFrames(string tubPath, string modelPath)
        {
            List<AiPredictFrame> predictedList = new List<AiPredictFrame>();

            string pythonPath = "wsl.exe";
            string scriptPath = "/home/jaeseo03/mycar/predict_all.py";

            bool useWsl = pythonPath != null && pythonPath.IndexOf("wsl", StringComparison.OrdinalIgnoreCase) >= 0;
            string linuxTubPath = useWsl ? ConvertToLinuxPath(tubPath) : tubPath;
            string linuxModelPath = useWsl ? ConvertToLinuxPath(modelPath) : modelPath;

            string linuxPythonEnv = "/home/jaeseo03/miniconda3/envs/e2e_env/bin/python";

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.Arguments = $"-d Ubuntu-22.04 -e bash -lc \"{linuxPythonEnv} '{scriptPath}' '{linuxTubPath}' '{linuxModelPath}'\"";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;

            void AppendLog(string text)
            {
                try
                {
                    if (rtbTrainLog == null) return;
                    if (rtbTrainLog.InvokeRequired)
                    {
                        rtbTrainLog.Invoke((MethodInvoker)(() => rtbTrainLog.AppendText(text + Environment.NewLine)));
                    }
                    else
                    {
                        rtbTrainLog.AppendText(text + Environment.NewLine);
                    }
                }
                catch { }
            }

            try
            {
                using (Process process = Process.Start(start))
                {
                    if (process == null) throw new Exception("프로세스 시작 실패");

                    string stdout = process.StandardOutput.ReadToEnd();
                    string stderr = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // 항상 로그 남기기(디버깅용)
                    if (!string.IsNullOrWhiteSpace(stderr))
                    {
                        AppendLog("[predict_all.py STDERR] " + stderr.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(stdout))
                    {
                        AppendLog("[predict_all.py STDOUT] (length: " + stdout.Length + ")");
                    }
                    else
                    {
                        AppendLog("[predict_all.py STDOUT] (empty)");
                    }

                    if (!string.IsNullOrWhiteSpace(stderr))
                    {
                        // 치명적 오류일 가능성 있으므로 사용자에게 보여줌
                        if (!stderr.Contains("WARNING") && !stderr.Contains("tensorflow"))
                        {
                            MessageBox.Show($"AI 예측 스크립트 오류: {stderr}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(stdout))
                    {
                        try
                        {
                            using (var doc = JsonDocument.Parse(stdout))
                            {
                                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var el in doc.RootElement.EnumerateArray())
                                    {
                                        float steering = 0f;
                                        float throttle = 0f;
                                        int frameIndex = -1;
                                        if (el.ValueKind == JsonValueKind.Object)
                                        {
                                            if (el.TryGetProperty("steering", out var sProp))
                                            {
                                                if (sProp.ValueKind == JsonValueKind.Number && sProp.TryGetDouble(out double sVal)) steering = (float)sVal;
                                                else if (sProp.ValueKind == JsonValueKind.String && float.TryParse(sProp.GetString(), out float sParsed)) steering = sParsed;
                                            }
                                            if (el.TryGetProperty("throttle", out var tProp))
                                            {
                                                if (tProp.ValueKind == JsonValueKind.Number && tProp.TryGetDouble(out double tVal)) throttle = (float)tVal;
                                                else if (tProp.ValueKind == JsonValueKind.String && float.TryParse(tProp.GetString(), out float tParsed)) throttle = tParsed;
                                            }
                                            if (el.TryGetProperty("index", out var idxProp) && idxProp.ValueKind == JsonValueKind.Number && idxProp.TryGetInt32(out int idxVal))
    frameIndex = idxVal;
                                        }

                                        predictedList.Add(new AiPredictFrame { FrameIndex = frameIndex, Angle = steering, Throttle = throttle });
                                    }
                                }
                                else
                                {
                                    AppendLog("[predict_all.py] JSON root is not array.");
                                }
                            }

                            AppendLog($"[predict_all.py] parsed {predictedList.Count} prediction items.");
                        }
                        catch (Exception ex)
                        {
                            AppendLog("[predict_all.py] JSON parse error: " + ex.Message);
                            MessageBox.Show($"AI 예측 결과 파싱 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("[PredictAllFrames] 오류: " + ex.Message);
                MessageBox.Show($"AI 예측값 연산 중 오류 발생: {ex.Message}");
            }

            return predictedList;
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

            if (!chkFilterThr.Checked && !chkFilterLargeThr.Checked && !chkFilterLargeAngle.Checked)
            {
                RefreshFrameList(_masterFrameList);
            }

            // 마스터 리스트를 복사해서 필터링을 시작할 임시 리스트 생성
            List<DonkeyFrame> filteredList = new List<DonkeyFrame>(_masterFrameList);

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

            RefreshFrameList(filteredList);
            MessageBox.Show($"필터링 완료! {filteredList.Count}개의 데이터가 조건에 맞습니다.", "필터 결과");
        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            List<int> selectedIndices = new List<int>();
            for (int i = 0; i < lstFrameData.Items.Count; i++)
            {
                if (lstFrameData.GetSelected(i)) selectedIndices.Add(i);
            }

            if (selectedIndices.Count == 0) return;

            DialogResult result = MessageBox.Show($"{selectedIndices.Count}개의 파일을 휴지통으로 이동하시겠습니까?",
                                                 "데이터 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            int targetStartIndex = selectedIndices.Min();

            // UI에서 선택된 항목 -> DonkeyFrame 객체 목록
            List<DonkeyFrame> selectedFrames = selectedIndices
                .Select(index => _displayedFrameList[index])
                .ToList();

            // --- 예측 리스트 동기화: 삭제될 프레임의 master 인덱스를 찾아서 예측값을 보관하고 제거 ---
            // master 인덱스 계산(프레임의 고유 FrameIndex로 매칭)
            var pairs = selectedFrames
                .Select(f => new { Frame = f, MasterIndex = _masterFrameList.FindIndex(m => m.FrameIndex == f.FrameIndex) })
                .Where(p => p.MasterIndex >= 0)
                .OrderByDescending(p => p.MasterIndex) // 내림차순으로 제거(인덱스 무효화 방지)
                .ToList();

            foreach (var p in pairs)
            {
                int frameKey = p.Frame.FrameIndex;
                // 맵에서 안전하게 제거
                _aiPredictedMap.Remove(frameKey);

                // 리스트에도 FrameIndex 매칭 항목이 있으면 제거
                if (_aiPredictedList != null)
                {
                    int idxToRemove = _aiPredictedList.FindIndex(x => x.FrameIndex == frameKey);
                    if (idxToRemove >= 0) _aiPredictedList.RemoveAt(idxToRemove);
                }
            }
            // ------------------------------------------------------------------

            _displayedFrameList.RemoveAll(frame => selectedFrames.Contains(frame));
            _masterFrameList.RemoveAll(frame => selectedFrames.Contains(frame));

            _trashFrameList.AddRange(selectedFrames);

            RefreshFrameList(_displayedFrameList);
            UpdateTrashListUI();
            pbMainCam.Invalidate();

            if (lstFrameData.Items.Count > 0)
            {
                int targetIndex = Math.Min(targetStartIndex, lstFrameData.Items.Count - 1);
                lstFrameData.SelectedIndex = targetIndex;
                lstFrameData.Focus();
            }
        }

        private void lstFrameData_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstFrameData.SelectedIndex == -1) return;
            if (_isPlaybackSelecting) return;

            _isViewingTrash = false;
            lstTrashItems.ClearSelected();

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
            var deleteItem = new ToolStripMenuItem("선택 프레임 휴지통으로 이동", null, (s, e) => btnDeleteData_Click(this, EventArgs.Empty));
            var clearSelectionItem = new ToolStripMenuItem("선택 해제", null, (s, e) => lstFrameData.ClearSelected());

            menu.Items.AddRange(new ToolStripItem[]
            {
                showFirstItem,
                showLastItem,
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
            if (trkFrameSlider.Value >= 0 && trkFrameSlider.Value < _displayedFrameList.Count)
            {
                SelectFrame(trkFrameSlider.Value);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e) { MoveFrame(-100); }
        private void btnNext_Click(object sender, EventArgs e) { MoveFrame(100); }
        private void btnFastRewind_Click(object sender, EventArgs e) { MoveFrame(-1); }
        private void btnFastForward_Click(object sender, EventArgs e) { MoveFrame(1); }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (!HasDisplayedFrames()) return;
            if  (_isViewingTrash) return;

            if (_playbackTimer.Enabled)
            {
                StopPlayback();
                return;
            }

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

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnDeleteData.PerformClick();
                e.Handled = true;
                return;
            }

            if (e.Control && e.KeyCode == Keys.Space)
            {
                int currentIndex = _currentFrameIndex;
                if (currentIndex >= 0)
                {
                    if (_isSelectingStart)
                    {
                        txtStartFrame.Text = currentIndex.ToString();
                        txtStartFrame.BackColor = Color.LightBlue;
                        _isSelectingStart = false;
                    }
                    else
                    {
                        txtEndFrame.Text = currentIndex.ToString();
                        txtEndFrame.BackColor = Color.Salmon;

                        if (_playbackTimer != null && _playbackTimer.Enabled)
                        {
                            btnPlay.PerformClick();
                        }

                        btnSelectRange.PerformClick();

                        _isSelectingStart = true;
                        txtStartFrame.BackColor = Color.White;
                        txtEndFrame.BackColor = Color.White;
                    }
                }
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Space)
            {
                if (!(this.ActiveControl is TextBox))
                {
                    if (!_isViewingTrash)
                    {
                        btnPlay.PerformClick();
                    }
                    e.Handled = true;
                }
                return;
            }

            if (e.KeyCode == Keys.Up) { MoveFocus(-1); e.Handled = true; }
            if (e.KeyCode == Keys.Down) { MoveFocus(1); e.Handled = true; }

            if (e.KeyCode == Keys.Home)
            {
                if (_displayedFrameList != null && _displayedFrameList.Count > 0) SelectFrame(0);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.End)
            {
                if (_displayedFrameList != null && _displayedFrameList.Count > 0) SelectFrame(_displayedFrameList.Count - 1);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Control ctrl = this.ActiveControl;
                    while (ctrl is ContainerControl container && container.ActiveControl != null)
                    {
                        ctrl = container.ActiveControl;
                    }
                    if (ctrl is Button btn)
                    {
                        btn.PerformClick();
                        e.Handled = true;
                        return;
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

        private void PlaybackTimer_Tick(object? sender, EventArgs e)
        {
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

        private void UpdateControlsAfterLoad()
        {
            bool has = _displayedFrameList != null && _displayedFrameList.Count > 0;

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

            if (lstFrameData.SelectedIndex != index)
            {
                lstFrameData.SelectedIndex = index;
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

            _currentFrameIndex = 0;
            pbMainCam.Invalidate();

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

        private int start = 0;
        private int end = 0;

        private void txtStartFrame_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (int.TryParse(tb.Text, out int v))
                {
                    start = Math.Max(0, Math.Min(v, (_displayedFrameList?.Count ?? 1) - 1));
                    tb.Text = start.ToString();
                }
                else
                {
                    tb.Text = start.ToString();
                }
            }
        }

        private void txtEndFrame_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (int.TryParse(tb.Text, out int v))
                {
                    end = Math.Max(0, Math.Min(v, (_displayedFrameList?.Count ?? 1) - 1));
                    tb.Text = end.ToString();
                }
                else
                {
                    tb.Text = end.ToString();
                }
            }
        }

        private void btnSetLeft_Click(object sender, EventArgs e) { start = lstFrameData.SelectedIndex; }
        private void btnSetRight_Click(object sender, EventArgs e) { end = lstFrameData.SelectedIndex; }

        private async void btnStart_Click(object? sender, EventArgs e)
        {
            // 1. 휴지통 파일 처리
            if (_trashFrameList.Count > 0)
            {
                if (MessageBox.Show($"휴지통에 있는 {_trashFrameList.Count}개의 파일을 영구 삭제하고 학습을 시작할까요?",
                                    "최종 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _fileRemover.RemoveFramesFromCatalogs(_masterFrameList, _trashFrameList);
                    _trashFrameList.Clear();
                    UpdateTrashListUI();
                }
                else { return; }
            }

            // 2. 데이터 유무 필수 확인
            if (string.IsNullOrEmpty(_currentCatalogPath))
            {
                MessageBox.Show("데이터가 로드되지 않았습니다. 폴더를 먼저 선택해주세요!");
                return;
            }

            // 🎯 [부활 포인트 1] 학습 버프 가동 시 차트 컴포넌트의 이전 기록을 안전하게 밀어버림!
            lossPoints.Clear();
            if (ChartRealTime != null)
            {
                ChartRealTime.Series.Clear();
                ChartRealTime.ChartAreas.Clear();
                ChartRealTime.Titles.Clear();
            }

            btnStartTraining.Enabled = false;
            btnEndTraining.Enabled = true;
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = true;
            btnStartTraining.BackColor = Color.FromArgb(62, 62, 66);
            btnEndTraining.BackColor = Color.FromArgb(211, 47, 47);
            btnViewLog.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);

            rtbTrainLog.Clear();
            rtbTrainLog.AppendText(" AI 학습 연동을 시작합니다...\r\n");

            string linuxTubPath = ConvertToLinuxPath(_currentCatalogPath);
            string pythonPath = "wsl.exe";
            string mycarDir = "/home/jaeseo03/mycar";

            await Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir, linuxTubPath));
        }

        // 🎯 [부활 포인트 2] 기주님 맞춤형 실시간 꺾은선 다크모드 렌더링 엔진 완벽 복구!
        private void DrawRealTimeChart()
        {
            if (ChartRealTime == null || lossPoints.Count == 0) return;

            if (ChartRealTime.InvokeRequired)
            {
                ChartRealTime.Invoke(new MethodInvoker(DrawRealTimeChart));
                return;
            }

            // -----------------------------------------------------------------
            // 1️⃣ 디자인 및 [왼쪽 스택형 가로 정렬] 세팅 구역 (Y축 범위는 자동 최적화)
            // -----------------------------------------------------------------
            if (ChartRealTime.Series.Count == 0 || ChartRealTime.Series[0].ChartType != System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line)
            {
                ChartRealTime.Series.Clear();
                ChartRealTime.ChartAreas.Clear();
                ChartRealTime.Legends.Clear();
                ChartRealTime.Titles.Clear(); // 상단 오차율 숫자 타이틀 초기화

                // 테슬라 다크모드 배경 색상
                ChartRealTime.BackColor = Color.FromArgb(28, 28, 30);
                ChartRealTime.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                ChartRealTime.BorderlineColor = Color.FromArgb(63, 63, 66);

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("LossArea");
                chartArea.BackColor = Color.FromArgb(16, 16, 18);

                // 📊 [오차율 하강폭 극대화] Y축 범위를 고정하지 않고 자동 최적화(NaN)로 줌인 극대화!
                chartArea.AxisY.IsStartedFromZero = false;
                chartArea.AxisY.Minimum = double.NaN;
                chartArea.AxisY.Maximum = double.NaN;

                // 은은한 그리드 격자선
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(45, 45, 48);
                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(45, 45, 48);

                Font axisFont = new Font("Segoe UI", 9, FontStyle.Bold);
                chartArea.AxisX.LabelStyle.ForeColor = Color.FromArgb(220, 220, 224);
                chartArea.AxisY.LabelStyle.ForeColor = Color.FromArgb(220, 220, 224);
                chartArea.AxisX.LabelStyle.Font = axisFont;
                chartArea.AxisY.LabelStyle.Font = axisFont;

                Font titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                chartArea.AxisX.Title = "Epoch (학습 횟수)";
                chartArea.AxisX.TitleForeColor = Color.White;
                chartArea.AxisX.TitleFont = titleFont;

                // 📊 [기주님 요청] 왼쪽 '오차율' 단어가 절대 잘리지 않게 스택형으로 쌓고 가로로 정렬!
                chartArea.AxisY.Title = "오\n차\n율";
                chartArea.AxisY.TitleForeColor = Color.White;
                chartArea.AxisY.TitleFont = titleFont;
                chartArea.AxisY.TitleAlignment = StringAlignment.Center;
                chartArea.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
                ChartRealTime.ChartAreas.Add(chartArea);

                // 네온 레드 꺾은선 시리즈 디자인
                var series = new System.Windows.Forms.DataVisualization.Charting.Series("LossSeries");
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                series.Color = Color.FromArgb(255, 45, 85);
                series.BorderWidth = 4;

                series.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                series.MarkerSize = 7;
                series.MarkerColor = Color.White;
                series.MarkerBorderColor = Color.FromArgb(255, 45, 85);
                series.MarkerBorderWidth = 2;

                ChartRealTime.Series.Add(series);
            }

            // -----------------------------------------------------------------
            // 2️⃣ 데이터 실시간 주입 및 [상단 실시간 오차율 숫자 띄우기]
            // -----------------------------------------------------------------
            var lossSeries = ChartRealTime.Series["LossSeries"];
            lossSeries.Points.Clear();

            float latestLoss = 0f; // 최신 오차율 값을 담아낼 변수

            for (int i = 0; i < lossPoints.Count; i++)
            {
                float currentLoss = lossPoints[i];
                int epochX = i + 1;

                int pointIndex = lossSeries.Points.AddXY(epochX, currentLoss);
                lossSeries.Points[pointIndex].ToolTip = $"[AI Training 상태]\n▶ Epoch : {epochX} 회차\n▶ Loss : {currentLoss:F4}";

                latestLoss = currentLoss; // 가장 마지막 루프에서 최신 오차율이 저장됨
            }

            // 🎯 [기주님 요청] 차트 스택형 텍스트 바로 위(상단 정중앙)에 현재 실시간 오차율 수치 표시!
            ChartRealTime.Titles.Clear(); // 잔상 제거
            var topTitle = new System.Windows.Forms.DataVisualization.Charting.Title();
            topTitle.Text = $"오차율 : {latestLoss:F4}"; // 소수점 4자리까지 정밀 수치 표기
            topTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold); // 14pt 볼드체
            topTitle.ForeColor = Color.FromArgb(255, 45, 85); // 네온 레드 칼라
            topTitle.Alignment = ContentAlignment.TopCenter; // 차트 상단 정중앙에 배치
            ChartRealTime.Titles.Add(topTitle);

            ChartRealTime.ResetAutoValues();
        }

        private void btnViewMonitor_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = false;
            pnlCamView.Visible = true;
            pnlTrainingLog.Visible = false;

            btnOpenManual.BackColor = Color.FromArgb(62, 62, 66);
            btnViewMonitor.BackColor = Color.FromArgb(0, 122, 204);
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = false;
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = true;

            btnOpenManual.BackColor = Color.FromArgb(62, 62, 66);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);
            btnViewLog.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void btnStopTraining_Click(object sender, EventArgs e)
        {
            donkeyTrainer.KillProcess();
            btnEndTraining.Visible = true;
            rtbTrainLog.AppendText("\r\n🛑 사용자의 요청으로 AI 학습을 일시 중단했습니다...\r\n");
        }

        private async void btnRestartTraining_Click(object sender, EventArgs e)
        {
            rtbTrainLog.AppendText("\r\n▶️ AI 학습을 이어서 재시작합니다...\r\n");

            string pythonPath = "wsl.exe";
            string mycarDir = "/home/jaeseo03/mycar";
            string tubPath = _currentCatalogPath ?? "./data";

            await System.Threading.Tasks.Task.Run(() => donkeyTrainer.StartTraining(pythonPath, mycarDir, tubPath));
        }

        private void btnEndTraining_Click(object sender, EventArgs e)
        {
            donkeyTrainer.KillProcess();

            btnEndTraining.Enabled = false;
            btnStartTraining.Enabled = true;

            btnStartTraining.BackColor = Color.FromArgb(0, 122, 204);
            btnEndTraining.BackColor = Color.FromArgb(62, 62, 66);

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

            _trashFrameList.RemoveAll(f => framesToRestore.Contains(f));
            _masterFrameList.AddRange(framesToRestore);
            _masterFrameList = _masterFrameList.OrderBy(f => f.FrameIndex).ToList();
            _displayedFrameList = _masterFrameList;

            RefreshFrameList(_displayedFrameList);
            UpdateTrashListUI();

            _lastViewedFrameId = framesToRestore[0].FrameIndex;

            pnlTrash.Visible = false;
            _isViewingTrash = false;

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

        private void pbMainCam_Paint(object? sender, PaintEventArgs e)
        {
            if (pbMainCam.Image == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float width = pbMainCam.Width;
            float height = pbMainCam.Height;

            if (_isViewingTrash)
            {
                string warningText = "삭제 대기 중인 항목입니다";

                using (Font mediumFont = new Font("Segoe UI", 14, FontStyle.Bold))
                {
                    SizeF wSize = g.MeasureString(warningText, mediumFont);
                    float x = width - wSize.Width - 10;
                    float y = 10;

                    RectangleF bgRect = new RectangleF(x - 5, y - 3, wSize.Width + 10, wSize.Height + 6);
                    using (SolidBrush bgBoxBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                    {
                        g.FillRectangle(bgBoxBrush, bgRect);
                    }

                    g.DrawString(warningText, mediumFont, Brushes.Tomato, x, y);
                }
                return;
            }

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
                var futureFrame = _displayedFrameList[futureIndex];
                float targetAngle;
                if (_aiPredictedMap != null && _aiPredictedMap.TryGetValue(futureFrame.FrameIndex, out var pred))
    targetAngle = pred.Angle;
    else
    targetAngle = (float)futureFrame.Angle; // fallback

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

            // AI 예측 경로(연두색) 오버레이: _aiPredictedList가 존재하면 동일한 로직으로 그립니다.
            var aiList = _aiPredictedList; // 로컬 복사로 스레드 안전성 확보
            if (aiList != null && aiList.Count > 0)
            {
                try
                {
                    float curXA = width / 2.0f;
                    float angleSumA = 0f;
                    PointF[] leftA = new PointF[segments + 1];
                    PointF[] rightA = new PointF[segments + 1];

                    leftA[0] = new PointF(curXA - bottomWidth / 2, height);
                    rightA[0] = new PointF(curXA + bottomWidth / 2, height);

                    float smoothedA = 0f;
                    for (int i = 1; i <= segments; i++)
                    {
                        int futureIndex = Math.Min(_currentFrameIndex + (i * lookAheadStep), aiList.Count - 1);
                        float targetAngle = (float)aiList[futureIndex].Angle;
                        smoothedA = (smoothedA * 0.7f) + (targetAngle * 0.3f);
                        angleSumA += smoothedA;

                        float progress = i / (float)segments;
                        float nextY = height - (pathHeight * progress);
                        float nextX = curXA + (angleSumA * maxDeflection);
                        curXA = nextX;
                        float currentWidth = bottomWidth - ((bottomWidth - topWidth) * progress);
                        leftA[i] = new PointF(curXA - currentWidth / 2, nextY);
                        rightA[i] = new PointF(curXA + currentWidth / 2, nextY);
                    }

                    using (SolidBrush aiBrush = new SolidBrush(Color.FromArgb(180, Color.LimeGreen)))
                    {
                        for (int i = 0; i < segments; i++)
                        {
                            PointF[] polyA = { leftA[i], rightA[i], new PointF(rightA[i + 1].X, rightA[i + 1].Y + 5f), new PointF(leftA[i + 1].X, leftA[i + 1].Y + 5f) };
                            g.FillPolygon(aiBrush, polyA);
                        }
                    }
                }
                catch { }
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

            lstFrameData.SelectedIndexChanged -= lstFrameData_SelectedIndexChanged;
            lstFrameData.BeginUpdate();

            try
            {
                SendMessage(lstFrameData.Handle, LB_SETSEL, 1, -1);
            }
            finally
            {
                lstFrameData.EndUpdate();
                lstFrameData.SelectedIndexChanged += lstFrameData_SelectedIndexChanged;
            }

            DisplayFrame(lstFrameData.Items.Count - 1);
        }

        private void btnOpenTrash_Click(object sender, EventArgs e)
        {
            _wasPlaying = _playbackTimer.Enabled;
            if (_wasPlaying) StopPlayback();

            if (lstFrameData.SelectedIndex >= 0 && lstFrameData.SelectedIndex < _displayedFrameList.Count)
            {
                _lastViewedFrameId = _displayedFrameList[lstFrameData.SelectedIndex].FrameIndex;
            }

            if (lstTrashItems.ContextMenuStrip == null)
            {
                ContextMenuStrip ctxTrashMenu = new ContextMenuStrip();
                ToolStripMenuItem menuRestore = new ToolStripMenuItem("선택한 항목 복원");

                menuRestore.Click += (s, ev) =>
                {
                    if (btnRestoreData.Enabled) btnRestoreData.PerformClick();
                };

                ctxTrashMenu.Items.Add(menuRestore);
                lstTrashItems.ContextMenuStrip = ctxTrashMenu;

                lstTrashItems.MouseDown += (snd, me) =>
                {
                    if (me.Button == MouseButtons.Right)
                    {
                        int index = lstTrashItems.IndexFromPoint(me.Location);
                        if (index != ListBox.NoMatches)
                        {
                            if (!lstTrashItems.GetSelected(index))
                            {
                                lstTrashItems.ClearSelected();
                                lstTrashItems.SetSelected(index, true);
                            }
                        }
                        else
                        {
                            if (lstTrashItems.Items.Count > 0)
                            {
                                lstTrashItems.ClearSelected();
                                lstTrashItems.SelectedIndex = lstTrashItems.Items.Count - 1;
                            }
                        }
                        ctxTrashMenu.Show(lstTrashItems, me.Location);
                    }
                };
            }

            pnlTrash.Visible = true;
            pnlTrash.BringToFront();

            if (lstTrashItems.Items.Count > 0)
            {
                lstTrashItems.SelectedIndex = 0;
                lstTrashItems_SelectedIndexChanged(null, null);
            }
            else
            {
                ClearFramePreview();
            }
        }

        private void btnCloseTrash_Click(object sender, EventArgs e)
        {
            pnlTrash.Visible = false;
            _isViewingTrash = false;

            if (_lastViewedFrameId != -1)
            {
                int foundIndex = _displayedFrameList.FindIndex(f => f.FrameIndex == _lastViewedFrameId);
                if (foundIndex >= 0)
                {
                    lstFrameData.SelectedIndex = foundIndex;
                    DisplayFrame(foundIndex);
                }
            }

            if (_wasPlaying) btnPlay.PerformClick();
            else btnPlay.Enabled = true;

            pbMainCam.Invalidate();
        }

        private void lstTrashItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTrashItems.SelectedIndex == -1) return;

            _isViewingTrash = true;
            lstFrameData.ClearSelected();

            if (_playbackTimer.Enabled) StopPlayback();
            btnPlay.Enabled = false;

            DisplayTrashFrame(lstTrashItems.SelectedIndex);
        }

        private void DisplayTrashFrame(int index)
        {
            if (_trashFrameList == null || index < 0 || index >= _trashFrameList.Count) return;

            DonkeyFrame trashFrame = _trashFrameList[index];

            _pictureHandler.LoadImageToPictureBox(pbMainCam, trashFrame.FullImagePath);

            lblAngle.Text = $"방향: {trashFrame.Angle:F3}";
            lblThrottleTop.Text = $"속도: {trashFrame.Throttle:F3}";
            lblFrameIndex.Text = $"[삭제 대기] 프레임 인덱스: {index + 1}/{_trashFrameList.Count} (원본 {trashFrame.FrameIndex})";
            lblTimestamp.Text = "휴지통 항목";

            trkFrameSlider.Enabled = false;
            pbMainCam.Invalidate();
        }

        private void DonkeyTrainer_TrainingFinished()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(DonkeyTrainer_TrainingFinished));
                return;
            }

            RefreshSimulatorButtonState();
            rtbTrainLog.AppendText("💡 자율주행 모니터 버튼이 활성화되었습니다! 버튼을 눌러 바로 확인해보세요.\r\n");
        }

        private void btnRunSimulator_Click(object sender, EventArgs e)
        {
            string wslModelsRoot = @"\\wsl.localhost\Ubuntu-22.04\home\jaeseo03\mycar\models\";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "자율주행에 사용할 모델 파일(.h5)을 선택하세요";
                ofd.Filter = "DonkeyCar Model (*.h5)|*.h5";

                if (System.IO.Directory.Exists(wslModelsRoot))
                {
                    ofd.InitialDirectory = wslModelsRoot;
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = System.IO.Path.GetFileName(ofd.FileName);

                    try
                    {
                        rtbTrainLog.AppendText($"\r\n🚗 자율주행 모니터링 시스템을 구동합니다... (선택된 모델: {selectedFileName})\r\n");

                        rtbTrainLog.AppendText("0. 이전 주행 서버 포트(8887)를 초기화합니다...\r\n");
                        System.Diagnostics.ProcessStartInfo killInfo = new System.Diagnostics.ProcessStartInfo();
                        killInfo.FileName = "wsl.exe";
                        killInfo.Arguments = "-d Ubuntu-22.04 -e bash -c \"fuser -k 8887/tcp || true\"";
                        killInfo.CreateNoWindow = true;
                        killInfo.UseShellExecute = false;

                        using (var killProc = System.Diagnostics.Process.Start(killInfo))
                        {
                            killProc.WaitForExit(1500);
                        }

                        System.Diagnostics.ProcessStartInfo wslInfo = new System.Diagnostics.ProcessStartInfo();
                        wslInfo.FileName = "wsl.exe";
                        wslInfo.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '/home/jaeseo03/mycar' && export PYTHONUNBUFFERED=1 && /home/jaeseo03/miniconda3/envs/e2e_env/bin/python manage.py drive --model=./models/{selectedFileName} --type=linear\"";
                        wslInfo.CreateNoWindow = false;
                        wslInfo.UseShellExecute = true;

                        System.Diagnostics.Process.Start(wslInfo);
                        rtbTrainLog.AppendText($"1. WSL2 자율주행 주행 서버 구동 시작 ({selectedFileName}).\r\n");

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
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshSimulatorButtonState));
                return;
            }

            if (System.IO.File.Exists(_modelWinPath))
            {
                _isTrainingComplete = true;
                btnRunSimulator.BackColor = Color.FromArgb(0, 122, 204);
            }
            else
            {
                _isTrainingComplete = false;
                btnRunSimulator.BackColor = Color.FromArgb(62, 62, 66);
            }
        }

        private void btnOpenManual_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = true;
            pnlCamView.Visible = false;
            pnlTrainingLog.Visible = false;

            btnOpenManual.BackColor = Color.FromArgb(0, 122, 204);
            btnViewMonitor.BackColor = Color.FromArgb(62, 62, 66);
            btnViewLog.BackColor = Color.FromArgb(62, 62, 66);
        }

        private void btnGoHome_Click(object sender, EventArgs e)
        {
            bool isMaximized = this.WindowState == FormWindowState.Maximized;

            Rectangle targetBounds = isMaximized
                ? this.RestoreBounds
                : this.Bounds;

            LauncherForm launcherForm = Application.OpenForms["LauncherForm"] as LauncherForm;

            if (launcherForm == null)
            {
                launcherForm = new LauncherForm();
            }

            launcherForm.StartPosition = FormStartPosition.Manual;

            // 기존 LauncherForm이 Maximized로 숨겨져 있었을 수 있으니 먼저 Normal로 초기화
            launcherForm.WindowState = FormWindowState.Normal;
            launcherForm.Bounds = targetBounds;

            launcherForm.Show();

            if (isMaximized)
            {
                launcherForm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                // Show 이후에 한 번 더 Normal + Bounds 적용
                launcherForm.BeginInvoke(new Action(() =>
                {
                    launcherForm.WindowState = FormWindowState.Normal;
                    launcherForm.Bounds = targetBounds;
                }));
            }

            this.Close();
        }

        private void lstTrashItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lstTrashItems.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    if (!lstTrashItems.GetSelected(index))
                    {
                        lstTrashItems.ClearSelected();
                        lstTrashItems.SetSelected(index, true);
                    }
                }
            }
        }


        private void btnSelectRange_Click(object sender, EventArgs e)
        {
            if (lstFrameData.Items.Count == 0) return;

            if (int.TryParse(txtStartFrame.Text, out int start) && int.TryParse(txtEndFrame.Text, out int end))
            {
                if (_playbackTimer != null && _playbackTimer.Enabled)
                {
                    btnPlay.PerformClick();
                }

                int s = Math.Max(0, Math.Min(start, end));
                int eIdx = Math.Min(lstFrameData.Items.Count - 1, Math.Max(start, end));

                for (int i = s; i <= eIdx; i++)
                {
                    lstFrameData.SetSelected(i, true);
                }

                txtStartFrame.Clear();
                txtEndFrame.Clear();

                _isSelectingStart = true;
                txtStartFrame.BackColor = Color.White;
                txtEndFrame.BackColor = Color.White;
            }
            else
            {
                MessageBox.Show("올바른 프레임 숫자를 입력해주세요!", "입력 오류");
            }
        }

        private async void btnCompareDataset_Click(object sender, EventArgs e)
        {
            // 모델 파일 선택
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "분석에 사용할 동키카 모델(.h5) 파일을 선택하세요.";
                ofd.Filter = "Keras Model (*.h5)|*.h5|All Files (*.*)|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                _selectedModelPath = ofd.FileName;
            }

            // Tub(카탈로그)가 미리 로드되어 있는지 확인
            if (string.IsNullOrWhiteSpace(_currentCatalogPath) || _masterFrameList == null || _masterFrameList.Count == 0)
            {
                MessageBox.Show("먼저 Tub 폴더를 'Load Tub' 버튼으로 로드하세요.", "준비 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            try
            {
                rtbTrainLog.AppendText($"선택 모델: {_selectedModelPath}\r\nTub: {_currentCatalogPath}\r\nAI 예측 시작...\r\n");

                // PredictAllFrames는 WSL/Python 호출로 블로킹이므로 백그라운드에서 실행
                var predicted = await Task.Run(() => PredictAllFrames(_currentCatalogPath, _selectedModelPath));
                _aiPredictedList = predicted ?? new List<AiPredictFrame>();
                _aiPredictedMap = new Dictionary<int, AiPredictFrame>();

                if (_aiPredictedList.Count > 0)
                {
                    // 1) 예측에 FrameIndex가 포함된 경우: 그걸로 매핑
                    if (_aiPredictedList.All(p => p.FrameIndex != -1))
                    {
                        foreach (var p in _aiPredictedList) _aiPredictedMap[p.FrameIndex] = p;
                    }
                    else
                    {
                        // 2) 포함 안 되어 있으면 master 순서와 매칭(최소 길이)
                        int m = Math.Min(_aiPredictedList.Count, _masterFrameList.Count);
                        for (int i = 0; i < m; i++)
                        {
                            int fid = _masterFrameList[i].FrameIndex;
                            _aiPredictedList[i].FrameIndex = fid;
                            _aiPredictedMap[fid] = _aiPredictedList[i];
                        }
                    }
                }

                rtbTrainLog.AppendText($"AI 예측 완료: {_aiPredictedList.Count}개 항목\r\n");

                if (_aiPredictedList.Count == 0)
                {
                    MessageBox.Show("예측 결과가 없습니다. rtbTrainLog의 stdout/stderr를 확인하세요.", "예측 실패", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 예측 개수가 카탈로그 개수와 다른 경우 경고 로그
                if (_aiPredictedList.Count != _masterFrameList.Count)
                {
                    rtbTrainLog.AppendText($"경고: 카탈로그({_masterFrameList.Count})와 예측({_aiPredictedList.Count}) 항목 수가 다릅니다. (최소 길이로 매칭하여 시각화)\r\n");
                }

                // UI 갱신 — 카탈로그는 이미 로드되어 있으므로 리스트 새로 고침
                RefreshFrameList(_masterFrameList);
                pbMainCam.Invalidate();

                MessageBox.Show($"AI 예측 연산 완료: {_aiPredictedList.Count}개", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예측 중 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtbTrainLog.AppendText($"예측 예외: {ex}\r\n");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
