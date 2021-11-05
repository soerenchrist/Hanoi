using Hanoi.Logic;
using Hanoi.Models;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;
using System.Diagnostics;
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

        public GameSettings GameSettings { get; } = new();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly IDialogService _dialogService;
        private readonly DataService _dataService;

        private DelegateCommand? _pause;
        public DelegateCommand Pause => _pause ??= new DelegateCommand(ExecutePause);

        public GamePageViewModel(INavigationService navigationService,
            IDialogService dialogService,
            DataService dataService) : base(navigationService)
        {
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
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }).Subscribe();

            _elapsedTime = Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => _stopwatch.Elapsed)
                .Select(FormatTime)
                .ToProperty(this, x => x.ElapsedTime);
            _dialogService = dialogService;
            _dataService = dataService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.Back)
                return;
            if (parameters.ContainsKey("Discs"))
            {
                var discCount = parameters.GetValue<int>("Discs");
                GameLogic = new GameLogic(discCount);

                GameLogic.WhenAnyValue(x => x.GameWon)
                    .Where(x => x)
                    .Do(_ => GameWon())
                    .Subscribe();
            } else
            {
                GoBack.Execute();
            }
            base.OnNavigatedTo(parameters);
        }

        private async void ExecutePause()
        {
            _stopwatch.Stop();
            var result = await _dialogService.ShowDialogAsync("GamePaused");
            if (result.Parameters.ContainsKey("GoToMainMenu"))
            {
                await NavigationService.GoBackAsync();
                return;
            }
            _stopwatch.Start();
        }

        private async void GameWon()
        {
            if (GameLogic == null)
                return;

            _stopwatch.Stop();
            var fastestTime = _dataService.GetFastestTime(GameLogic.NumberOfDiscs);
            bool highScore = _stopwatch.ElapsedMilliseconds < fastestTime;

            _dataService.AddHighscore(new HighscoreItem
            {
                DateTime = DateTime.Now,
                NumberOfDiscs = GameLogic.NumberOfDiscs,
                TimeInMilliseconds = _stopwatch.ElapsedMilliseconds,
            });

            var dialogParameters = new DialogParameters
            {
                { "Highscore", highScore },
                { "Time", _stopwatch.Elapsed }
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
