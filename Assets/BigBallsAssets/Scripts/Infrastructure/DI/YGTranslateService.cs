using System;
using YG;

namespace BigBalls.Infrastructure.DI
{
    public class YGTranslateService : ITranslateService
    {
        public string CurrentLanguage => YG2.lang;

        public event Action<string> LanguageChanged;

        public void SetDefaulteLanguage ()
        {
            YG2.SwitchLanguage(YG2.envir.language);
            LanguageChanged?.Invoke(YG2.envir.language);
        }

        public void SwitchLanguage (string language)
        {
            YG2.SwitchLanguage(language);
            LanguageChanged?.Invoke(language);
        }
    }
}