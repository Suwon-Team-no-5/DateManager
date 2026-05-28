using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DateManager
{
    public class Picture
    {
        // 픽처박스에 이미지를 띄워주는 전용 기능
        public void LoadImageToPictureBox(PictureBox pb, string imagePath)
        {
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    // 파일을 메모리로 한번에 읽어서 바로 닫음 (파일 점유 해제)
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                        pb.Image = Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    // 이미지 로드 실패 시 무시하거나 에러 처리
                    Console.WriteLine("이미지 로드 오류: " + ex.Message);
                }
            }
        }
    }
}
