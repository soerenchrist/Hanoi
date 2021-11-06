using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Prism;
using Prism.Ioc;
using Hanoi.Services;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Ads;

namespace Hanoi.Droid
{
    [Activity(Label = "Hanoi", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Landscape, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPlatformInitializer
    {
        private GameService _gameService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);

            MobileAds.Initialize(this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            _gameService = new GameService();

            LoadApplication(new App(this));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _gameService.OnActivityResult(requestCode, resultCode, data);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IGameService>(_gameService);
        }
    }
}