using System;
using System.IO;

public class BackupManager
{
    // 원본 카탈로그 파일의 전체 경로를 저장합니다.
    public string CurrentCatalogPath { get; set; }

    /// <summary>
    /// 백업 파일 이름을 받아 원본 카탈로그 파일로 복구합니다.
    /// </summary>
    public void RestoreFromBackup(string fullBackupFilePath, string tubFolderPath)
    {
        // 1. 선택한 백업 파일이 실제로 있는지 확인
        if (!File.Exists(fullBackupFilePath))
            throw new FileNotFoundException("선택한 백업 파일이 없습니다.", fullBackupFilePath);

        // 2. 덮어씌울 원본 파일 이름 유추 (예: catalog_0_20260528.catalog -> catalog_0.catalog)
        string backupFileName = Path.GetFileName(fullBackupFilePath);
        string originalFileName = "catalog_0.catalog"; // 기본값

        string[] parts = backupFileName.Split('_');
        if (parts.Length >= 2)
        {
            originalFileName = $"{parts[0]}_{parts[1]}.catalog";
        }

        // 3. 원본 위치에 그대로 덮어쓰기!
        string targetOriginalPath = Path.Combine(tubFolderPath, originalFileName);
        File.Copy(fullBackupFilePath, targetOriginalPath, overwrite: true);

        // 4. [핵심] 캐시 파일 무조건 삭제! 
        // 이걸 안 지우면 프로그램이 복원된 파일 대신 예전 10400개짜리 캐시를 읽어버립니다.
        string[] cacheFiles = Directory.GetFiles(tubFolderPath, "*.bin");
        foreach (string cache in cacheFiles)
        {
            File.Delete(cache);
        }
    }

    /// <summary>
    /// 카탈로그 변경 시 남은 기존 캐시 파일을 삭제하여 최신 데이터 로드를 돕습니다.
    /// </summary>
    private void DeleteCatalogCaches(string folderPath)
    {
        try
        {
            var files = Directory.GetFiles(folderPath, ".catalogcache_*.bin");
            foreach (string cacheFile in files)
            {
                File.Delete(cacheFile);
            }
            // 로그 추가: 진짜 삭제가 되었는지 확인
            //if (files.Length > 0) MessageBox.Show($"{files.Length}개의 캐시 파일을 삭제했습니다.");
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"캐시 삭제 실패: {ex.Message}");
        }
    }
}