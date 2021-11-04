using Hanoi.Logic;
using Hanoi.Pages.Base;
using Prism.Navigation;
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

        public GamePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _elapsedTime = Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => _stopwatch.Elapsed)
                .Select(x => x.ToString(@"hh\:mm\:ss"))
                .ToProperty(this, x => x.ElapsedTime);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Discs"))
            {
                var discCount = parameters.GetValue<int>("Discs");
                GameLogic = new GameLogic(discCount);
                _stopwatch.Reset();
                _stopwatch.Start();
            } else
            {
                GoBack.Execute();
            }
            base.OnNavigatedTo(parameters);
        }
    }
}
