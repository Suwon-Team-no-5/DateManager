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
            // 1. 기존 이미지 해제 (필수!)
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                // 2. 파일 스트림을 사용하여 파일 잠금 방지
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Image = Image.FromStream(fs);
                }
            }
        }
    }
}