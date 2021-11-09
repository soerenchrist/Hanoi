using Hanoi.Dialogs.Base;
using Hanoi.Models;
using Hanoi.Services;
using Hanoi.ViewModels;
using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace Hanoi.Dialogs
{
    public class GameFinishedDialogViewModel : DialogViewModelBase
    {
        private HighscoreItem? _currentItem;
        public HighscoreItem? CurrentItem
        {
            get => _currentItem;
            set => this.RaiseAndSetIfChanged(ref _currentItem, value);
        }

        public ObservableCollection<HighscoreViewModel> Highscores { get; } = new ObservableCollection<HighscoreViewModel>();

        private readonly DataService _dataService;
        public GameFinishedDialogViewModel(DataService dataService)
        {
            _dataService = dataService;
        }


        public override void OnDialogOpened(IDialogParameters parameters)
        {
            CurrentItem = parameters.GetValue<HighscoreItem>("HighscoreItem");
            var highscores = _dataService.GetTopHighscores(CurrentItem.NumberOfDiscs);
            bool isTop5 = false;
            int index = 1;
            foreach (var item in highscores)
            {
                item.Position = index;
                bool found = item.Id == CurrentItem.Id;
                if (found)
                    isTop5 = true;
                Highscores.Add(new HighscoreViewModel(item, found));
                index++;
            }
            if (!isTop5)
            {
                CurrentItem.Position = _dataService.GetPositionOfHighscoreItem(CurrentItem);
                Highscores.RemoveAt(Highscores.Count - 1);
                Highscores.Add(new HighscoreViewModel(CurrentItem, true));
            }
        }
    }
}
