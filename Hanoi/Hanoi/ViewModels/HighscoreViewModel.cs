using Hanoi.Models;
using ReactiveUI;
using System;

namespace Hanoi.ViewModels
{
    public class HighscoreViewModel : ReactiveObject
    {
        public int Position { get; }
        public DateTime DateTime { get; }
        public TimeSpan Time { get; set; }

        public HighscoreViewModel(HighscoreItem item)
        {
            DateTime = item.DateTime;
            Position = item.Position;
            Time = new TimeSpan(item.TimeInMilliseconds * 10000);
        }
    }
}
