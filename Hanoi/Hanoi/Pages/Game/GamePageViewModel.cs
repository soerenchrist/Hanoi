using Hanoi.Logic;
using Hanoi.Models;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Hanoi.Services.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Hanoi.Pages.Game
{
    public class GamePageViewModel : ViewModelBase
    {
        private ObservableAsPropertyHelper<string> _elapsedTime;
        public string ElapsedTime => _elapsedTime.Value;

        private GameLogic? _gameLogic;
        public GameLogic? GameLogic
        {
            get => _gameLogic;
            private set => this.RaiseAndSetIfChanged(ref _gameLogic, value);
        }

        private int _countDown = 3;
        public int CountDown
        {
            get => _countDown;
            private set => this.RaiseAndSetIfChanged(ref _countDown, value);
        }

        private ObservableAsPropertyHelper<bool> _gameRunning;
        public bool GameRunning => _gameRunning.Value;

        public bool ShowNumbers => _settingsService.ShowNumbers;

        
        private DelegateCommand? _pause;
        public DelegateCommand Pause => _pause ??= new DelegateCommand(ExecutePause);
        
        private readonly IDialogService _dialogService;
        private readonly DataService _dataService;
        private readonly ISettingsService _settingsService;

        public GamePageViewModel(INavigationService navigationService,
            IDialogService dialogService,
            ISettingsService settingsService,
            DataService dataService) : base(navigationService)
        {
            _dialogService = dialogService;
            _dataService = dataService;
            _settingsService = settingsService;

            var countdownChanged = this.WhenAnyValue(x => x.CountDown);
            var countdownFinished = countdownChanged.Where(x => x == 0);

            _gameRunning = this.WhenAnyValue(x => x.CountDown)
                .Select(x => x == 0)
                .ToProperty(this, x => x.GameRunning);

            Observable.Interval(TimeSpan.FromSeconds(1))
                .TakeUntil(countdownFinished)
                .Do(_ =>
                {
                    CountDown--;
                })
                .Subscribe();

            this.WhenAnyValue(x => x.GameRunning)
                .Where(x => x)
                .Do(_ =>
                {
                    GameLogic?.Stopwatch.Start();
                }).Subscribe();

            _elapsedTime = Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => GameLogic?.Stopwatch.Elapsed ?? new TimeSpan())
                .Select(FormatTime)
                .ToProperty(this, x => x.ElapsedTime);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.Back)
                return;
            if (parameters.ContainsKey("Discs"))
            {
                var discCount = parameters.GetValue<int>("Discs");
                StartNewGame(discCount);
            } 
            else if (parameters.ContainsKey("SavedGame"))
            {
                var savedGame = parameters.GetValue<SavedGame>("SavedGame");
                LoadGame(savedGame);
            }
            else
            {
                GoBack.Execute();
                return;
            }

            base.OnNavigatedTo(parameters);
        }

        private void StartNewGame(int discCount)
        {
            GameLogic = new GameLogic(discCount);
            _dataService.CurrentGame = GameLogic;
            GameLogic.WhenAnyValue(x => x.GameWon)
                .Where(x => x)
                .Do(_ => GameWon())
                .Subscribe();
        }

        private void LoadGame(SavedGame savedGame)
        {
            GameLogic = new GameLogic(savedGame);
            _dataService.CurrentGame = GameLogic;
            GameLogic.WhenAnyValue(x => x.GameWon)
                .Where(x => x)
                .Do(_ => GameWon())
                .Subscribe();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.Back)
            {
                _dataService.IncrementGameCount();
                _dataService.SaveCurrentGame();
                _dataService.CurrentGame = null;
            }
            base.OnNavigatedFrom(parameters);
        }

        private async void ExecutePause()
        {
            GameLogic?.Stopwatch.Stop();
            var result = await _dialogService.ShowDialogAsync("GamePaused");
            if (result.Parameters.ContainsKey("GoToMainMenu"))
            {
                await NavigationService.GoBackAsync();
                return;
            }

            if (result.Parameters.ContainsKey("RestartGame"))
            {
                StartNewGame(GameLogic!.NumberOfDiscs);
            }

            GameLogic?.Stopwatch.Start();
        }

        private async void GameWon()
        {
            if (GameLogic == null)
                return;

            GameLogic.Stopwatch.Stop();
            var fastestTime = _dataService.GetFastestTime(GameLogic.NumberOfDiscs);
            bool highScore = GameLogic.Stopwatch.ElapsedMilliseconds < fastestTime;

            _dataService.AddHighscore(new HighscoreItem
            {
                DateTime = DateTime.Now,
                NumberOfDiscs = GameLogic.NumberOfDiscs,
                TimeInMilliseconds = GameLogic.Stopwatch.ElapsedMilliseconds,
                MovesNeeded = GameLogic.MoveCount
            });

            var dialogParameters = new DialogParameters
            {
                { "Highscore", highScore },
                { "Time", GameLogic.Stopwatch.Elapsed }
            };

            var parameters = await _dialogService.ShowDialogAsync($"GameFinished", dialogParameters);
            if (parameters.Parameters?.ContainsKey("ShowHighscores") ?? false)
            {
                await NavigationService.NavigateAsync($"../Highscores?NumberOfDiscs={GameLogic.NumberOfDiscs}");
            }
            else
            {
                GoBack.Execute();
            }
        }

        private string FormatTime(TimeSpan time)
            => time.ToString(@"hh\:mm\:ss");
        
    }
}
