using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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
        // 간단한 설명 주석 추가: 이 클래스는 카탈로그(.catalog) 파일을 읽어 DonkeyFrame 리스트로 반환합니다.
        // 성능 개선 포인트: 스트리밍, 병렬 처리, 에러 격리
        // 💡 [추가] 기존 호출 방식(인수 2개)과 호환을 위한 오버로드 메서드
        public List<DonkeyFrame> LoadCatalogData(string catalogFilePath, string imagesFolderPath)
        {
            // catalogFilePath의 상위 폴더 경로를 추출하여 폴더 기반 로드 메서드 호출
            string folderPath = Path.GetDirectoryName(catalogFilePath);
            return LoadCatalogData(folderPath);
        }

        // 💡 [수정] 폴더 내 모든 파일을 한 번에 로드하는 메인 메서드
        // 개선점: File.ReadLines로 스트리밍 처리하여 초기 지연 및 메모리 사용을 줄이고,
        // 파일 단위로 병렬 처리를 적용하여 전체 처리 속도를 향상시킵니다.
        public List<DonkeyFrame> LoadCatalogData(string folderPath)
        {
            string imagesFolderPath = Path.Combine(folderPath, "images");
            string[] catalogFiles = Directory.GetFiles(folderPath, "*.catalog");

            var allFramesBag = new ConcurrentBag<DonkeyFrame>();

            var po = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) };

            Parallel.ForEach(catalogFiles, po, file =>
            {
                try
                {
                    foreach (string line in File.ReadLines(file))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            DonkeyFrame frame = JsonConvert.DeserializeObject<DonkeyFrame>(line);
                            if (frame == null) continue;

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

                            allFramesBag.Add(frame);
                        }
                        catch
                        {
                            // 개별 라인이 잘못되어도 전체 로드에 영향 주지 않도록 무시
                            continue;
                        }
                    }
                }
                catch
                {
                    // 파일 단위 읽기 실패 시에도 다른 파일 처리는 계속함
                }
            });

            // 병렬로 수집한 결과를 리스트로 변환한 뒤, 원본 JSON의 _index 필드가 있으면 그에 따라 정렬
            List<DonkeyFrame> allFrames = allFramesBag.ToList();
            if (allFrames.Count == 0) return allFrames;

            // 정렬: 가능하면 JSON의 _index(Index)로 정렬하여 결정적 순서를 보장
            allFrames = allFrames.OrderBy(f => f.Index).ToList();

            for (int i = 0; i < allFrames.Count; i++)
            {
                allFrames[i].FrameIndex = i;
            }

            return allFrames;
        }

        // Overload: 진행률 보고를 지원하는 버전
        public List<DonkeyFrame> LoadCatalogData(string folderPath, IProgress<int> progress)
        {
            string imagesFolderPath = Path.Combine(folderPath, "images");
            string[] catalogFiles = Directory.GetFiles(folderPath, "*.catalog");

            var allFramesBag = new ConcurrentBag<DonkeyFrame>();
            var po = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) };

            int totalFiles = catalogFiles.Length;
            int processedFiles = 0;

            Parallel.ForEach(catalogFiles, po, file =>
            {
                try
                {
                    foreach (string line in File.ReadLines(file))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            DonkeyFrame frame = JsonConvert.DeserializeObject<DonkeyFrame>(line);
                            if (frame == null) continue;

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

                            allFramesBag.Add(frame);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (totalFiles > 0)
                    {
                        int processed = System.Threading.Interlocked.Increment(ref processedFiles);
                        int percent = (int)Math.Round(processed * 100.0 / totalFiles);
                        try { progress?.Report(percent); } catch { }
                    }
                }
            });

            List<DonkeyFrame> allFrames = allFramesBag.ToList();
            if (allFrames.Count == 0)
            {
                try { progress?.Report(100); } catch { }
                return allFrames;
            }

            allFrames = allFrames.OrderBy(f => f.Index).ToList();

            for (int i = 0; i < allFrames.Count; i++)
            {
                allFrames[i].FrameIndex = i;
            }

            try { progress?.Report(100); } catch { }
            return allFrames;
        }

        // 이전에는 파일 전체를 한꺼번에 읽어들이는 방식이었으나
        // 이제 File.ReadLines 기반 스트리밍으로 처리하므로 이 헬퍼는 더이상 사용되지 않습니다.
        private List<DonkeyFrame> ParseSingleCatalog(string catalogFilePath, string imagesFolderPath)
        {
            // 호환성 유지를 위해 간단한 구현을 남겨두지만, 사용은 권장하지 않습니다.
            var result = new List<DonkeyFrame>();
            if (!File.Exists(catalogFilePath)) return result;

            foreach (var line in File.ReadLines(catalogFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var frame = JsonConvert.DeserializeObject<DonkeyFrame>(line);
                    if (frame == null) continue;
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
                    result.Add(frame);
                }
                catch
                {
                    continue;
                }
            }

            return result;
        }
    }
}