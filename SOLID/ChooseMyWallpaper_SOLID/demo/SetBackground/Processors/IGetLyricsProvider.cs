namespace SetBackground.Processors
{
    internal interface IGetLyricsProvider
    {
        string GetLyrics(string songName, string artistName);
    }
}