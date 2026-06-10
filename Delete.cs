using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DateManager
{
    public class DeleteResult //몇 줄 삭제했고 몇 줄 백업했는지 메세지 띄우기 위한 메서드
    {
        public int DeletedCount { get; set; }
        public List<string> BackupFiles { get; } = new List<string>();
    }

    public class FileRemover
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        public DeleteResult RemoveFramesFromCatalogs(List<DonkeyFrame> frameList, IEnumerable<DonkeyFrame> targetFrames)
        {
            if (frameList == null) throw new ArgumentNullException(nameof(frameList));
            if (targetFrames == null) throw new ArgumentNullException(nameof(targetFrames));

            // 1. 경로 정보가 있는 프레임과 없는 프레임을 구분합니다.
            var framesWithCatalog = targetFrames
                .Where(f => f != null && !string.IsNullOrWhiteSpace(f.SourceCatalogPath))
                .ToList();

            var framesWithoutCatalog = targetFrames
                .Where(f => f != null && string.IsNullOrWhiteSpace(f.SourceCatalogPath))
                .ToList();

            // 2. 만약 삭제할 카탈로그 대상이 하나도 없다면, 그냥 리스트에서만 제거하고 종료합니다.
            if (framesWithCatalog.Count == 0)
            {
                frameList.RemoveAll(frame => targetFrames.Contains(frame));
                return new DeleteResult(); // 빈 결과 반환
            }

            // 3. 이제 카탈로그 정보가 있는 것들만 처리
            var targets = framesWithCatalog
                .GroupBy(frame => Path.GetFullPath(frame.SourceCatalogPath))
                .ToList();

            var result = new DeleteResult();
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            foreach (var catalogGroup in targets)
            {
                string catalogPath = catalogGroup.Key;
                if (!File.Exists(catalogPath))
                {
                    // 여기서 예외를 던지지 말고 로그를 남기거나 건너뛰는 것이 안전합니다.
                    continue;
                }

                var deleteKeys = catalogGroup.Select(CreateFrameKey).ToHashSet();

                string catalogFolder = Path.GetDirectoryName(catalogPath) ?? "";
                string backupFolder = Path.Combine(catalogFolder, "backup");
                Directory.CreateDirectory(backupFolder);

                string backupPath = Path.Combine(
                    backupFolder,
                    $"{Path.GetFileNameWithoutExtension(catalogPath)}_{timestamp}{Path.GetExtension(catalogPath)}");

                File.Copy(catalogPath, backupPath, overwrite: false);
                result.BackupFiles.Add(backupPath);

                string[] originalLines = File.ReadAllLines(catalogPath);
                var remainingLines = new List<string>(originalLines.Length);
                int deletedInFile = 0;

                foreach (string line in originalLines)
                {
                    if (ShouldDeleteLine(line, deleteKeys))
                    {
                        deletedInFile++;
                        continue;
                    }
                    remainingLines.Add(line);
                }

                File.WriteAllLines(catalogPath, remainingLines);
                result.DeletedCount += deletedInFile;
                DeleteCatalogCaches(catalogFolder);
            }

            // 최종적으로 전체 리스트에서 제거
            frameList.RemoveAll(frame => targetFrames.Contains(frame));

            return result;
        }

        private bool ShouldDeleteLine(string line, HashSet<string> deleteKeys) //라인이 삭제 대상인지 확인하는 메서드
        {
            if (string.IsNullOrWhiteSpace(line)) return false;

            try
            {
                DonkeyFrame? frame = JsonSerializer.Deserialize<DonkeyFrame>(line, _jsonOptions);
                if (frame == null) return false;

                return deleteKeys.Contains(CreateFrameKey(frame));
            }
            catch
            {
                return false;
            }
        }

        private string CreateFrameKey(DonkeyFrame frame)
        {
            string imagePath = frame.ImagePath ?? "";
            string sessionId = frame.SessionId ?? "";
            return $"{frame.Index}|{sessionId}|{imagePath}";
        }

        private void DeleteCatalogCaches(string folderPath) //캐시에서도 지우는 메서드
        {
            try
            {
                foreach (string cacheFile in Directory.GetFiles(folderPath, ".catalogcache_*.bin"))
                {
                    File.Delete(cacheFile);
                }
            }
            catch
            {
                // 캐시 삭제 실패는 catalog 삭제 결과에 영향을 주지 않음
            }
        }
    }
}
