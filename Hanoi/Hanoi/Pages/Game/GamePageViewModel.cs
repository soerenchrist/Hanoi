using Hanoi.Logic;
using Hanoi.Pages.Base;
using Prism.Navigation;
using Prism.Services;
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
        private readonly IPageDialogService _pageDialogService;

        public GamePageViewModel(INavigationService navigationService,
            IPageDialogService pageDialogService) : base(navigationService)
        {
            _elapsedTime = Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => _stopwatch.Elapsed)
                .Select(x => x.ToString(@"hh\:mm\:ss"))
                .ToProperty(this, x => x.ElapsedTime);
            _pageDialogService = pageDialogService;
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
            _stopwatch.Stop();
            await _pageDialogService.DisplayAlertAsync("Congrats!", $"You did it! Your time: {_stopwatch.Elapsed.ToString("c")}", "Ok");
            GoBack.Execute();
        }
    }
}
