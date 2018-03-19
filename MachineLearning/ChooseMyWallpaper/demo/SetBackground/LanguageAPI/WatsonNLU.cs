using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.LanguageAPI
{
    public class WatsonNLU : IlanguageProvider
    {
        public string[] ExtractKeyPhrases(string text, string language)
        {
            throw new NotImplementedException();
        }

        public string GetLanguage(string text)
        {
            throw new NotImplementedException();
        }

        public string TranslateTo(string originalText, string originalLanguage, string targetLanguage)
        {
            throw new NotImplementedException();
        }

        public string TranslateTo(string originalText, string targetLanguage)
        {
            throw new NotImplementedException();
        }
    }
}
