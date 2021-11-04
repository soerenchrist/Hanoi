using Hanoi.Dialogs;
using Hanoi.Pages.Game;
using Hanoi.Pages.Highscores;
using Hanoi.Pages.Start;
using Hanoi.Services;
using Prism.Ioc;
using System.Diagnostics;
using Xamarin.Forms;

namespace Hanoi
{
    public partial class App
    {
        protected override async void OnInitialized()
        {
            InitializeComponent();

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
            containerRegistry.RegisterDialog<GameFinishedDialog, GameFinishedDialogViewModel>("GameFinished");
        }
    }
}
