using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

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
        // 프레임리스트, 삭제할 프레임1, 삭제할 프레임2를 인자로 받아와 반복문을 돌리며 삭제
        // 시간복잡도가 O(n)으로 프레임리스트의 크기에 비례하여 증가하므로, 프레임리스트가 클 경우 성능 저하가 발생할 수 있음
        public void RemoveFrames(List<DonkeyFrame> frameList, DonkeyFrame targetFrame1, DonkeyFrame targetFrame2)
        {
            if (targetFrame1 == null) return;
            if (targetFrame2 == null) return;
            int startIndex = targetFrame1.FrameIndex;
            int endIndex = targetFrame2.FrameIndex;
            try
            {
                // 2. 파일 먼저 안전하게 삭제 (리스트 구조를 변경하지 않으므로 인덱스가 꼬이지 않음)
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (i >= 0 && i < frameList.Count)
                    {
                        string path = frameList[i].FullImagePath;
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
                //    // 3. 리스트에서 일괄 삭제 (O(N) 속도 보장 및 버그 해결)
                frameList.RemoveAll(f => f.FrameIndex >= startIndex && f.FrameIndex <= endIndex);

            }
            catch (System.Exception ex)
            {

                throw new System.Exception($"파일 삭제 실패: {ex.Message}");
            }



            //public void RemoveFrames(List<DonkeyFrame> frameList, DonkeyFrame targetFrame1, DonkeyFrame targetFrame2)
            //{
            //    if (targetFrame1 == null || targetFrame2 == null || frameList == null) return;
            //    if (string.IsNullOrEmpty(targetFrame1.FullImagePath) || !File.Exists(targetFrame1.FullImagePath)) return;
            //    if (string.IsNullOrEmpty(targetFrame2.FullImagePath) || !File.Exists(targetFrame2.FullImagePath)) return;


            //    int startIndex = targetFrame1.FrameIndex;
            //    int endIndex = targetFrame2.FrameIndex;
            //    int countToRemove = endIndex - startIndex;
            //    if (startIndex >= 0 && startIndex + countToRemove <= frameList.Count)
            //    {
            //        frameList.RemoveRange(startIndex, countToRemove); // 내부 메모리 이동이 1번만 발생
            //    }

            //}
        }
    }
}
