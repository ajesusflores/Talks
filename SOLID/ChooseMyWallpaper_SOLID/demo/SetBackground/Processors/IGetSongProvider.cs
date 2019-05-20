using SetBackground.MusicAPI;

namespace SetBackground.Processors
{
    internal interface IGetSongProvider
    {
        SongResult GetCurrentSong();
    }
}