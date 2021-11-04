﻿using DynamicData;
using Hanoi.Models;
using Hanoi.Pages.Base;
using Hanoi.Services;
using Prism.Navigation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Hanoi.Pages.Highscores
{
    public class HighscoresPageViewModel : ViewModelBase
    {
        public List<int> DiscSizes { get; } = new List<int>();

        private int _selectedDiscSizeIndex = 3;
        public int SelectedDiscSizeIndex
        {
            get => _selectedDiscSizeIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedDiscSizeIndex, value);
        }

        private SourceCache<HighscoreItem, int> _highscoresCache = new(x => x.Id);
        private readonly ReadOnlyObservableCollection<HighscoreItem> _highscores;
        public ReadOnlyObservableCollection<HighscoreItem> Highscores => _highscores;

        private readonly DataService _dataService;
        public HighscoresPageViewModel(INavigationService navigationService,
            DataService dataService) : base(navigationService)
        {
            _dataService = dataService;
            for (int i = 3; i <= 20; i++)
            {
                DiscSizes.Add(i);
            }

            _highscoresCache.Connect()
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
        }
    }
}