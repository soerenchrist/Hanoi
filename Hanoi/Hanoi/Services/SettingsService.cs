
using Hanoi.Services.Abstractions;
using Xamarin.Essentials;

namespace Hanoi.Services
{
    public class SettingsService : ISettingsService
    {
        public bool IsPro 
        { 
            get => Preferences.Get("Pro", false); 
            set => Preferences.Set("Pro", value); 
        }
        public string ProReceipt
        {
            get => Preferences.Get(nameof(ProReceipt), "");
            set => Preferences.Set(nameof(ProReceipt), value);
        }

        public bool ShowNumbers
        {
            get => Preferences.Get(nameof(ShowNumbers), true);
            set => Preferences.Set(nameof(ShowNumbers), value);
        }
        public bool HasReviewed
        {
            get => Preferences.Get(nameof(HasReviewed), true);
            set => Preferences.Set(nameof(HasReviewed), value);
        }

    }
}
