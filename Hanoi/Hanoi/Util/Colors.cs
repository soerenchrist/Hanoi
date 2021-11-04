using SkiaSharp;

namespace Hanoi.Util
{
    public static class Colors
    {
        private static string[] _colors = new[]
        {
            "#ff0000",
            "#00ff00",
            "#0000ff",
            "#ffff00",
            "#00ffff",
            "#ff00ff",
            "#fff00f"
        };

        public static SKColor GetColor(int id)
        {
            var realId = id % _colors.Length;
            return SKColor.Parse(_colors[realId]);
        }

    }
}
