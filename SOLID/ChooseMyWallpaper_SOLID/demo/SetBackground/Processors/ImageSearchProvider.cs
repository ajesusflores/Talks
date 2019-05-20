using SetBackground.PhotographyAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.Processors
{
    internal class ImageSearchProvider : IImageSearchProvider
    {
        private FlickrAPI flickrApi;
        private PexelAPI pexelApi;

        internal ImageSearchProvider(string flickrKey, string pexelsAPI)
        {
            flickrApi = new FlickrAPI(flickrKey);
            pexelApi = new PexelAPI(pexelsAPI);
        }

        public string GetImageUrlFromText(string keyPhrase)
        {
            string photo = pexelApi.GetImageUrlFromText(keyPhrase);
            if (string.IsNullOrEmpty(photo))
                photo = flickrApi.GetImageUrlFromText(keyPhrase);

            return photo;
        }
    }
}
