using Hanoi.Pages.Base;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;
using System.Reactive.Linq;

namespace Hanoi.Pages.Start
{
    public class StartPageViewModel : ViewModelBase
    {
        private const int MinDiscs = 3;
        private const int MaxDiscs = 20;

        private int _numberOfDiscs = 3;
        public int NumberOfDiscs
        {
            get => _numberOfDiscs;
            set => this.RaiseAndSetIfChanged(ref _numberOfDiscs, value);
        }

        private ObservableAsPropertyHelper<string> _discsText;
        public string DiscsText => _discsText.Value;

        private DelegateCommand? _plus;
        private DelegateCommand? _minus;
        private DelegateCommand? _startGame;
        public DelegateCommand Plus => _plus ??= new DelegateCommand(() => NumberOfDiscs++, 
            () => NumberOfDiscs < MaxDiscs)
            .ObservesProperty(() => NumberOfDiscs);
        public DelegateCommand Minus => _minus ??= new DelegateCommand(() => NumberOfDiscs--,
            () => NumberOfDiscs > MinDiscs)
            .ObservesProperty(() => NumberOfDiscs);

        public DelegateCommand StartGame => _startGame ??= new DelegateCommand(ExecuteStartGame);

        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _discsText = this.WhenAnyValue(x => x.NumberOfDiscs)
                .Select(x => $"{x} discs")
                .ToProperty(this, x => x.DiscsText);
        }

        private void ExecuteStartGame()
        {
            Navigate.Execute($"Game?Discs={NumberOfDiscs}");
        }
    }
}
