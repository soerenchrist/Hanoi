using Hanoi.Logic;
using Hanoi.Models;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Prism.Navigation;
using Prism.Services;
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

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly IDialogService _dialogService;
        private readonly DataService _dataService;
        public GamePageViewModel(INavigationService navigationService,
            IDialogService dialogService,
            DataService dataService) : base(navigationService)
        {
            _elapsedTime = Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => _stopwatch.Elapsed)
                .Select(FormatTime)
                .ToProperty(this, x => x.ElapsedTime);
            _dialogService = dialogService;
            _dataService = dataService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Discs"))
            {
                var discCount = parameters.GetValue<int>("Discs");
                GameLogic = new GameLogic(discCount);

                GameLogic.WhenAnyValue(x => x.GameWon)
                    .Where(x => x)
                    .Do(_ => GameWon())
                    .Subscribe();

                _stopwatch.Reset();
                _stopwatch.Start();
            } else
            {
                GoBack.Execute();
            }
            base.OnNavigatedTo(parameters);
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

            var parameters = await _dialogService.ShowDialogAsync($"GameFinished?Highscore={highScore}&Time={_stopwatch.ElapsedMilliseconds}");
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
