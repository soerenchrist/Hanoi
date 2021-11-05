using System;
using System.Diagnostics;

namespace Hanoi.Util
{
    public class ResumableStopWatch : Stopwatch
    {
        public TimeSpan StartOffset { get; private set; }
        public ResumableStopWatch()
        {

        }

        public ResumableStopWatch(TimeSpan startOffset)
        {
            StartOffset = startOffset;
        }

        public ResumableStopWatch(long millis) : this(new TimeSpan(millis * 10000))
        { }

        public new long ElapsedMilliseconds => base.ElapsedMilliseconds + (long)StartOffset.TotalMilliseconds;
        public new long ElapsedTicks => base.ElapsedTicks + StartOffset.Ticks;
        public new TimeSpan Elapsed => base.Elapsed + StartOffset;
    }
}
