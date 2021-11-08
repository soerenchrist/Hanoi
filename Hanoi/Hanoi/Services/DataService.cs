using Hanoi.Logic;
using Hanoi.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials;

namespace Hanoi.Services
{
    public class DataService
    {
        private readonly SQLiteConnection _db;
        public GameLogic? CurrentGame { get; set; }

        public DataService()
        {
            var basePath = FileSystem.AppDataDirectory;
            var filePath = Path.Combine(basePath, "database.db");
            _db = new SQLiteConnection(filePath);
        }

        public void Initialize()
        {
            _db.CreateTable<HighscoreItem>();
            _db.CreateTable<SavedGame>();
            _db.CreateTable<AdCount>();
        }

        public void AddHighscore(HighscoreItem item)
        {
            _db.DeleteAll<SavedGame>();
            _db.Insert(item);
        }

        public void SaveCurrentGame()
        {
            if (CurrentGame == null)
                return;
            if (CurrentGame.GameWon)
                return;


            var savedGame = CurrentGame.ToSavedGame();
            _db.DeleteAll<SavedGame>();
            _db.Insert(savedGame);
        }

        public bool HasSavedGame()
            => _db.Table<SavedGame>().Count() > 0;

        public SavedGame? GetSavedGame()
            => _db.Table<SavedGame>().FirstOrDefault();
        
        public IEnumerable<HighscoreItem> GetHighscoreItems(int numberOfDiscs)
        {
            var items = _db.Table<HighscoreItem>().Where(x => x.NumberOfDiscs == numberOfDiscs)
                               .OrderBy(x => x.TimeInMilliseconds)
                               .ToList();

            int position = 1;
            foreach (var item in items)
            {
                item.Position = position;
                position++;
            }

            return items;
        }

        public void IncrementGameCount()
        {
            if (Preferences.Get("Pro", false))
                return;

            var count = _db.Table<AdCount>().Count();
            _db.Insert(new AdCount()
            {
                Count = count + 1
            });
        }

        public bool ShouldShowAd()
        {
            if (Preferences.Get("Pro", false))
                return false;

            var adCount = _db.Table<AdCount>().Count();
            if (adCount >= 3)
            {
                _db.DeleteAll<AdCount>();
                return true;
            }

            return false;
        }

        public int GetTotalMoves()
            => _db.Table<HighscoreItem>().Sum(x => x.MovesNeeded)
                + _db.Table<SavedGame>().Sum(x => x.MoveCount);

        public long GetFastestTime(int numberOfDiscs)
            => _db.Table<HighscoreItem>()
                .Where(x => x.NumberOfDiscs == numberOfDiscs)
                .OrderBy(x => x.TimeInMilliseconds)
                .FirstOrDefault()?.TimeInMilliseconds ?? long.MaxValue;
    }
}
