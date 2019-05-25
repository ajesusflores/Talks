# PleaseSetMyBackground
Set your wallpaper based on the music you are listening to

Ho it works:
1) Spotify API gets the playing track
2) MusicXMatch gets the lyrics
3) Microsoft Text Analytics extracts key phrases from the lyrics
4.1) Pexels returns an Image based on the key phrases
4.2) If Pexels didn't return anything, repeat the search against Flickr
