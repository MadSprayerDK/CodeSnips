using System.Drawing;

namespace ManoSoftware.Extensions.ImageExtensions
{
    public static class CropExtension
    {
        public static Image Crop(this Bitmap baseImage, float x, float y, float x2, float y2)
        {
            var imgX = (int)(baseImage.Width * x);
            var imgY = (int)(baseImage.Height * y);
            var imgW = (int)(baseImage.Width * (x2 - x));
            var imgH = (int)(baseImage.Height * (y2 - y));

            return baseImage.Clone(new Rectangle(imgX, imgY, imgW, imgH), baseImage.PixelFormat);
        }
    }
}
