using Hanoi.Logic;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Plugin.InAppBilling;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
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

        public string Version => VersionTracking.CurrentVersion;

        private DelegateCommand? _purchase;
        private DelegateCommand? _sendMail;
        public DelegateCommand Purchase => _purchase ??= new DelegateCommand(ExecutePurchase);
        public DelegateCommand SendMail => _sendMail ??= new DelegateCommand(ExecuteSendMail);

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

        private async void ExecuteSendMail()
        {
            try
            {
                var info = new StringBuilder();
                info.AppendLine($"Version: {Version}");
                info.AppendLine($"Platform: {DeviceInfo.Platform.ToString()}");
                info.AppendLine($"OS Version: {DeviceInfo.VersionString}");
                info.AppendLine($"Manufacturer: {DeviceInfo.Manufacturer}");
                info.AppendLine($"Model: {DeviceInfo.Model}");
                info.AppendLine();
                info.AppendLine($"Please describe your issue in detail here:");
                var message = new EmailMessage
                {
                    Subject = $"Issue with Hanoi - Speed Run",
                    Body = info.ToString(),
                    To = new List<string> { "s.christ@mailbox.org" }
                };

                await Email.ComposeAsync(message);
            }
            catch (Exception)
            {
                await _pageDialogService.DisplayAlertAsync("Error", "Could not send mail. Please contact s.christ@mailbox.org directly", "Ok");
            }
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
