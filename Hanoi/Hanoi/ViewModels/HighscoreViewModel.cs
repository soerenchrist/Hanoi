using Hanoi.Models;
using ReactiveUI;
using System;

namespace Hanoi.ViewModels
{
    public class HighscoreViewModel : ReactiveObject
    {
        public DateTime DateTime { get; }
        public TimeSpan Time { get; set; }

        public HighscoreViewModel(HighscoreItem item)
        {
            DateTime = item.DateTime;
            Time = new TimeSpan(item.TimeInMilliseconds * 10000);
        }
    }
}
