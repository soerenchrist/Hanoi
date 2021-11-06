using Hanoi.Logic;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Prism.Navigation;
using System;

namespace Hanoi.Pages.Settings
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private IGameService _gameService;

        public GameSettings GameSettings { get; } = new();

        public SettingsPageViewModel(INavigationService navigationService,
            IGameService gameService) : base(navigationService)
        {
            _gameService = gameService;
        }
    }
}
