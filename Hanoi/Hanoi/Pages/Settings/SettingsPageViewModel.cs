using Hanoi.Logic;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Plugin.InAppBilling;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Hanoi.Pages.Settings
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public GameSettings GameSettings { get; } = new();
        private bool _isPro;
        public bool IsPro 
        {
            get => _isPro;
            set => this.RaiseAndSetIfChanged(ref _isPro, value);
        }

        private DelegateCommand? _purchase;
        public DelegateCommand Purchase => _purchase ??= new DelegateCommand(ExecutePurchase);

        private IBillingService _billingService;
        private IPageDialogService _pageDialogService;
        public SettingsPageViewModel(INavigationService navigationService,
            IBillingService billingService,
            IPageDialogService pageDialogService) : base(navigationService)
        {
            _billingService = billingService;
            _pageDialogService = pageDialogService;
            IsPro = Preferences.Get("Pro", false);
        }

        private async void ExecutePurchase()
        {
            var connected = await CheckConnectivity("Offline", "You are currently offline. Try again later.");
            if (!connected)
                return;

            try
            {
                var restored = await _billingService.RestorePurchase();
                if (restored)
                    return;
            }
            catch (InAppBillingPurchaseException)
            {

            }

            try
            {
                var purchased = await _billingService.Purchase();
                if (purchased)
                {
                    IsPro = true;
                }
            }
            catch (InAppBillingPurchaseException ex)
            {
                var message = ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => "The app store is currently unavailable.",
                    PurchaseError.BillingUnavailable => "The app store is currently unavailable.",
                    PurchaseError.PaymentInvalid => "The payment was invalid!",
                    PurchaseError.PaymentNotAllowed => "The payment is not allowed",
                    _ => "Unknown error"
                };

                await _pageDialogService.DisplayAlertAsync("Error", message, "Ok");
            }
        }

        private async ValueTask<bool> CheckConnectivity(string title, string message)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                return true;

            await _pageDialogService.DisplayAlertAsync(title, message, "Ok");
            return false;
        }
    }
}
