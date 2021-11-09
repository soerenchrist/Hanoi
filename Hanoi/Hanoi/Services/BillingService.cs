using Hanoi.Services.Abstractions;
using Plugin.InAppBilling;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Hanoi.Services
{
    public class BillingService : IBillingService
    {
        private const string ProductId = "hanoi_pro";

        private ISettingsService _settingsService;
        public BillingService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<string> GetPrice()
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                    throw new InAppBillingPurchaseException(PurchaseError.GeneralError, "Could not connect to in app billing service");

                var items = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, ProductId);
                var item = items.FirstOrDefault(x => x.ProductId == ProductId);
                if (item == null)
                    throw new InvalidOperationException($"Could not get product for product id {ProductId}");

                return item.LocalizedPrice;

            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        public async Task<bool> Purchase()
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                    throw new InAppBillingPurchaseException(PurchaseError.BillingUnavailable);

                var purchase = await CrossInAppBilling.Current.PurchaseAsync(ProductId, ItemType.InAppPurchase);

                if (purchase == null)
                    throw new InAppBillingPurchaseException(PurchaseError.ItemUnavailable);
                else if (purchase.State == PurchaseState.Purchased)
                {
                    _settingsService.IsPro = true;
                    _settingsService.ProReceipt = purchase.PurchaseToken ?? string.Empty;

                    try
                    {
                        await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchase.PurchaseToken);
                    }
                    catch (Exception ex)
                    {
                        throw new InAppBillingPurchaseException(PurchaseError.GeneralError, ex);
                    }
                    return true;
                }
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
            return false;
        }

        public async Task<bool> RestorePurchase()
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();
                if (!connected)
                    throw new InAppBillingPurchaseException(PurchaseError.BillingUnavailable);

                var purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase);
                if (purchases?.Any(p => p.ProductId == ProductId) ?? false)
                {
                    _settingsService.IsPro = true;

                    var purchase = purchases.FirstOrDefault(p => p.ProductId == ProductId);
                    if (string.IsNullOrWhiteSpace(_settingsService.ProReceipt))
                    {
                        _settingsService.ProReceipt = purchase?.PurchaseToken ?? string.Empty;
                    }

                    var acknoledged = purchase?.IsAcknowledged ?? true;
                    if (!acknoledged)
                    {
                        try
                        {
                            await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchase?.PurchaseToken);
                        }
                        catch (Exception ex)
                        {
                            throw new InAppBillingPurchaseException(PurchaseError.GeneralError, ex);
                        }
                    }
                    return true;
                }
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }

            return false;
        }
    }

    public class MockBillingService : IBillingService
    {
        public MockBillingService()
        {
        }
        public async Task<string> GetPrice()
        {
            await Task.Delay(2000);
            return "2,00 €";
        }

        public async Task<bool> Purchase()
        {
            await Task.Delay(2000);
            Preferences.Set("Pro", true);
            return true;
        }

        public async Task<bool> RestorePurchase()
        {
            await Task.Delay(2000);
            Preferences.Set("Pro", true);
            return true;
        }
    }
}
