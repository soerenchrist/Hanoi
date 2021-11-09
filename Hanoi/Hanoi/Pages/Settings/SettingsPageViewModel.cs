using Hanoi.Logic;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Hanoi.Services.Abstractions;
using Hanoi.Themes;
using Hanoi.Util;
using Plugin.InAppBilling;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Hanoi.Pages.Settings
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public bool ShowNumbers
        { 
            get => _settingsService.ShowNumbers;
            set => _settingsService.ShowNumbers = value;
        }

        private bool _isPro;
        public bool IsPro 
        {
            get => _isPro;
            set => this.RaiseAndSetIfChanged(ref _isPro, value);
        }

        private GameTheme _selectedTheme;
        public GameTheme SelectedTheme
        {
            get => _selectedTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        }

        public string Version => VersionTracking.CurrentVersion;

        private DelegateCommand? _purchase;
        private DelegateCommand? _sendMail;
        private ReactiveCommand<GameTheme, Unit>? _setTheme;
        public DelegateCommand Purchase => _purchase ??= new DelegateCommand(ExecutePurchase);
        public DelegateCommand SendMail => _sendMail ??= new DelegateCommand(ExecuteSendMail);
        public ReactiveCommand<GameTheme, Unit> SetTheme => _setTheme ??= ReactiveCommand.Create<GameTheme, Unit>(ExecuteSetTheme);

        private readonly IBillingService _billingService;
        private readonly IPageDialogService _pageDialogService;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        public SettingsPageViewModel(INavigationService navigationService,
            IBillingService billingService,
            ISettingsService settingsService,
            IPageDialogService pageDialogService,
            IDialogService dialogService) : base(navigationService)
        {
            _billingService = billingService;
            _pageDialogService = pageDialogService;
            _dialogService = dialogService;
            _settingsService = settingsService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsPro = _settingsService.IsPro;
            SelectedTheme = ThemeHelper.CurrentTheme;
        }

        private async void ExecuteSendMail()
        {
            try
            {
                var info = new StringBuilder();
                info.AppendLine($"Version: {Version}");
                info.AppendLine($"Platform: {DeviceInfo.Platform}");
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

        private Unit ExecuteSetTheme(GameTheme theme)
        {
            ThemeHelper.ChangeTheme(theme);
            SelectedTheme = theme;

            return Unit.Default;
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
                {
                    await _dialogService.ShowAlertAsync("Your purchase has been restored. Thank you!", "Close");
                    return;
                }
            }
            catch (Exception)
            {

            }

            try
            {
                var purchased = await _billingService.Purchase();
                if (purchased)
                {
                    IsPro = true;
                    await _dialogService.ShowAlertAsync("Your purchase was successful. Thank you!", "Close");
                }
            }
            catch (Exception ex)
            {
                string? message;
                if (ex is InAppBillingPurchaseException inAppBillingPurchaseException)
                {
                    message = inAppBillingPurchaseException.PurchaseError switch
                    {
                        PurchaseError.AppStoreUnavailable => "The app store is currently unavailable.",
                        PurchaseError.BillingUnavailable => "The app store is currently unavailable.",
                        PurchaseError.PaymentInvalid => "The payment was invalid!",
                        PurchaseError.PaymentNotAllowed => "The payment is not allowed",
                        _ => "Unknown error"
                    };
                } else
                {
                    message = $"Unknown error: {ex.Message}";
                }
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
