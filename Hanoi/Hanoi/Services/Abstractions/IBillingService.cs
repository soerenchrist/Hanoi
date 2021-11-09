using System.Threading.Tasks;

namespace Hanoi.Services.Abstractions
{
    public interface IBillingService
    {
        Task<string> GetPrice();
        Task<bool> Purchase();
        Task<bool> RestorePurchase();
    }

}
