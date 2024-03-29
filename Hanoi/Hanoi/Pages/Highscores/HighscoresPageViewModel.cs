﻿using DynamicData;
using Hanoi.Models;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Hanoi.Services.Abstractions;
using Hanoi.Util;
using Hanoi.ViewModels;
using MarcTron.Plugin;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace Hanoi.Pages.Highscores
{
    public class HighscoresPageViewModel : ViewModelBase
    {
        private string? _bannerAdId;
        public string BannerAdId => _bannerAdId ??= AdUtil.GetBannerAdId();

        private bool _isPro;
        public bool IsPro
        {
            get => _isPro;
            private set => this.RaiseAndSetIfChanged(ref _isPro, value);
        }

        public List<int> DiscSizes { get; } = new List<int>();

        private int _selectedDiscSizeIndex = 3;
        public int SelectedDiscSizeIndex
        {
            get => _selectedDiscSizeIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedDiscSizeIndex, value);
        }

        private SourceCache<HighscoreItem, int> _highscoresCache = new(x => x.Id);
        private readonly ReadOnlyObservableCollection<HighscoreViewModel> _highscores;
        public ReadOnlyObservableCollection<HighscoreViewModel> Highscores => _highscores;

        private readonly DataService _dataService;
        private readonly ISettingsService _settingsService;
        public HighscoresPageViewModel(INavigationService navigationService,
            DataService dataService,
            ISettingsService settingsService) : base(navigationService)
        {
            _settingsService = settingsService;
            _dataService = dataService;
            for (int i = 3; i <= 20; i++)
            {
                DiscSizes.Add(i);
            }

            _highscoresCache.Connect()
                .Transform(x => new HighscoreViewModel(x))
                .SortBy(x => x.Time)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _highscores)
                .Subscribe();
            
            this.WhenAnyValue(x => x.SelectedDiscSizeIndex)
                .Do(LoadHighscores)
                .Subscribe();
        }

        private void LoadHighscores(int discSize)
        {
            var items = _dataService.GetHighscoreItems(discSize);
            _highscoresCache.Edit(x =>
            {
                x.Clear();
                x.AddOrUpdate(items);
            });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsPro = _settingsService.IsPro;
            if (parameters.ContainsKey("NumberOfDiscs"))
            {
                SelectedDiscSizeIndex = parameters.GetValue<int>("NumberOfDiscs");
            }
        }
    }
}
