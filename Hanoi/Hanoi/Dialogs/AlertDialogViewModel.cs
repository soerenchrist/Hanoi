using Hanoi.Dialogs.Base;
using Prism.Services.Dialogs;
using ReactiveUI;

namespace Hanoi.Dialogs
{
    public class AlertDialogViewModel : DialogViewModelBase
    {
        private string _text = "";
        public string Text
        {
            get => _text;
            private set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private string _buttonText = "";
        public string ButtonText
        {
            get => _buttonText;
            private set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }


        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            if (parameters.ContainsKey("Text"))
            {
                Text = parameters.GetValue<string>("Text");
            }
            if (parameters.ContainsKey("ButtonText"))
            {
                ButtonText = parameters.GetValue<string>("ButtonText");
            }
        }
    }
}
