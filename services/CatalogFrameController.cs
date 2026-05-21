using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        public static CatalogFrameController? TryAttach(Control root)
        {
            var loadTubButton = FindControl<Button>(root, "btnLoadTub");
            var frameListBox = FindControl<ListBox>(root, "lstFrameData");
            var framePictureBox = FindControl<PictureBox>(root, "pbMainCam");

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
                Title = "Select catalog file",
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

        private void UpdateFrameLabels(FrameData? frame)
        {
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
