using Xamarin.Essentials;

namespace Hanoi.Logic
{
    public class GameSettings
    {
        public bool ShowDiscNumbers
        {
            get => Preferences.Get(nameof(ShowDiscNumbers), true);
            set => Preferences.Set(nameof(ShowDiscNumbers), value);
        }
    }
}
