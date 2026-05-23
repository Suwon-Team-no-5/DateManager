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
        // 💡 [추가] 기존 호출 방식(인수 2개)과 호환을 위한 오버로드 메서드
        public List<DonkeyFrame> LoadCatalogData(string catalogFilePath, string imagesFolderPath)
        {
            // catalogFilePath의 상위 폴더 경로를 추출하여 폴더 기반 로드 메서드 호출
            string folderPath = Path.GetDirectoryName(catalogFilePath);
            return LoadCatalogData(folderPath);
        }

        // 💡 [수정] 폴더 내 모든 파일을 한 번에 로드하는 메인 메서드
        public List<DonkeyFrame> LoadCatalogData(string folderPath)
        {
            List<DonkeyFrame> allFrames = new List<DonkeyFrame>();
            string imagesFolderPath = Path.Combine(folderPath, "images");

            // 폴더 내 모든 .catalog 파일 검색
            string[] catalogFiles = Directory.GetFiles(folderPath, "*.catalog");

            int globalIndex = 0;
            foreach (string file in catalogFiles)
            {
                List<DonkeyFrame> framesFromFile = ParseSingleCatalog(file, imagesFolderPath);
                foreach (var frame in framesFromFile)
                {
                    frame.FrameIndex = globalIndex++;
                    allFrames.Add(frame);
                }
            }
            return allFrames;
        }

        // 개별 파일 파싱 전용 로직
        private List<DonkeyFrame> ParseSingleCatalog(string catalogFilePath, string imagesFolderPath)
        {
            List<DonkeyFrame> frameList = new List<DonkeyFrame>();

            if (!File.Exists(catalogFilePath)) return frameList;

            string[] lines = File.ReadAllLines(catalogFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                DonkeyFrame frame = JsonConvert.DeserializeObject<DonkeyFrame>(line);
                if (frame != null)
                {
                    // 로직: 신규/기존 데이터 판단
                    if (frame.SessionId == "26-05-21_1" || string.IsNullOrEmpty(frame.ImagePath))
                    {
                        frame.IsNewData = true;
                        frame.DataTypeSummary = "신규 수집(이미지 누락)";
                        frame.FullImagePath = "";
                    }
                    else
                    {
                        frame.IsNewData = false;
                        frame.DataTypeSummary = "기존 데이터(정상)";
                        frame.FullImagePath = Path.Combine(imagesFolderPath, frame.ImagePath);
                    }
                    frameList.Add(frame);
                }
            }
            return frameList;
        }
    }
}