using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DateManager
{
    public class DeleteResult
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
            if (frameList == null) throw new ArgumentNullException(nameof(frameList)); // frameList이 null인 경우 예외처리
            if (targetFrames == null) throw new ArgumentNullException(nameof(targetFrames));

            var selectedFrames = targetFrames
                .Where(frame => frame != null)
                .ToList();
            // 선택된 프레임 중에서 null이 아닌 것만 필터링

            var targets = selectedFrames
                .Where(frame => frame != null && !string.IsNullOrWhiteSpace(frame.SourceCatalogPath))
                .GroupBy(frame => Path.GetFullPath(frame.SourceCatalogPath))
                .ToList();
            // 선택된 프레임 중에서 SourceCatalogPath가 유효한 것만 필터링하고, 절대 경로로 그룹화

            if (targets.Count == 0)
            {
                throw new InvalidOperationException("삭제할 프레임의 원본 catalog 파일 정보를 찾을 수 없습니다. 데이터를 다시 로드한 뒤 시도해 주세요.");
            }

            var result = new DeleteResult();
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            foreach (var catalogGroup in targets)// 각 catalog 파일별로 그룹화된 프레임들을 처리
            {
                string catalogPath = catalogGroup.Key;
                if (!File.Exists(catalogPath))
                {
                    throw new FileNotFoundException("원본 catalog 파일을 찾을 수 없습니다.", catalogPath);
                }

                var deleteKeys = catalogGroup
                    .Select(CreateFrameKey)
                    .ToHashSet();

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

            frameList.RemoveAll(frame => selectedFrames.Contains(frame));

            return result;
        }

        private bool ShouldDeleteLine(string line, HashSet<string> deleteKeys)
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

        private void DeleteCatalogCaches(string folderPath)
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
                // 캐시 삭제 실패는 catalog 삭제 결과에 영향을 주지 않습니다.
            }
        }
    }
}
