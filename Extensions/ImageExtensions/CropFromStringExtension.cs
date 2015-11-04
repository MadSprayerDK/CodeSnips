using System.Drawing;
using System.Globalization;

namespace ManoSoftware.Extensions.ImageExtensions
{
    public static class CropFromStringExtension
    {
        public static Image CropFromString(this Bitmap baseImage, string x, string y, string x2, string y2)
        {
            var xf = float.Parse(x, CultureInfo.InvariantCulture);
            var yf = float.Parse(y, CultureInfo.InvariantCulture);
            var x2f = float.Parse(x2, CultureInfo.InvariantCulture);
            var y2f = float.Parse(y2, CultureInfo.InvariantCulture);
            return baseImage.Crop(xf, yf, x2f, y2f);
        }
    }
}
