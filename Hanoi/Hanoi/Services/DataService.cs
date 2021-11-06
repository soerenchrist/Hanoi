using Hanoi.Logic;
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

        public bool ShouldShowAd()
        {
            if (Preferences.Get("Pro", false))
                return false;

            var ads = _db.Table<AdCount>().ToList();
            if (ads.Count == 2)
            {
                _db.DeleteAll<AdCount>();
                return true;
            }
            _db.Insert(new AdCount()
            {
                Count = ads.Count + 1
            });

            return false;
        }

        public long GetFastestTime(int numberOfDiscs)
            => _db.Table<HighscoreItem>()
                .Where(x => x.NumberOfDiscs == numberOfDiscs)
                .OrderBy(x => x.TimeInMilliseconds)
                .FirstOrDefault()?.TimeInMilliseconds ?? long.MaxValue;
    }
}
