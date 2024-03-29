﻿using Hanoi.Pages.Base;
using Hanoi.Services;
using Hanoi.Services.Abstractions;
using Hanoi.Themes;
using Hanoi.Util;
using MarcTron.Plugin;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
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

        private int _totalMoves;
        public int TotalMoves
        {
            get => _totalMoves;
            private set => this.RaiseAndSetIfChanged(ref _totalMoves, value);
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
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        public StartPageViewModel(INavigationService navigationService,
            DataService dataService,
            ISettingsService settingsService,
            IDialogService dialogService) : base(navigationService)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;
            _dataService = dataService;
            _discsText = this.WhenAnyValue(x => x.NumberOfDiscs)
                .Select(x => $"{x} discs")
                .ToProperty(this, x => x.DiscsText);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsPro = _settingsService.IsPro;
            HasSavedGame = _dataService.HasSavedGame();
            TotalMoves = _dataService.GetTotalMoves();

            if (_dataService.ShouldRequestStoreReview())
            {
                _dialogService.ShowDialogAsync("RequestReview");
                // if requesting a review, don't show an ad
                return;
            }
            
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

        private async void ExecuteStartGame()
        {
            if (HasSavedGame)
            {
                var result = await _dialogService.ShowDialogAsync("ConfirmNewGame");
                if (!result.Parameters.ContainsKey("NewGame"))
                {
                    return;
                }
            }

            Navigate.Execute($"Game?Discs={NumberOfDiscs}");
        }
    }
}
