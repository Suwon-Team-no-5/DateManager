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
            // 1. 기존 이미지 해제 (메모리 누수 및 파일 잠김 방지)
            if (pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }

            // 2. 파일이 진짜 있는지 확인 후 로드
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pb.SizeMode = PictureBoxSizeMode.Zoom; // 이미지를 픽처박스 크기에 맞게 조절
                pb.Image = Image.FromFile(imagePath);
            }
        }
    }
}