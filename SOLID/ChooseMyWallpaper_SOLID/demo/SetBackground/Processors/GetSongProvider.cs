using SetBackground.MusicAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;

namespace SetBackground.Processors
{
    internal class GetSongProvider : IGetSongProvider
    {
        private SpotifyWeb spotifyClient;

        internal GetSongProvider(string redirectUrl, int redirectPort, string spotifyKey)
        {
            spotifyClient = new SpotifyWeb(redirectUrl, redirectPort, spotifyKey, Scope.UserReadPlaybackState);
        }
        
        public SongResult GetCurrentSong()
        {
            return spotifyClient.GetCurrentSong();
        }
    }
}
