using System.Drawing;
using System.Drawing.Drawing2D;

namespace ManoSoftware.Extensions.ImageExtensions
{
    public static class ResizeExtension
    {
        public static Image Resize(this Image baseImage, int width, int height)
        {
            var resized = new Bitmap(width, height);
            using (var gr = Graphics.FromImage(resized))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(baseImage, new Rectangle(0, 0, resized.Width, resized.Height));
            }

            return resized;
        }
    }
}
