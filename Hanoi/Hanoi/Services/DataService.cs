using Hanoi.Logic;
using Hanoi.Models;
using Hanoi.Services.Abstractions;
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

        private readonly ISettingsService _settingsService;
        public DataService(ISettingsService settingsService)
        {
            var basePath = FileSystem.AppDataDirectory;
            var filePath = Path.Combine(basePath, "database.db");
            _db = new SQLiteConnection(filePath);
            _settingsService = settingsService;
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

        public List<HighscoreItem> GetTopHighscores(int numberOfDiscs)
            => _db.Table<HighscoreItem>()
            .Where(x => x.NumberOfDiscs == numberOfDiscs)
            .OrderBy(x => x.TimeInMilliseconds)
            .Take(5)
            .ToList();

        public int GetPositionOfHighscoreItem(HighscoreItem item)
            => _db.Table<HighscoreItem>()
                .Where(x => x.NumberOfDiscs == item.NumberOfDiscs)
                .Where(x => x.TimeInMilliseconds < item.TimeInMilliseconds)
                .Count() + 1;

        public void IncrementGameCount()
        {
            if (_settingsService.IsPro)
                return;

            var count = _db.Table<AdCount>().Count();
            _db.Insert(new AdCount()
            {
                Count = count + 1
            });
        }

        public bool ShouldRequestStoreReview()
        {
            if (_settingsService.HasReviewed)
                return false;

            return _db.Table<HighscoreItem>().Count() > 5;
        }

        public bool ShouldShowAd()
        {
            if (_settingsService.IsPro)
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
