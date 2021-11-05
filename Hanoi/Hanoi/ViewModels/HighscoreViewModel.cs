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


        public int MovesNeeded { get; }
        public int PossibleMoves { get; }
        public bool PerfectGame { get; }

        public HighscoreViewModel(HighscoreItem item)
        {
            DateTime = item.DateTime;
            Position = item.Position;
            Time = new TimeSpan(item.TimeInMilliseconds * 10000);

            PossibleMoves = (int) Math.Pow(2, item.NumberOfDiscs) - 1;
            MovesNeeded = item.MovesNeeded;

            PerfectGame = MovesNeeded == PossibleMoves;
        }
    }
}
