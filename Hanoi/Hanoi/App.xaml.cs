using Hanoi.Dialogs;
using Hanoi.Pages.Game;
using Hanoi.Pages.Highscores;
using Hanoi.Pages.Settings;
using Hanoi.Pages.Start;
using Hanoi.Services;
using Hanoi.Themes;
using Prism;
using Prism.Ioc;
using System.Diagnostics;
using Xamarin.Forms;

namespace Hanoi
{
    public partial class App
    {

        public App(IPlatformInitializer initializer) : base(initializer) 
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            var theme = ThemeHelper.LoadTheme();
            ThemeHelper.ChangeTheme(theme);

            var result = await NavigationService.NavigateAsync("Nav/Start");
            if (!result.Success && Debugger.IsAttached)
                Debugger.Break();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var dataService = new DataService();
            dataService.Initialize();

            containerRegistry.RegisterInstance(dataService);

            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");
            containerRegistry.RegisterForNavigation<StartPage, StartPageViewModel>("Start");
            containerRegistry.RegisterForNavigation<GamePage, GamePageViewModel>("Game");
            containerRegistry.RegisterForNavigation<HighscoresPage, HighscoresPageViewModel>("Highscores");
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>("Settings");

            containerRegistry.RegisterDialog<GameFinishedDialog, GameFinishedDialogViewModel>("GameFinished");
            containerRegistry.RegisterDialog<GamePausedDialog, GamePausedDialogViewModel>("GamePaused");
            containerRegistry.RegisterDialog<ConfirmNewGameDialog, ConfirmNewGameDialogViewModel>("ConfirmNewGame");
            containerRegistry.RegisterDialog<RequestStoreReviewDialog, RequestStoreReviewDialogViewModel>("RequestReview");
            containerRegistry.RegisterDialog<AlertDialog, AlertDialogViewModel>("Alert");

            containerRegistry.Register<IBillingService, BillingService>();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            var dataService = Container.Resolve<DataService>();
            dataService.SaveCurrentGame();

            dataService.CurrentGame?.Stopwatch.Stop();
        }

        protected override void OnResume()
        {
            base.OnResume();

            var dataService = Container.Resolve<DataService>();
            dataService.CurrentGame?.Stopwatch.Start();
        }
    }
}
