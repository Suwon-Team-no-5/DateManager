using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DateManager.services
{
    internal sealed class CatalogFrameController
    {
        private readonly Button loadCatalogButton;
        private readonly ComboBox filterComboBox;
        private readonly ListBox frameListBox;
        private readonly PictureBox framePictureBox;
        private readonly Label statusLabel;
        private readonly Label infoLabel;
        private readonly List<FrameData> allFrames = [];

        private List<FrameData> visibleFrames = [];
        private string catalogDirectory = string.Empty;

        public static CatalogFrameController? TryAttach(Control root)
        {
            
            var loadCatalogButton = FindControl<Button>(root, "loadCatalogButton", "btnLoadCatalog", "openCatalogButton");
            var filterComboBox = FindControl<ComboBox>(root, "filterComboBox", "cmbFilter", "frameFilterComboBox");
            var frameListBox = FindControl<ListBox>(root, "frameListBox", "lstFrames", "framesListBox");
            var framePictureBox = FindControl<PictureBox>(root, "framePictureBox", "pictureBoxFrame", "previewPictureBox");
            var statusLabel = FindControl<Label>(root, "statusLabel", "lblStatus", "catalogStatusLabel");
            var infoLabel = FindControl<Label>(root, "infoLabel", "lblFrameInfo", "frameInfoLabel");

            if (loadCatalogButton is null ||
                filterComboBox is null ||
                frameListBox is null ||
                framePictureBox is null ||
                statusLabel is null ||
                infoLabel is null)
            {
                return null;
            }

            return new CatalogFrameController(
                loadCatalogButton,
                filterComboBox,
                frameListBox,
                framePictureBox,
                statusLabel,
                infoLabel);
        }

        public CatalogFrameController(
            Button loadCatalogButton,
            ComboBox filterComboBox,
            ListBox frameListBox,
            PictureBox framePictureBox,
            Label statusLabel,
            Label infoLabel)
        {
            this.loadCatalogButton = loadCatalogButton;
            this.filterComboBox = filterComboBox;
            this.frameListBox = frameListBox;
            this.framePictureBox = framePictureBox;
            this.statusLabel = statusLabel;
            this.infoLabel = infoLabel;

            this.loadCatalogButton.Click += LoadCatalogButton_Click;
            this.filterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            this.frameListBox.SelectedIndexChanged += FrameListBox_SelectedIndexChanged;

            if (this.filterComboBox.Items.Count == 0)
            {
                this.filterComboBox.Items.AddRange(new object[]
                {
                    "All frames",
                    "Stopped (Throttle = 0)",
                    "Straight driving (Angle ~= 0, Throttle > 0)"
                });
            }

            this.filterComboBox.SelectedIndex = 0;
        }

        private static T? FindControl<T>(Control root, params string[] names)
            where T : Control
        {
            foreach (var name in names)
            {
                var matches = root.Controls.Find(name, true);

                if (matches.Length > 0 && matches[0] is T control)
                {
                    return control;
                }
            }

            return null;
        }

        private void LoadCatalogButton_Click(object? sender, EventArgs e)
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
                ApplySelectedFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Catalog load failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplySelectedFilter();
        }

        private void FrameListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (frameListBox.SelectedItem is FrameData frame)
            {
                ShowFrame(frame);
            }
        }

        private void ApplySelectedFilter()
        {
            visibleFrames = filterComboBox.SelectedIndex switch
            {
                1 => FrameFilterService.GetStoppedFrames(allFrames),
                2 => FrameFilterService.GetStraightDrivingFrames(allFrames),
                _ => new List<FrameData>(allFrames)
            };

            frameListBox.DataSource = null;
            frameListBox.DataSource = visibleFrames;
            statusLabel.Text = $"Showing: {visibleFrames.Count} / Total: {allFrames.Count}";

            if (visibleFrames.Count > 0)
            {
                frameListBox.SelectedIndex = 0;
                return;
            }

            framePictureBox.Image = null;
            infoLabel.Text = "No frames match the selected filter.";
        }

        private void ShowFrame(FrameData frame)
        {
            var imagePath = Path.Combine(catalogDirectory, frame.ImageFile);

            framePictureBox.Image?.Dispose();
            framePictureBox.Image = File.Exists(imagePath) ? Image.FromFile(imagePath) : null;

            infoLabel.Text = File.Exists(imagePath)
                ? $"{frame.ImageFile} | angle: {frame.Angle:0.000} | throttle: {frame.Throttle:0.000}"
                : $"Image file not found: {imagePath}";
        }
    }
}
