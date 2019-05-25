using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.PhotographyAPI
{
    interface IPhotographyProvider
    {
        string GetImageUrlFromText(string textToSearch);
    }
}
