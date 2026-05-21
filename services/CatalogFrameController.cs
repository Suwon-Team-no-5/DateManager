using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

//UI컨트롤들과 catalog 처리기능 연결하는 클래스
namespace DateManager.services
{
    internal sealed class CatalogFrameController
    {
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
        private readonly Label? throttleBottomLabel;
        private readonly Label? timestampLabel;
        private readonly List<FrameData> allFrames = [];

        private List<FrameData> visibleFrames = [];
        private string catalogDirectory = string.Empty;

        public static CatalogFrameController? TryAttach(Control root) //Form1 안에서 필요한 컨트롤들을 이름으로 찾아서 연결
        {
            var loadTubButton = FindControl<Button>(root, "btnLoadTub"); //카탈로그 파일 불러오는 버튼
            var frameListBox = FindControl<ListBox>(root, "lstFrameData"); //전체 프레임 목록 보여주는 리스트박스
            var framePictureBox = FindControl<PictureBox>(root, "pbMainCam"); //선택한 프레임의 이미지 보여주는 픽쳐박스

            if (loadTubButton is null || frameListBox is null || framePictureBox is null)
            {
                return null;
            }

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
                FindControl<Label>(root, "lblThrottleBottom"),
                FindControl<Label>(root, "lblTimestamp"));
        }

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
            Label? throttleBottomLabel,
            Label? timestampLabel)
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
            this.throttleBottomLabel = throttleBottomLabel;
            this.timestampLabel = timestampLabel;

            this.loadTubButton.Click += LoadTubButton_Click;
            this.frameListBox.SelectedIndexChanged += FrameListBox_SelectedIndexChanged;
            //찾은 컨트롤에 이벤트 연결

            if (this.applyFilterButton is not null)
            {
                this.applyFilterButton.Click += ApplyFilterButton_Click;
            }

            if (this.frameSlider is not null)
            {
                this.frameSlider.Scroll += FrameSlider_Scroll;
            }
        }

        private static T? FindControl<T>(Control root, string name)
            where T : Control
        {
            var matches = root.Controls.Find(name, true);
            return matches.Length > 0 && matches[0] is T control ? control : null;
        }

        private void LoadTubButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "카탈로그 파일 선택",
                Filter = "Catalog or JSON Lines (*.catalog;*.jsonl;*.txt)|*.catalog;*.jsonl;*.txt|All files (*.*)|*.*"
                //카탈로그 파일 선택하는 다이얼로그, .catalog, .jsonl, .txt 확장자 허용
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                allFrames.Clear();
                allFrames.AddRange(CatalogServices.LoadCatalog(dialog.FileName)); //CatalogServices의 LoadCatalog 메서드로 선택한 파일에서 프레임 데이터 로드
                catalogDirectory = Path.GetDirectoryName(dialog.FileName) ?? string.Empty; //카탈로그 파일이 있는 디렉토리 경로 저장
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
            //리스트박스에서 프레임 선택이 바뀌면 해당 프레임 보여주기
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
            } //throttle이 0보다 큰 프레임만 남김

            if (angleZeroFilterCheckBox?.Checked == true)
            {
                filteredFrames = filteredFrames.Where(frame => Math.Abs(frame.Angle) <= 0.01);
            } //angle이 거의 0인 프레임만 남김

            if (largeAngleFilterCheckBox?.Checked == true)
            {
                filteredFrames = filteredFrames.Where(frame => Math.Abs(frame.Angle) > 0.3);
            } //angle이 0.3보다 큰 프레임만 남김

            visibleFrames = filteredFrames.ToList();
            BindFrameList();
            //필터 적용된 프레임 목록 업데이트
        }

        private void BindFrameList()
        {
            frameListBox.DataSource = null;
            frameListBox.DataSource = visibleFrames; //리스트박스에 필터 적용된 프레임 목록 바인딩

            if (frameSlider is not null) //프레임 슬라이더가 있으면 슬라이더 범위 업데이트
            {
                frameSlider.Minimum = 0;
                frameSlider.Maximum = Math.Max(0, visibleFrames.Count - 1);
                frameSlider.Value = 0;
            }

            if (visibleFrames.Count > 0)
            {
                frameListBox.SelectedIndex = 0;
                return;
            } //프레임이 있으면 자동으로 첫 프레임 선택

            framePictureBox.Image = null;
            UpdateFrameLabels(null);
        }

        private void ShowFrame(FrameData frame)
        {
            var imagePath = Path.Combine(catalogDirectory, frame.ImageFile);
            //카탈로그 디렉토리와 프레임의 이미지 파일 경로 합쳐서 전체 이미지 경로 생성

            framePictureBox.Image?.Dispose();
            framePictureBox.Image = File.Exists(imagePath) ? Image.FromFile(imagePath) : null;
            //이미지 파일이 존재하면 로드해서 픽쳐박스에 보여주고, 없으면 null로 설정

            if (frameSlider is not null && frameListBox.SelectedIndex >= 0)
            {
                frameSlider.Value = frameListBox.SelectedIndex;
            }//프레임 슬라이더가 있으면 현재 선택된 프레임 인덱스로 슬라이더 위치 업데이트

            UpdateFrameLabels(frame);
        }

        private void UpdateFrameLabels(FrameData? frame)
        {
            //프레임 정보 레이블 업데이트, 프레임이 null이면 기본값으로 설정
            if (frame is null)
            {
                SetText(frameIndexLabel, "Frame Index 0/0");
                SetText(angleLabel, "Angle: +0.0");
                SetText(throttleTopLabel, "Throttle: +0.0");
                SetText(throttleBottomLabel, "Throttle: +0.0");
                SetText(timestampLabel, "Timestamp");
                return;
            }

            SetText(frameIndexLabel, $"Frame Index {frame.Index}/{Math.Max(0, allFrames.Count - 1)}");
            SetText(angleLabel, $"Angle: {frame.Angle:+0.000;-0.000;0.000}");
            SetText(throttleTopLabel, $"Throttle: {frame.Throttle:+0.000;-0.000;0.000}");
            SetText(throttleBottomLabel, $"Throttle: {frame.Throttle:+0.000;-0.000;0.000}");
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
