using Hanoi.Pages.Game;
using Hanoi.Pages.Start;
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
            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");
            containerRegistry.RegisterForNavigation<StartPage, StartPageViewModel>("Start");
            containerRegistry.RegisterForNavigation<GamePage, GamePageViewModel>("Game");
        }
    }
}
