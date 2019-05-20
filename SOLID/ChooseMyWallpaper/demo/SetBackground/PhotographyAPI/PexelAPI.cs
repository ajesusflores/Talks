using PexelsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.PhotographyAPI
{
    public class PexelAPI : IPhotographyProvider
    {
        PexelsClient _client;
        
        public PexelAPI(string apiKey)
        {
            _client = new PexelsClient(apiKey);
        }
        public string GetImageUrlFromText(string textToSearch)
        {
            var results = _client.SearchAsync(textToSearch, 1, 1).Result;

            return results.Photos.Any() ? results.Photos[0].Src.Original : "";
        }
    }
}
