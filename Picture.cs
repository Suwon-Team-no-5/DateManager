using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DateManager
{
    public class Picture
    {
        public void LoadImageToPictureBox(PictureBox pb, string imagePath)
        {
            Image? oldImage = pb.Image;
            pb.Image = null;
            oldImage?.Dispose();

            pb.SizeMode = PictureBoxSizeMode.Zoom;

            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return;

            try
            {
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = Image.FromStream(fs))
                {
                    pb.Image = new Bitmap(temp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("이미지 로드 오류: " + ex.Message);
            }
        }
    }
}