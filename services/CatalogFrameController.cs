using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DateManager.services
{
    internal sealed class CatalogFrameController
    {
        // 1. UI 컨트롤 필드 선언 (정리해주신 명명 규칙 반영)
        private readonly Button loadTubButton;
        private readonly Button? applyFilterButton;
        private readonly CheckBox? throttleFilterCheckBox;
        private readonly CheckBox? angleZeroFilterCheckBox;
        private readonly CheckBox? largeAngleFilterCheckBox;
        private readonly ListBox frameListBox;
        private readonly PictureBox framePictureBox;
        private readonly TrackBar? frameSlider;
        private readonly Label? frameIndexLabel;
        private readonly Label? angleLabel;
        private readonly Label? throttleTopLabel;
        private readonly ProgressBar? prgThrottle; // [변경] lblThrottleBottom(Label) ➔ prgThrottle(ProgressBar)
        private readonly Label? timestampLabel;

        // [추가] 독립 배치된 재생/정지 버튼 제어용 필드
        private readonly Button? btnPlay;
        private readonly Button? btnStop;

        private readonly List<FrameData> allFrames = [];
        private List<FrameData> visibleFrames = [];
        private string catalogDirectory = string.Empty;

        // 2. Form1에서 컨트롤들을 이름으로 자동 연결하는 메서드
        public static CatalogFrameController? TryAttach(Control root)
        {
            // 필수 컨트롤 체크 (이름 일치 확인용)
            var loadTubButton = FindControl<Button>(root, "btnLoadTub");
            var frameListBox = FindControl<ListBox>(root, "lstFrameData");
            var framePictureBox = FindControl<PictureBox>(root, "pbMainCam");

            if (loadTubButton is null || frameListBox is null || framePictureBox is null)
            {
                return null;
            }

            // 정의해주신 새로운 변수명 규칙에 맞춰 1:1 매핑하여 생성자 호출
            return new CatalogFrameController(
                loadTubButton,
                FindControl<Button>(root, "btnApplyFilter"),
                FindControl<CheckBox>(root, "chkFilterThr"),
                FindControl<CheckBox>(root, "chkFilterAngleZero"),
                FindControl<CheckBox>(root, "chkFilterLargeAngle"),
                frameListBox,
                framePictureBox,
                FindControl<TrackBar>(root, "trkFrameSlider"),
                FindControl<Label>(root, "lblFrameIndex"),
                FindControl<Label>(root, "lblAngle"),
                FindControl<Label>(root, "lblThrottleTop"),
                FindControl<ProgressBar>(root, "prgThrottle"), // [변경] 하단 스레틀 게이지 바 연동
                FindControl<Label>(root, "lblTimestamp"),
                FindControl<Button>(root, "btnPlay"),           // [추가] 재생 버튼 매핑
                FindControl<Button>(root, "btnStop"));          // [추가] 정지 버튼 매핑
        }

        // 3. 생성자 및 이벤트 바인딩
        private CatalogFrameController(
            Button loadTubButton,
            Button? applyFilterButton,
            CheckBox? throttleFilterCheckBox,
            CheckBox? angleZeroFilterCheckBox,
            CheckBox? largeAngleFilterCheckBox,
            ListBox frameListBox,
            PictureBox framePictureBox,
            TrackBar? frameSlider,
            Label? frameIndexLabel,
            Label? angleLabel,
            Label? throttleTopLabel,
            ProgressBar? prgThrottle,
            Label? timestampLabel,
            Button? btnPlay,
            Button? btnStop)
        {
            this.loadTubButton = loadTubButton;
            this.applyFilterButton = applyFilterButton;
            this.throttleFilterCheckBox = throttleFilterCheckBox;
            this.angleZeroFilterCheckBox = angleZeroFilterCheckBox;
            this.largeAngleFilterCheckBox = largeAngleFilterCheckBox;
            this.frameListBox = frameListBox;
            this.framePictureBox = framePictureBox;
            this.frameSlider = frameSlider;
            this.frameIndexLabel = frameIndexLabel;
            this.angleLabel = angleLabel;
            this.throttleTopLabel = throttleTopLabel;
            this.prgThrottle = prgThrottle;
            this.timestampLabel = timestampLabel;
            this.btnPlay = btnPlay;
            this.btnStop = btnStop;

            // 필수 이벤트 연결
            this.loadTubButton.Click += LoadTubButton_Click;
            this.frameListBox.SelectedIndexChanged += FrameListBox_SelectedIndexChanged;

            if (this.applyFilterButton is not null)
            {
                this.applyFilterButton.Click += ApplyFilterButton_Click;
            }

            if (this.frameSlider is not null)
            {
                this.frameSlider.Scroll += FrameSlider_Scroll;
            }

            // [추가] 재생/정지 버튼 클릭 시 UX 비활성화/활성화 상태 제어 트리거 연결
            if (this.btnPlay is not null)
            {
                this.btnPlay.Click += BtnPlay_Click;
                this.btnPlay.Enabled = true; // 초기 상태: 재생 가능
            }
            if (this.btnStop is not null)
            {
                this.btnStop.Click += BtnStop_Click;
                this.btnStop.Enabled = false; // 초기 상태: 정지 비활성화
            }
        }

        private static T? FindControl<T>(Control root, string name) where T : Control
        {
            var matches = root.Controls.Find(name, true);
            return matches.Length > 0 && matches[0] is T control ? control : null;
        }

        // [추가] 미디어 플레이어 전형의 UI 흐름 적용 (재생 클릭 시)
        private void BtnPlay_Click(object? sender, EventArgs e)
        {
            if (btnPlay is not null) btnPlay.Enabled = false;
            if (btnStop is not null) btnStop.Enabled = true;
        }

        // [추가] 미디어 플레이어 전형의 UI 흐름 적용 (정지 클릭 시)
        private void BtnStop_Click(object? sender, EventArgs e)
        {
            if (btnPlay is not null) btnPlay.Enabled = true;
            if (btnStop is not null) btnStop.Enabled = false;
        }

        private void LoadTubButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "카탈로그 파일 선택",
                Filter = "Catalog or JSON Lines (*.catalog;*.jsonl;*.txt)|*.catalog;*.jsonl;*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                allFrames.Clear();
                allFrames.AddRange(CatalogServices.LoadCatalog(dialog.FileName));
                catalogDirectory = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                ApplySelectedFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catalog load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilterButton_Click(object? sender, EventArgs e)
        {
            ApplySelectedFilters();
        }

        private void FrameListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (frameListBox.SelectedItem is FrameData frame)
            {
                ShowFrame(frame);
            }
        }

        private void FrameSlider_Scroll(object? sender, EventArgs e)
        {
            if (frameSlider is null || frameSlider.Value < 0 || frameSlider.Value >= visibleFrames.Count)
            {
                return;
            }

            frameListBox.SelectedIndex = frameSlider.Value;
        }

        private void ApplySelectedFilters()
        {
            IEnumerable<FrameData> filteredFrames = allFrames;

            if (throttleFilterCheckBox?.Checked == true)
            {
                filteredFrames = filteredFrames.Where(frame => frame.Throttle > 0);
            }

            if (angleZeroFilterCheckBox?.Checked == true)
            {
                filteredFrames = filteredFrames.Where(frame => Math.Abs(frame.Angle) <= 0.01);
            }

            if (largeAngleFilterCheckBox?.Checked == true)
            {
                filteredFrames = filteredFrames.Where(frame => Math.Abs(frame.Angle) > 0.3);
            }

            visibleFrames = filteredFrames.ToList();
            BindFrameList();
        }

        private void BindFrameList()
        {
            frameListBox.DataSource = null;
            frameListBox.DataSource = visibleFrames;

            if (frameSlider is not null)
            {
                frameSlider.Minimum = 0;
                frameSlider.Maximum = Math.Max(0, visibleFrames.Count - 1);
                frameSlider.Value = 0;
            }

            if (visibleFrames.Count > 0)
            {
                frameListBox.SelectedIndex = 0;
                return;
            }

            framePictureBox.Image = null;
            UpdateFrameLabels(null);
        }

        private void ShowFrame(FrameData frame)
        {
            var imagePath = Path.Combine(catalogDirectory, frame.ImageFile);

            framePictureBox.Image?.Dispose();
            framePictureBox.Image = File.Exists(imagePath) ? Image.FromFile(imagePath) : null;

            if (frameSlider is not null && frameListBox.SelectedIndex >= 0)
            {
                frameSlider.Value = frameListBox.SelectedIndex;
            }

            UpdateFrameLabels(frame);
        }

        // 4. 데이터 표시 갱신 (라벨 영문화 및 프로그레스 바 연동 핵심부)
        // 4. 데이터 표시 갱신 (항상 한글 표기 및 프로그레스 바 연동)
        private void UpdateFrameLabels(FrameData? frame)
        {
            // [수정] 맨 처음 또는 데이터가 없을 때 기본 한글 표시
            if (frame is null)
            {
                SetText(frameIndexLabel, "프레임 인덱스 0/0");
                SetText(angleLabel, "조향각(앵글): +0.0");
                SetText(throttleTopLabel, "출력(스레틀): +0.0");
                if (prgThrottle is not null) prgThrottle.Value = 0; // 하단 게이지 초기화
                SetText(timestampLabel, "타임스탬프");
                return;
            }

            // [수정] 리스트에서 프레임을 선택했을 때도 한글로 데이터 업데이트
            SetText(frameIndexLabel, $"프레임 인덱스 {frame.Index}/{Math.Max(0, allFrames.Count - 1)}");
            SetText(angleLabel, $"조향각(앵글): {frame.Angle:+0.000;-0.000;0.000}");
            SetText(throttleTopLabel, $"출력(스레틀): {frame.Throttle:+0.000;-0.000;0.000}");

            // 하단 스레틀 게이지 바 연동 (0.0~1.0 사이의 값을 0~100%로 변환)
            if (prgThrottle is not null)
            {
                int progressValue = (int)(frame.Throttle * 100);

                // ProgressBar 범위를 벗어나 에러가 발생하는 것을 방지 (0~100 사이로 보정)
                prgThrottle.Value = Math.Max(0, Math.Min(100, progressValue));
            }

            SetText(timestampLabel, frame.TimestampMs.ToString());
        }

        private static void SetText(Label? label, string text)
        {
            if (label is not null)
            {
                label.Text = text;
            }
        }
    }
}