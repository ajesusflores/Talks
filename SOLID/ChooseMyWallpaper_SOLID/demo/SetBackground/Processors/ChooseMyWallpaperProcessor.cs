using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.Processors
{
    internal class ChooseMyWallpaperProcessor
    {
        private GetSongProvider getSongProvider;
        private GetLyricsProvider getLyricsProvider;
        private LanguageProvider languageProvider;
        private ImageSearchProvider imageSearchProvider;

        public ChooseMyWallpaperProcessor(GetSongProvider getSongProvider, GetLyricsProvider getLyricsProvider, LanguageProvider languageProvider, ImageSearchProvider imageSearchProvider)
        {
            this.getSongProvider = getSongProvider;
            this.getLyricsProvider = getLyricsProvider;
            this.languageProvider = languageProvider;
            this.imageSearchProvider = imageSearchProvider;
        }

        public string ChooseMyWallpaper()
        {
            var song = getSongProvider.GetCurrentSong();

            var lyrics = getLyricsProvider.GetLyrics(song.Title, song.Artist);

            var keyPhrase = languageProvider.GetKeyPhraseFromLyrics(lyrics);
            keyPhrase = string.IsNullOrEmpty(keyPhrase) ? song.Title : keyPhrase;

            var imageUrl = imageSearchProvider.GetImageUrlFromText(keyPhrase);
            return imageUrl;
        }
    }
}
