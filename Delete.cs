using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DateManager
{
    public class FileRemover // 'Delete'는 키워드와 겹칠 수 있어 FileRemover 추천
    {
        public void RemoveFrame(List<DonkeyFrame> frameList, DonkeyFrame targetFrame)
        {
            if (targetFrame == null) return;

            try
            {
                if (!string.IsNullOrEmpty(targetFrame.FullImagePath) && File.Exists(targetFrame.FullImagePath))
                {
                    File.Delete(targetFrame.FullImagePath);
                }

                if (frameList.Contains(targetFrame))
                {
                    frameList.Remove(targetFrame);
                }

                // 성공 알림은 UI 호출부에서 처리하거나 여기서 처리
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"파일 삭제 실패: {ex.Message}");
            }
        }
    }
}