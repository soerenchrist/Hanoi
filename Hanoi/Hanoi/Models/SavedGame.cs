using SQLite;

namespace Hanoi.Models
{
    public class SavedGame
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int NumberOfDiscs { get; set; }
        public long CurrentTime { get; set; }
        public int MoveCount { get; set; }

        public string LeftStack { get; set; } = null!;
        public string RightStack { get; set; } = null!;
        public string MiddleStack { get; set; } = null!;
    }
}
