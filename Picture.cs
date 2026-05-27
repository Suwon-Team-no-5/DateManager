using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace DateManager
{
    public class Picture
    {
        // 간단한 메모리 기반 썸네일 캐시
        // 키: 이미지 파일 경로, 값: Image (썸네일)
        private static readonly ConcurrentDictionary<string, Image> _thumbCache = new ConcurrentDictionary<string, Image>();

        // 픽처박스에 이미지를 비차단 방식으로 로드하여 UI 스레드 정체를 줄입니다.
        // 내부적으로 파일을 바이트로 읽어 MemoryStream으로 이미지 생성 후 컨트롤에 할당합니다.
        public void LoadImageToPictureBox(PictureBox pb, string imagePath)
        {
            // 기존 이미지 해제는 UI 스레드에서 안전하게 수행
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return;
            }

            // 컨트롤 크기는 UI 스레드에서 읽어 로컬에 저장 (비동기 내부에서 참조 금지)
            var targetSize = pb.ClientSize;

            // 캐시에 이미 있으면 바로 UI 스레드에 적용
            if (_thumbCache.TryGetValue(imagePath, out Image cached))
            {
                try
                {
                    pb.InvokeIfRequired(() =>
                    {
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                        // 이미지 객체는 여러 컨텍스트에서 공유하지 않도록 복제해서 할당
                        pb.Image = (Image)cached.Clone();
                    });
                    return;
                }
                catch
                {
                    // 캐시 사용 중 에러가 나면 캐시에서 제거
                    _thumbCache.TryRemove(imagePath, out _);
                }
            }

            // 비동기 로드 시작
            Task.Run(() =>
            {
                try
                {
                    byte[] data = File.ReadAllBytes(imagePath);
                    using (var ms = new MemoryStream(data))
                    using (var img = Image.FromStream(ms))
                    {
                        // 썸네일 생성: 대상 크기에 맞춰 고품질 리사이즈
                        int w = Math.Max(1, targetSize.Width);
                        int h = Math.Max(1, targetSize.Height);
                        var thumb = new Bitmap(w, h);
                        using (var g = Graphics.FromImage(thumb))
                        {
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.DrawImage(img, 0, 0, w, h);
                        }

                        // 캐시에 추가 (성능/메모리 제한을 위해 단순 정책 사용)
                        try
                        {
                            if (_thumbCache.Count > 200)
                            {
                                // 너무 커지면 간단히 비움 (더 정교한 LRU 필요 시 확장)
                                foreach (var k in _thumbCache.Keys)
                                {
                                    if (_thumbCache.TryRemove(k, out Image old))
                                    {
                                        old.Dispose();
                                    }
                                }
                            }

                            _thumbCache[imagePath] = (Image)thumb.Clone();
                        }
                        catch { }

                        // UI 스레드에 이미지 할당
                        pb.InvokeIfRequired(() =>
                        {
                            pb.SizeMode = PictureBoxSizeMode.Zoom;
                            pb.Image = (Image)thumb.Clone();
                        });

                        // 임시 thumb은 Dispose
                        thumb.Dispose();
                    }
                }
                catch
                {
                    // 로드 실패는 조용히 무시
                }
            });
        }
    }
}