using System.Threading.Tasks;

namespace Hanoi.Services
{
    public interface IGameService
    {
        Task<bool> SilentSignInAsync();
        bool IsSignedIn();
        Task<bool> SignInAsync();
        Task SignOutAsync();
        Task ShowLeaderboardAsync(string leaderboardId);
    }
}
