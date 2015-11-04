using System;
using System.Drawing;
using System.IO;

namespace ManoSoftware.Extensions.ImageExtensions
{
    public static class ImageFromBase64StringExtension
    {
        public static Bitmap ImageFromBase64String(string base64String)
        {
            var memoryStream = new MemoryStream(Convert.FromBase64String(base64String));
            return (Bitmap)Image.FromStream(memoryStream);
        }
    }
}
