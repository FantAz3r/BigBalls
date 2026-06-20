using System;
using YG;

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
