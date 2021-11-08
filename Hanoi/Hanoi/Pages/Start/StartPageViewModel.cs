using Hanoi.Pages.Base;
using Hanoi.Services;
using Hanoi.Util;
using MarcTron.Plugin;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace Hanoi.Pages.Start
{
    public class StartPageViewModel : ViewModelBase
    {
        private const int MinDiscs = 3;
        private const int MaxDiscs = 20;
        private bool _isPro;
        public bool IsPro
        {
            get => _isPro;
            private set => this.RaiseAndSetIfChanged(ref _isPro, value);
        }

        private string? _bannerAdId;
        public string BannerAdId => _bannerAdId ??= AdUtil.GetBannerAdId();

        private int _numberOfDiscs = 3;
        public int NumberOfDiscs
        {
            get => _numberOfDiscs;
            set => this.RaiseAndSetIfChanged(ref _numberOfDiscs, value);
        }

        private bool _hasSavedGame;
        public bool HasSavedGame
        {
            get => _hasSavedGame;
            private set => this.RaiseAndSetIfChanged(ref _hasSavedGame, value);
        }

        private readonly ObservableAsPropertyHelper<string> _discsText;
        public string DiscsText => _discsText.Value;

        private DelegateCommand? _plus;
        private DelegateCommand? _minus;
        private DelegateCommand? _startGame;
        private DelegateCommand? _resumeGame;
        public DelegateCommand Plus => _plus ??= new DelegateCommand(() => NumberOfDiscs++, 
            () => NumberOfDiscs < MaxDiscs)
            .ObservesProperty(() => NumberOfDiscs);
        public DelegateCommand Minus => _minus ??= new DelegateCommand(() => NumberOfDiscs--,
            () => NumberOfDiscs > MinDiscs)
            .ObservesProperty(() => NumberOfDiscs);

        public DelegateCommand StartGame => _startGame ??= new DelegateCommand(ExecuteStartGame);
        public DelegateCommand ResumeGame => _resumeGame ??= new DelegateCommand(ExecuteResumeGame);

        private readonly DataService _dataService;

        public StartPageViewModel(INavigationService navigationService,
            DataService dataService) : base(navigationService)
        {
            _dataService = dataService;
            _discsText = this.WhenAnyValue(x => x.NumberOfDiscs)
                .Select(x => $"{x} discs")
                .ToProperty(this, x => x.DiscsText);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsPro = Preferences.Get("Pro", false);
            HasSavedGame =  _dataService.HasSavedGame();
            
            if (parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.Back)
            {
                if (_dataService.ShouldShowAd())
                {
                    static void ShowAd(object sender, EventArgs args)
                    {
                        if (CrossMTAdmob.Current.IsInterstitialLoaded())
                            CrossMTAdmob.Current.ShowInterstitial();

                        CrossMTAdmob.Current.OnInterstitialLoaded -= ShowAd;
                    }

                    CrossMTAdmob.Current.OnInterstitialLoaded += ShowAd;
                    CrossMTAdmob.Current.LoadInterstitial(AdUtil.GetInterstitialAdId());
                }
            }
        }

        private async void ExecuteResumeGame()
        {
            var savedGame = _dataService.GetSavedGame();
            if (savedGame == null)
                StartGame.Execute();

            var result = await NavigationService.NavigateAsync("Game", new NavigationParameters 
            {
                { "SavedGame", savedGame }
            });
            if (!result.Success)
                Debugger.Break();
        }

        private void ExecuteStartGame()
        {
            Navigate.Execute($"Game?Discs={NumberOfDiscs}");
        }
    }
}
