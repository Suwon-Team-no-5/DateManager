using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DateManager
{
    public class DonkeyFrame
    {
        [JsonProperty("_index")]
        public int Index { get; set; }

        [JsonProperty("_session_id")]
        public string SessionId { get; set; }

        [JsonProperty("cam/image_array")]
        public string ImagePath { get; set; }

        [JsonProperty("user/angle")]
        public double Angle { get; set; }

        [JsonProperty("user/throttle")]
        public double Throttle { get; set; }

        public int FrameIndex { get; set; }
        public string FullImagePath { get; set; }
        public bool IsNewData { get; set; }
        public string DataTypeSummary { get; set; }
    }

    public class DataProcessor
    {
        public List<DonkeyFrame> LoadCatalogData(string catalogFilePath, string imagesFolderPath)
        {
            List<DonkeyFrame> frameList = new List<DonkeyFrame>();

            try
            {
                if (!File.Exists(catalogFilePath))
                {
                    MessageBox.Show("지정한 catalog 파일을 찾을 수 없습니다.", "파일 에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return frameList;
                }

                string[] lines = File.ReadAllLines(catalogFilePath);
                int index = 0;
                int oldDataCount = 0;
                int newDataCount = 0;

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    DonkeyFrame frame = JsonConvert.DeserializeObject<DonkeyFrame>(line);

                    if (frame != null)
                    {
                        frame.FrameIndex = index++;

                        if (frame.SessionId == "26-05-21_1" || string.IsNullOrEmpty(frame.ImagePath))
                        {
                            frame.IsNewData = true;
                            frame.DataTypeSummary = "신규 수집(이미지 누락)";
                            frame.FullImagePath = "";
                            newDataCount++;
                        }
                        else
                        {
                            frame.IsNewData = false;
                            frame.DataTypeSummary = "기존 데이터(정상)";
                            frame.FullImagePath = Path.Combine(imagesFolderPath, frame.ImagePath);
                            oldDataCount++;
                        }

                        frameList.Add(frame);
                    }
                }

                if (frameList.Count > 0)
                {
                    string reportMessage = $"[로드 완료] 총 {frameList.Count}개의 프레임을 읽었습니다.\n\n" +
                                           $"🔹 기존 데이터(25-12-15): {oldDataCount}개\n" +
                                           $"🔸 신규 데이터(26-05-21): {newDataCount}개 (정제 필요)";

                    MessageBox.Show(reportMessage, "데이터 구분 성공");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터를 읽는 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return frameList;
        }

        public void DeleteFrame(List<DonkeyFrame> frameList, DonkeyFrame targetFrame)
        {
            if (targetFrame == null) return;

            try
            {
                if (!string.IsNullOrEmpty(targetFrame.FullImagePath) && File.Exists(targetFrame.FullImagePath))
                {
                    File.Delete(targetFrame.FullImagePath);
                }

                frameList.Remove(targetFrame);
                MessageBox.Show($"프레임 {targetFrame.FrameIndex}번 ({targetFrame.DataTypeSummary}) 데이터가 성공적으로 정제(삭제)되었습니다.", "정제 완료");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 삭제 실패: {ex.Message}", "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
