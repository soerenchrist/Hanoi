using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Hanoi.Util
{
    public static class Colors
    {

        public static SKColor GetColor(int id)
        {
            var realId = id % 8;

            var resourceId = $"Disc{realId}";

            Color color = (Color) Application.Current.Resources[resourceId];

            return color.ToSKColor();
        }




        public static bool IsDarkColor(SKColor color)
        {
            // Standard formula
            var luma = 0.2126 * color.Red + 0.7152 * color.Green + 0.0722 * color.Blue;

            return luma < 128;
        }
    }
}
