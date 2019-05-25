using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
//using 
using SetBackground.MusicAPI;
using SetBackground.LyricsAPI;
using SetBackground.LanguageAPI;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Specialized;
using SetBackground.PhotographyAPI;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Forms;
using SetBackground.Processors;

namespace SetBackground
{
    class Program
    {
        static TimeSpan startTime = TimeSpan.Zero;
        static TimeSpan interval = TimeSpan.FromSeconds(35);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        static void Main(string[] args)
        {
            var languageConfig = ConfigurationManager.GetSection("APIs/LanguageAPI") as NameValueCollection;
            var MSAnalyticsKey = languageConfig["MSTextAnalyticsK1"];

            var lyricsConfig = ConfigurationManager.GetSection("APIs/LyricsAPI") as NameValueCollection;
            var musicXMatchKey = lyricsConfig["MusicXMatchKey"];

            var musicConfig = ConfigurationManager.GetSection("APIs/MusicAPI") as NameValueCollection;
            var spotifyKey = musicConfig["SpotifyKey"];
            var spotifyRedirectUrl = musicConfig["SpotifyRedirectUrl"];
            var spotifRedirectPort = int.Parse(musicConfig["SpotifyListeningPort"]);

            var photosConfig = ConfigurationManager.GetSection("APIs/PhotographyAPI") as NameValueCollection;
            var flickrKey = photosConfig["FlickrAPI"];
            var pexelsAPI = photosConfig["PexelsAPI"];

            // Implementation based on contracts (Interfaces)
            IGetSongProvider songProvider = new GetSongProvider(spotifyRedirectUrl, spotifRedirectPort, spotifyKey);
            IGetLyricsProvider lyricsProvider = new GetLyricsProvider(musicXMatchKey);
            ILanguageProvider languageProvider = new LanguageProvider(MSAnalyticsKey);
            IImageSearchProvider imageSearchProvider = new ImageSearchProvider(flickrKey, pexelsAPI);

            ChooseMyWallpaperProcessor processor = new ChooseMyWallpaperProcessor(songProvider, lyricsProvider, languageProvider, imageSearchProvider);

            Console.WriteLine("==========================S T A R T==========================");

            var timer = new System.Threading.Timer((e) =>
            {
                Console.WriteLine(Environment.NewLine + "**New Iteration**");
                SetWallpaper(processor.ChooseMyWallpaper());
            }, null, startTime, interval);

            Console.ReadLine();
        }

        static void SetWallpaper(string imageUrl)
        {
            var fileName = imageUrl.DownloadImageFromUrl("C:/newBackground");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                fileName,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
