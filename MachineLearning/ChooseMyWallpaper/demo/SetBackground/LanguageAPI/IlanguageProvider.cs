using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.LanguageAPI
{
    interface IlanguageProvider
    {
        string GetLanguage(string text);
        string TranslateTo(string originalText, string originalLanguage, string targetLanguage);
        string TranslateTo(string originalText, string targetLanguage);
        string[] ExtractKeyPhrases(string text, string language);
    }
}
