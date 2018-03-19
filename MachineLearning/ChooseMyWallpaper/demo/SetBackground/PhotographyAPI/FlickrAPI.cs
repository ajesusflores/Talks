using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.PhotographyAPI
{
    public class FlickrAPI : IPhotographyProvider
    {
        Flickr _flickrAPI;
        public FlickrAPI(string apiKey)
        {
            _flickrAPI = new Flickr(apiKey);
        }

        public string GetImageUrlFromText(string textToSearch)
        {
            var options = new PhotoSearchOptions
            {
                PerPage = 3,
                Text = textToSearch,
                SortOrder = PhotoSearchSortOrder.InterestingnessDescending,
                MediaType = MediaType.Photos,
                SafeSearch = SafetyLevel.Moderate,
                Extras =  PhotoSearchExtras.LargeUrl |  PhotoSearchExtras.CountFaves,
                
            };

            var options2 = new PhotoSearchOptions
            {
                PerPage = 3,
                SortOrder = PhotoSearchSortOrder.Relevance,
                Tags = textToSearch.Split(' ')[0],
                MediaType = MediaType.Photos,
                SafeSearch = SafetyLevel.Moderate,
                Extras = PhotoSearchExtras.LargeUrl | PhotoSearchExtras.CountFaves,

            };

            var photos = _flickrAPI.PhotosSearch(options)
                            .Concat(_flickrAPI.PhotosSearch(options2));

            var orderedPhotos = photos.Where(x => x.DoesLargeExist);
            if (!orderedPhotos.Any())
                orderedPhotos = photos;

            if (!photos.Any())
                return "";

            var selected = orderedPhotos.ElementAt((new Random()).Next(0, orderedPhotos.Count() - 1));

            return selected.DoesLargeExist ?
                        selected.LargeUrl :
                        selected.DoesMediumExist ?
                            selected.MediumUrl :
                            selected.SmallUrl;
        }
    }
}
