using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Hanoi.Themes
{
    public static class ThemeHelper
    {
        public static GameTheme CurrentTheme = GameTheme.Blue;
        public static void ChangeTheme(GameTheme theme)
        {
            if (CurrentTheme == theme)
                return;

            var applicationResourceDictionary = Application.Current.Resources;
            ResourceDictionary newTheme = GetDictionaryForTheme(theme);
            ManuallyCopyThemes(newTheme, applicationResourceDictionary);
            SaveTheme(theme);
            CurrentTheme = theme;
        }

        public static ResourceDictionary GetDictionaryForTheme(GameTheme theme)
             => theme switch
             {
                 GameTheme.Red => new RedTheme(),
                 GameTheme.BlackWhite => new BlackWhiteTheme(),
                 GameTheme.Light => new LightTheme(),
                 GameTheme.Rainbow => new RainbowTheme(),
                 _ => new BaseTheme()
             };


        private static void SaveTheme(GameTheme theme)
        {
            Preferences.Set("Theme", theme.ToString());
        }

        public static GameTheme LoadTheme()
        {
            var value = Preferences.Get("Theme", "Blue");
            return (GameTheme) Enum.Parse(typeof(GameTheme), value);
        }

        private static void ManuallyCopyThemes(ResourceDictionary fromResource, ResourceDictionary toResource)
        {
            foreach (var item in fromResource.Keys)
            {
                toResource[item] = fromResource[item];
            }
        }
    }
}
