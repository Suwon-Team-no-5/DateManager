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

                string catalogFolder = Path.GetDirectoryName(catalogPath) ?? ""; // catalog 파일의 폴더 경로
                string backupFolder = Path.Combine(catalogFolder, "backup"); // 백업 폴더 경로
                Directory.CreateDirectory(backupFolder); // 백업 폴더가 없으면 생성

                string backupPath = Path.Combine(
                    backupFolder,
                    $"{Path.GetFileNameWithoutExtension(catalogPath)}_{timestamp}{Path.GetExtension(catalogPath)}");
                // 원본 catalog 파일을 백업 폴더로 복사 (덮어쓰기 방지)
                File.Copy(catalogPath, backupPath, overwrite: false); // 백업 파일이 이미 존재하는 경우 예외 발생
                result.BackupFiles.Add(backupPath); // 백업 파일 경로를 결과에 추가

                string[] originalLines = File.ReadAllLines(catalogPath); // 원본 catalog 파일의 모든 라인을 읽어옴
                var remainingLines = new List<string>(originalLines.Length); // 삭제되지 않은 라인들을 저장할 리스트
                int deletedInFile = 0; // 현재 catalog 파일에서 삭제된 라인 수를 카운트

                foreach (string line in originalLines)
                {
                    if (ShouldDeleteLine(line, deleteKeys))// 해당 라인이 삭제 대상인지 확인
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
