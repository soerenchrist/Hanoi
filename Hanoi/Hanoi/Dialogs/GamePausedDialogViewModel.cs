using Hanoi.Dialogs.Base;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Hanoi.Dialogs
{
    public class GamePausedDialogViewModel : DialogViewModelBase
    {
        private DelegateCommand? _goToMainMenu;
        public DelegateCommand GoToMainMenu => _goToMainMenu ??= new DelegateCommand(
            () => CloseWithParams.Execute(new DialogParameters()
            {
                { "GoToMainMenu", true }
            }));
    }
}
