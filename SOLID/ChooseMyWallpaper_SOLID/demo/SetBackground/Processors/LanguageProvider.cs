using SetBackground.LanguageAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetBackground.Processors
{
    internal class LanguageProvider
    {
        private MicrosoftTextAnalytics msText;

        internal LanguageProvider(string MSAnalyticsKey)
        {
            msText = new MicrosoftTextAnalytics(MSAnalyticsKey);
        }

        internal string GetKeyPhraseFromLyrics(string lyrics)
        {
            var songLanguage = msText.GetLanguage(lyrics);
            var songKeys = msText.ExtractKeyPhrases(lyrics, songLanguage);
            return GetTextToSearchImage(songKeys);
            
        }

        internal string GetTextToSearchImage(string[] keys)
        {
            if (!keys.Any())
                return string.Empty;

            var result = keys.FirstOrDefault(x => x.Contains(" "));

            return keys[0].Contains(" ") ?
                keys[0] :
                keys[1].Contains(" ") ?
                    keys[1] :
                    string.Format($"{keys[0]} {keys[1]}");
        }
    }
}
