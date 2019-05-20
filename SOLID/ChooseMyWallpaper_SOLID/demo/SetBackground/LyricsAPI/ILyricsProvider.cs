using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.LyricsAPI
{
    interface ILyricsProvider
    {
        string GetLyrics(string songName);
        string GetLyrics(string songName, string artistName);
        string GetLyrics(string songName, string artistName, string albumName);
    }
}
