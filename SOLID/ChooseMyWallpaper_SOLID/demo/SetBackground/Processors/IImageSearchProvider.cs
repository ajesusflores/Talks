namespace SetBackground.Processors
{
    internal interface IImageSearchProvider
    {
        string GetImageUrlFromText(string keyPhrase);
    }
}