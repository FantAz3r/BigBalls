using System;

public interface ITranslateService
{
    string CurrentLanguage { get;}
    event Action<string> LanguageChanged;
    void SwitchLanguage(string language);
}