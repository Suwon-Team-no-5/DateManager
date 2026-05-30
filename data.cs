using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace DateManager
{
    public class DonkeyFrame
    {
        [JsonPropertyName("_index")]
        public int Index { get; set; }

        [JsonPropertyName("_session_id")]
        public string SessionId { get; set; } = "";

        [JsonPropertyName("cam/image_array")]
        public string ImagePath { get; set; } = "";

        [JsonPropertyName("user/angle")]
        public double Angle { get; set; }

        [JsonPropertyName("user/throttle")]
        public double Throttle { get; set; }

        public int FrameIndex { get; set; }
        public string FullImagePath { get; set; } = "";
        public bool IsNewData { get; set; }
        public string DataTypeSummary { get; set; } = "";
        public string SourceCatalogPath { get; set; } = "";
    }

    public class DataProcessor
    {
        // 데이터 처리 클래스: 카탈로그(.catalog) 파일을 읽어 DonkeyFrame 리스트로 반환합니다.
        // 💡 [추가] 기존 호출 방식(인수 2개)과 호환을 위한 오버로드 메서드
        public List<DonkeyFrame> LoadCatalogData(string catalogFilePath, string imagesFolderPath)
        {
            // catalogFilePath의 상위 폴더 경로를 추출하여 폴더 기반 로드 메서드 호출
            string folderPath = Path.GetDirectoryName(catalogFilePath) ?? "";
            return LoadCatalogData(folderPath);
        }

        // 💡 [수정] 폴더 내 모든 파일을 한 번에 로드하는 메인 메서드
        // 개선점: File.ReadLines로 스트리밍 처리하여 초기 지연 및 메모리 사용을 줄이고,
        // 파일 단위로 병렬 처리를 적용하여 전체 처리 속도를 향상시킵니다.
        public List<DonkeyFrame> LoadCatalogData(string folderPath)
        {
            string imagesFolderPath = Path.Combine(folderPath, "images");
            string[] catalogFiles = Directory.GetFiles(folderPath, "*.catalog");

            // 간단한 디스크 캐시 도입: 카탈로그 파일들의 경로와 최종 수정시간을 기반으로 해시를 만들어
            // 해당 해시에 대응하는 캐시 파일이 있으면 파싱을 건너뛰고 캐시를 로드합니다.
            const string cacheVersion = "catalog-source-v1";
            string cacheHashSource = cacheVersion + "|" + string.Join("|", catalogFiles.Select(p => p + ":" + File.GetLastWriteTimeUtc(p).Ticks));
            string cacheHash;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] src = System.Text.Encoding.UTF8.GetBytes(cacheHashSource);
                byte[] dig = sha.ComputeHash(src);
                cacheHash = BitConverter.ToString(dig).Replace("-", "").ToLowerInvariant();
            }

            string cacheFilePath = Path.Combine(folderPath, $".catalogcache_{cacheHash}.bin");

            if (File.Exists(cacheFilePath))
            {
                try
                {
                    byte[] data = File.ReadAllBytes(cacheFilePath);
                    var cached = JsonSerializer.Deserialize<List<DonkeyFrame>>(data);
                    if (cached != null && cached.Count > 0)
                    {
                        // 캐시 로드 시에는 FullImagePath는 imagesFolderPath를 기준으로 재연결
                        foreach (var f in cached)
                        {
                            if (!string.IsNullOrEmpty(f.ImagePath))
                            {
                                f.FullImagePath = Path.Combine(imagesFolderPath, f.ImagePath);
                            }
                        }
                        return cached;
                    }
                }
                catch
                {
                    // 캐시 로드 실패 시 무시하고 새로 파싱
                }
            }

            var allFramesBag = new ConcurrentBag<DonkeyFrame>();

            var po = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            Parallel.ForEach(catalogFiles, po, file =>
            {
                try
                {
                    // Use StreamReader to control buffering and use ReadLine for streaming
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan))
                    using (var sr = new StreamReader(fs))
                    {
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            try
                            {
                                var frame = JsonSerializer.Deserialize<DonkeyFrame>(line, jsonOptions);
                                if (frame == null) continue;
                                frame.SourceCatalogPath = file;

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
                                    // 기본 경로 조합
                                    frame.FullImagePath = ResolveImagePath(folderPath, frame.ImagePath);
                                }
                        

                                allFramesBag.Add(frame);
                            }
                            catch
                            {
                                // ignore bad line
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                    // file read error ignore
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

            // 파싱 결과를 캐시 파일로 저장 (비동기적으로 저장하여 로드에 영향 최소화)
            try
            {
                var toWrite = JsonSerializer.SerializeToUtf8Bytes(allFrames);
                Task.Run(() =>
                {
                    try { File.WriteAllBytes(cacheFilePath, toWrite); } catch { }
                });
            }
            catch { }

            return allFrames;
        }

        private string ResolveImagePath(string tubFolderPath, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return "";

            string imagesFolderPath = Path.Combine(tubFolderPath, "images");

            string candidate;

            if (Path.IsPathRooted(imagePath))
            {
                candidate = imagePath;
                if (File.Exists(candidate)) return candidate;
            }

            candidate = Path.Combine(tubFolderPath, imagePath);
            if (File.Exists(candidate)) return candidate;

            candidate = Path.Combine(imagesFolderPath, imagePath);
            if (File.Exists(candidate)) return candidate;

            string fileName = Path.GetFileName(imagePath);
            if (string.IsNullOrWhiteSpace(fileName) || !Directory.Exists(imagesFolderPath))
                return "";

            try
            {
                return Directory.EnumerateFiles(imagesFolderPath, fileName, SearchOption.AllDirectories)
                    .FirstOrDefault() ?? "";
            }
            catch
            {
                return "";
            }
        }

        // 이전에는 파일 전체를 한꺼번에 읽어들이는 방식이었으나
        // 이제 File.ReadLines 기반 스트리밍으로 처리하므로 이 헬퍼는 더이상 사용되지 않습니다.
        private List<DonkeyFrame> ParseSingleCatalog(string catalogFilePath, string imagesFolderPath)
        {
            // 호환성 유지를 위해 간단한 구현을 남겨두지만, 사용은 권장하지 않습니다.
            var result = new List<DonkeyFrame>();
            if (!File.Exists(catalogFilePath)) return result;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            foreach (var line in File.ReadLines(catalogFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try
                {
                    var frame = JsonSerializer.Deserialize<DonkeyFrame>(line, jsonOptions);
                    if (frame == null) continue;
                    frame.SourceCatalogPath = catalogFilePath;
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
