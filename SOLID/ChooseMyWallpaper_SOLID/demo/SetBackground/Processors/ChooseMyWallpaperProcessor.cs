using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.Processors
{
    internal class ChooseMyWallpaperProcessor
    {
        private IGetSongProvider getSongProvider;
        private IGetLyricsProvider getLyricsProvider;
        private ILanguageProvider languageProvider;
        private IImageSearchProvider imageSearchProvider;

        public ChooseMyWallpaperProcessor(IGetSongProvider getSongProvider, IGetLyricsProvider getLyricsProvider, ILanguageProvider languageProvider, IImageSearchProvider imageSearchProvider)
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
