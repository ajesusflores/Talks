using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.MusicAPI
{
    class SpotifyWeb : IMusicProvider
    {
        private static SpotifyWebAPI _spotifyAPI;

        public SpotifyWeb(string redirectUrl, int listeningPort, string clientId, Scope scope)
        {
            var webapi = new WebAPIFactory(redirectUrl, listeningPort, clientId, scope);
            _spotifyAPI = webapi.GetWebApi().Result;
        }

        public SongResult GetCurrentSong()
        {
            FullTrack song = _spotifyAPI.GetPlayback().Item ?? _spotifyAPI.GetPlayingTrack().Item;

            if (song != null)
                return new SongResult() { Album = song.Album.Name, Artist = string.Join(", ", song.Artists.Select(x => x.Name)), Title = song.Name };

            return null;
        }
    }
}
