using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetBackground.LyricsAPI;

namespace SetBackground.Processors
{
    internal class GetLyricsProvider : IGetLyricsProvider
    {
        private MusicXMatchAPI musicMatch;

        internal GetLyricsProvider(string musicXMatchKey)
        {
            musicMatch = new MusicXMatchAPI(musicXMatchKey);
        }

        public string GetLyrics(string songName, string artistName)
        {
            var lyrics = musicMatch.GetLyricsAndLanguage(songName, artistName);

            if (string.IsNullOrEmpty(lyrics.Item1))
            {
                Console.WriteLine("*2nd Call to lyrics Service");
                lyrics = musicMatch.GetLyricsAndLanguage(songName, null);
            }

            return lyrics.Item1;
        }
    }
}
