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

            Console.WriteLine("==========================S T A R T==========================");

            var timer = new System.Threading.Timer((e) =>
            {
                Console.WriteLine(Environment.NewLine + "**New Iteration**");
                var song = GetCurrentSong(spotifyRedirectUrl, spotifRedirectPort, spotifyKey);
                if(song != null && song.Title != null)
                {
                    if (lastSong != song.Title)
                    {
                        lastSong = song.Title;
                        Console.WriteLine($"{song.Artist} - {song.Title}: " + Environment.NewLine);

                        var lyrics = GetLyricsAndLanguage(musicXMatchKey, lastSong, song.Artist);

                        lyrics.Item1 = lyrics.Item1.Replace("\n", " ").Replace("  ", " ").ToLower();
                        Console.WriteLine(lyrics.Item1);
                        Console.WriteLine(lyrics.Item2 + Environment.NewLine);

                        var songKeys = ExtractKeyPhrasesFromLyrics(MSAnalyticsKey, lyrics.Item1);

                        Console.WriteLine(string.Join(Environment.NewLine, songKeys) + Environment.NewLine);

                        var textToSearch = GetTextToSearchImage(songKeys);
                        textToSearch = string.IsNullOrEmpty(textToSearch) ? song.Title : textToSearch;

                        var photoUrl = GetPhotoUrl(flickrKey, pexelsAPI, textToSearch);
                        Console.WriteLine($"{textToSearch}: {photoUrl}");

                        SetWallpaper(photoUrl);
                    }
                    else
                        Console.WriteLine("no new song");
                }
                    else
                    Console.WriteLine("no song");
            }, null, startTime, interval);

            Console.ReadLine();
        }

        static SongResult GetCurrentSong(string spotifyRedirectUrl, int spotifyRedirectPort, string spotifyKey)
        {
            var spotify = new SpotifyWeb(spotifyRedirectUrl, spotifyRedirectPort, spotifyKey, Scope.UserReadPlaybackState);
            return spotify.GetCurrentSong();
        }

        static (string, string) GetLyricsAndLanguage(string musicXMatchKey, string songName, string artistName)
        {
            var musicMatch = new MusicXMatchAPI(musicXMatchKey);
            var lyrics = musicMatch.GetLyricsAndLanguage(songName, artistName);

            if (string.IsNullOrEmpty(lyrics.Item1))
            {
                Console.WriteLine("*2nd Call to lyrics Service");
                lyrics = musicMatch.GetLyricsAndLanguage(songName, null);
            }

            return lyrics;
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

        static string[] ExtractKeyPhrasesFromLyrics(string MSAnalyticsKey, string lyrics)
        {
            var msText = new MicrosoftTextAnalytics(MSAnalyticsKey);
            var songLanguage = msText.GetLanguage(lyrics);
            return msText.ExtractKeyPhrases(lyrics, songLanguage);
        }

        static string GetPhotoUrl(string flickrKey, string pexelsAPI, string textToSearch)
        {
            var flickr = new FlickrAPI(flickrKey);
            var pexels = new PexelAPI(pexelsAPI);

            string photo = pexels.GetImageUrlFromText(textToSearch);
            if (string.IsNullOrEmpty(photo))
                photo = flickr.GetImageUrlFromText(textToSearch);

            return photo;
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
