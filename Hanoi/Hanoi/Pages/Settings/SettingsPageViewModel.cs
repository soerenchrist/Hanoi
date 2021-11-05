using Hanoi.Logic;
using Hanoi.Pages.Base;
using Prism.Navigation;

namespace Hanoi.Pages.Settings
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public GameSettings GameSettings { get; } = new();

        public SettingsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
