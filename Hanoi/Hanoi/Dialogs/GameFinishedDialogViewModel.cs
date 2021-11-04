using Hanoi.Dialogs.Base;
using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;

namespace Hanoi.Dialogs
{
    public class GameFinishedDialogViewModel : DialogViewModelBase
    {
        private long _time;
        public long Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }

        private bool _highScore;
        public bool HighScore
        {
            get => _highScore;
            set => this.RaiseAndSetIfChanged(ref _highScore, value);
        }

        private DelegateCommand? _showHighscores;
        public DelegateCommand ShowHighscores => _showHighscores ??= new DelegateCommand(() => CloseWithParams.Execute(new DialogParameters()
        {
            { "ShowHighscores", true }
        }));

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Time = parameters.GetValue<long>("Time");
            HighScore = parameters.GetValue<bool>("Highscore");
        }
    }
}
