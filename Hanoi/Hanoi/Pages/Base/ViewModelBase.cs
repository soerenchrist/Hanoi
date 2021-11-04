using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using System.Diagnostics;

namespace Hanoi.Pages.Base
{
    public class ViewModelBase : ReactiveObject, INavigationAware, IInitialize
    {
        private DelegateCommand<string>? _navigate;
        public DelegateCommand<string> Navigate => _navigate ??= new DelegateCommand<string>(ExecuteNavigate);
        private DelegateCommand? _goBack;
        public DelegateCommand GoBack => _goBack ??= new DelegateCommand(ExecuteGoBack);

        protected INavigationService NavigationService { get; }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        private async void ExecuteNavigate(string route)
        {
            var result = await NavigationService.NavigateAsync(route);
            if (Debugger.IsAttached && !result.Success)
                Debugger.Break();
        }

        private async void ExecuteGoBack()
        {
            var result = await NavigationService.GoBackAsync();
            if (Debugger.IsAttached && !result.Success)
                Debugger.Break();
        }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
