
using SQLite;
using System;

namespace Hanoi.Models
{
    public class HighscoreItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int NumberOfDiscs { get; set; }
        public long TimeInMilliseconds { get; set; }
        public DateTime DateTime { get; set; }

        [Ignore]
        public int Position { get; set; }
    }
}
