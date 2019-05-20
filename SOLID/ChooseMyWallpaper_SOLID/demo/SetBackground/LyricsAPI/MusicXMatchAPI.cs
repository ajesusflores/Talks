using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.LyricsAPI
{
    class MusicXMatchAPI : ILyricsProvider
    {
        LyricsApi _api;

        public MusicXMatchAPI(string apiKey)
        {
            Configuration.Default.ApiKey.Add("apikey", apiKey);
            _api = new LyricsApi();
        }

        public string GetLyrics(string songName)
        {
            return GetLyrics(songName, null);
        }

        public string GetLyrics(string songName, string artistName)
        {
            return ParseLyricsResponse(_api.MatcherLyricsGetGet(null, null, songName, artistName));
        }

        public (string, string) GetLyricsAndLanguage(string songName, string artistName)
        {
            try
            {
                var lyrics = ParseLyricsResponse(_api.MatcherLyricsGetGet(null, null, songName, artistName));
                var language = ParseLanguageResponse(_api.MatcherLyricsGetGet(null, null, songName, artistName));
                return
                    (lyrics, language);
            }
            catch (ApiException)
            {   
                return ("Restricted", "en");
            }
        }

        public string GetLyrics(string songName, string artistName, string albumName)
        {
            return GetLyrics(songName, artistName);
        }

        private string ParseLyricsResponse(InlineResponse2007 response)
        {
            if (response == null || response.Message == null || response.Message.Header == null || response.Message.Header.StatusCode != 200)
                return string.Empty;
            if (response.Message.Body.Lyrics.Restricted == 1)
                return "Restricted";

            return response.Message.Body.Lyrics.LyricsBody.Replace("******* This Lyrics is NOT for Commercial use *******", string.Empty);
        }

        private string ParseLanguageResponse(InlineResponse2007 response)
        {
            if (response == null || response.Message == null || response.Message.Header == null || response.Message.Header.StatusCode != 200)
                return string.Empty;

            return response.Message.Body.Lyrics.LyricsLanguage;
        }
    }
}

