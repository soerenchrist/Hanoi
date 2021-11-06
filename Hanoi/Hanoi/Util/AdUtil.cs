using Xamarin.Forms;

namespace Hanoi.Util
{
    public static class AdUtil
    {
        public static string GetInterstitialAdId()
        {
#if DEBUG
            return "ca-app-pub-3940256099942544/1033173712";
#else
            return GetResource("InterstitialAdId");
#endif
        }

        public static string GetBannerAdId()
        {
#if DEBUG
            return "ca-app-pub-3940256099942544/6300978111";
#else
            return GetResource("BannerAdId");
#endif
        }
        private static string GetResource(string name)
        {
            return (string) Application.Current.Resources[name];
        }
    }
}
