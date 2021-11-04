using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;

namespace Hanoi.Dialogs.Base
{
    public class DialogViewModelBase : ReactiveObject, IDialogAware
    {
        public event Action<IDialogParameters?>? RequestClose;

        private DelegateCommand? _close;
        private DelegateCommand<DialogParameters>? _closeWithParams;
        public DelegateCommand Close => _close ??= new DelegateCommand(() => RequestClose?.Invoke(null));
        public DelegateCommand<DialogParameters> CloseWithParams => _closeWithParams ??= new DelegateCommand<DialogParameters>(
            x => RequestClose?.Invoke(x));
        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
