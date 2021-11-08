using Hanoi.Dialogs.Base;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Hanoi.Dialogs
{
    public class ConfirmNewGameDialogViewModel : DialogViewModelBase
    {
        private DelegateCommand? _newGame;
        public DelegateCommand NewGame => _newGame ??= new DelegateCommand(ExecuteNewGame);

        private void ExecuteNewGame()
        {
            CloseWithParams.Execute(new DialogParameters
            {
                { "NewGame", true }
            });
        }
    }

}
