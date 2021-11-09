using Hanoi.Dialogs.Base;
using Hanoi.Services.Abstractions;
using Plugin.StoreReview;
using Prism.Commands;
using Xamarin.Essentials;

namespace Hanoi.Dialogs
{
    public class RequestStoreReviewDialogViewModel : DialogViewModelBase
    {
        private DelegateCommand? _review;
        public DelegateCommand Review => _review ??= new DelegateCommand(ExecuteReview);

        private readonly ISettingsService _settingsService;
        public RequestStoreReviewDialogViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            _settingsService.HasReviewed = true;
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
