using Prism.Services.Dialogs;
using System.Threading.Tasks;

namespace Hanoi.Util
{
    public static class DialogUtil
    {
        public static Task<IDialogResult> ShowAlertAsync(this IDialogService dialogService, string text, string? buttonText = null)
        {
            var parameters = new DialogParameters
            {
                { "Text", text }
            };
            if (buttonText != null)
            {
                parameters.Add("ButtonText", buttonText);
            }
            return dialogService.ShowDialogAsync("Alert", parameters);
        }
    }
}
