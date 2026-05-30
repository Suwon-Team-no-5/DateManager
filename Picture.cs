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

            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var temp = Image.FromStream(fs))
            {
                pb.Image = new Bitmap(temp);
            }
        }
    }
}