using Hanoi.Util;
using SkiaSharp;

namespace Hanoi.Models
{
    public class Disc
    {
        public int Size { get; }
        public SKColor Color { get; }
        public Disc(int size)
        {
            Size = size;
            Color = Colors.GetColor(size - 1);
        }

    }
}
