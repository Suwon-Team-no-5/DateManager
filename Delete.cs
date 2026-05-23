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
            }
            // 성공 알림은 UI 호출부에서 처리하거나 여기서 처리

            catch (System.Exception ex)
            {
                throw new System.Exception($"파일 삭제 실패: {ex.Message}");
            }
        }

        // 여러 프레임 삭제 메서드 추가(윤형규가 추가함 오류 발생시 우선 주석처리 해볼 것)(crtl + / 로 다중 주석처리 가능)
        // 
        public void RemoveFrames(List<DonkeyFrame> frameList, DonkeyFrame targetFrame1, DonkeyFrame targetFrame2)
        {
            if (targetFrame1 == null) return;
            if (targetFrame2 == null) return;
            if (!string.IsNullOrEmpty(targetFrame1.FullImagePath) && File.Exists(targetFrame1.FullImagePath)&&!string.IsNullOrEmpty(targetFrame2.FullImagePath) && File.Exists(targetFrame2.FullImagePath))
            {
                for (int i = targetFrame1.FrameIndex; i < targetFrame2.FrameIndex; i++) { 
                    File.Delete(frameList[i].FullImagePath);
                    if (frameList.Contains(frameList[i]))
                    {
                        frameList.Remove(frameList[i]);
                    }
                }
            }
            
        }
    }
}