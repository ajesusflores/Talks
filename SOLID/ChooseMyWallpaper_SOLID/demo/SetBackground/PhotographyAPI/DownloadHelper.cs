using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.PhotographyAPI
{
    public static class DownloadHelper
    {
        public static string DownloadImageFromUrl(this string imageUrl, string destinationFile)
        {
            var extension = Path.GetExtension(imageUrl);
            var fileName = $"{destinationFile}{extension}";
            if(!string.IsNullOrEmpty(imageUrl))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imageUrl, fileName);
                }
            }
            return fileName;
        }
    }
}
