using Hanoi.Dialogs.Base;
using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;

namespace Hanoi.Dialogs
{
    public class GameFinishedDialogViewModel : DialogViewModelBase
    {
        private TimeSpan _time;
        public TimeSpan Time
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
            Time = parameters.GetValue<TimeSpan>("Time");
            HighScore = parameters.GetValue<bool>("Highscore");
        }
    }
}
