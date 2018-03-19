using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
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

namespace SetBackground
{
    class Program
    {
        static TimeSpan startTime = TimeSpan.Zero;
        static TimeSpan interval = TimeSpan.FromSeconds(25);

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

            var lastSong = string.Empty;
            var spotify = new SpotifyWeb(spotifyRedirectUrl, spotifRedirectPort, spotifyKey, Scope.UserReadPlaybackState);
            var musicMatch = new MusicXMatchAPI(musicXMatchKey);
            var msText = new MicrosoftTextAnalytics(MSAnalyticsKey);
            var flickr = new FlickrAPI(flickrKey);
            var pexels = new PexelAPI(pexelsAPI);

            Console.WriteLine("==========================S T A R T==========================");

            var timer = new System.Threading.Timer((e) =>
            {
                Console.WriteLine(Environment.NewLine + "**New Iteration**");
                var song = spotify.GetCurrentSong();
                if(song != null && song.Title != null)
                {
                    if (lastSong != song.Title)
                    {
                        lastSong = song.Title;
                        Console.WriteLine($"{song.Artist} - {song.Title}: " + Environment.NewLine);
                        var lyrics = musicMatch.GetLyricsAndLanguage(lastSong, song.Artist);
                        if(string.IsNullOrEmpty(lyrics.Item1))
                        {
                            Console.WriteLine("*2nd Call to lyrics Service");
                            lyrics = musicMatch.GetLyricsAndLanguage(lastSong, null);
                        }
                        lyrics.Item1 = lyrics.Item1.Replace("\n", " ").Replace("  ", " ").ToLower();
                        Console.WriteLine(lyrics.Item1);
                        Console.WriteLine(lyrics.Item2 + Environment.NewLine);

                        var songLanguage = msText.GetLanguage(lyrics.Item1);
                        var songKeys = msText.ExtractKeyPhrases(lyrics.Item1, songLanguage);

                        Console.WriteLine(string.Join(Environment.NewLine, songKeys) + Environment.NewLine);

                        var textToSearch = GetTextToSearchImage(songKeys);
                        textToSearch = string.IsNullOrEmpty(textToSearch) ? song.Title : textToSearch;
                         
                        string photo = pexels.GetImageUrlFromText(textToSearch);
                        if(string.IsNullOrEmpty(photo))
                            photo = flickr.GetImageUrlFromText(textToSearch);
                        Console.WriteLine($"{textToSearch}: {photo}");

                        var fileName = photo.DownloadImageFromUrl("C:/newBackground");
                        SetWallpaper(fileName);
                    }
                    else
                        Console.WriteLine("no new song");
                }
                    else
                    Console.WriteLine("no song");
            }, null, startTime, interval);

            Console.ReadLine();
        }

        static string GetTextToSearchImage(string[] keys)
        {
            if (!keys.Any())
                return string.Empty;

            var result = keys.FirstOrDefault(x => x.Contains(" "));

            return /*result ??*/ keys[0].Contains(" ") ?
                                keys[0] :
                                keys[1].Contains(" ") ?
                                keys[1] :
                                string.Format($"{keys[0]} {keys[1]}");
        }

        static void SetWallpaper(string fileName)
        {

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
