using System;
using YG;

namespace BigBalls.Infrastructure.DI
{
    public class YGTranslateService : ITranslateService
    {
        public string CurrentLanguage => YG2.lang;

        public event Action<string> LanguageChanged;

        public void SwitchLanguage (string language)
        {
            YG2.SwitchLanguage(language);
            LanguageChanged?.Invoke(language);
        }
    }
}