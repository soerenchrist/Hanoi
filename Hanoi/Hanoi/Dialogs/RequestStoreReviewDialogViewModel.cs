using Hanoi.Dialogs.Base;
using Plugin.StoreReview;
using Prism.Commands;
using Xamarin.Essentials;

namespace Hanoi.Dialogs
{
    public class RequestStoreReviewDialogViewModel : DialogViewModelBase
    {
        private DelegateCommand? _review;
        public DelegateCommand Review => _review ??= new DelegateCommand(ExecuteReview);


        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            Preferences.Set("HasReviewed", true);
        }

        private async void ExecuteReview()
        {
#if DEBUG
            var testMode = true;
#else
            var testMode = false;
#endif
            await CrossStoreReview.Current.RequestReview(testMode);

            Close.Execute();
        }
    }
}
