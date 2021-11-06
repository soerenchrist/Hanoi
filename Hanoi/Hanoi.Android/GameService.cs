#nullable enable
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Games;
using Android.Gms.Common.Apis;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Hanoi.Services;
using Android.App;
using Hanoi.Droid.Util;

namespace Hanoi.Droid
{
    public class GameService : IGameService
    {
        public const int RequestCodeSignIn = 9001;
        private readonly GoogleSignInClient _googleSignInClient;
        private IAchievementsClient? _achievementsClient;
        private ILeaderboardsClient? _leaderboardsClient;

        private TaskCompletionSource<bool>? _loginCts;
        public GameService()
        {
            _googleSignInClient = GoogleSignIn.GetClient(Platform.CurrentActivity, 
                new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultGamesSignIn).Build());
        }

        public async Task<bool> SilentSignInAsync()
        {
            var account = await _googleSignInClient.SilentSignInAsync();
            OnConnected(account);
            return account != null;
        }

        public bool IsSignedIn()
            => GoogleSignIn.GetLastSignedInAccount(Platform.CurrentActivity) != null;

        public Task<bool> SignInAsync()
        {
            _loginCts = new TaskCompletionSource<bool>();
            Platform.CurrentActivity.StartActivityForResult(_googleSignInClient.SignInIntent, RequestCodeSignIn);

            return _loginCts.Task;
        }

        public Task SignOutAsync()
            => _googleSignInClient.SignOutAsync();

        public async void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (requestCode == RequestCodeSignIn)
            {
                try
                { 
                    var account = await GoogleSignIn.GetSignedInAccountFromIntentAsync(intent);
                    OnConnected(account);
                } catch(ApiException)
                {
                    OnDisconnected();
                }
            }
        }

        public async Task ShowLeaderboardAsync(string leaderboardId)
        {
            if (_leaderboardsClient == null)
                return;

            var intentObject= await _leaderboardsClient.GetLeaderboardIntent(leaderboardId).ToAwaitableTask();
            if (intentObject is Intent intent)
                Platform.CurrentActivity.StartActivity(intent);
        }

        private void OnDisconnected()
        {
            _loginCts?.SetResult(false);
        }


        private void OnConnected(GoogleSignInAccount account)
        {
            _achievementsClient = GamesClass.GetAchievementsClient(Platform.CurrentActivity, account);
            _leaderboardsClient = GamesClass.GetLeaderboardsClient(Platform.CurrentActivity, account);
            
            _loginCts?.SetResult(true);
        }
    }
}