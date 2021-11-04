using Hanoi.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;

namespace Hanoi.Services
{
    public class DataService
    {
        private readonly SQLiteConnection _db;

        public DataService()
        {
            var basePath = FileSystem.AppDataDirectory;
            var filePath = Path.Combine(basePath, "database.db");
            _db = new SQLiteConnection(filePath);
        }

        public void Initialize()
        {
            _db.CreateTable<HighscoreItem>();
        }

        public void AddHighscore(HighscoreItem item)
        {
            _db.Insert(item);
        }

        public IEnumerable<HighscoreItem> GetHighscoreItems(int numberOfDiscs)
            => _db.Table<HighscoreItem>().Where(x => x.NumberOfDiscs == numberOfDiscs)
                    .OrderBy(x => x.TimeInMilliseconds);

        public long GetFastestTime(int numberOfDiscs)
            => _db.Table<HighscoreItem>()
                .Where(x => x.NumberOfDiscs == numberOfDiscs)
                .OrderBy(x => x.TimeInMilliseconds)
                .FirstOrDefault()?.TimeInMilliseconds ?? long.MaxValue;
    }
}
