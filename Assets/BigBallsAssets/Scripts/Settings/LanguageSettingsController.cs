using System.Collections.Generic;
using System.Linq;

public sealed class LanguageSettingsController
{
    public const string AutoLang = "auto";

    private readonly ITranslateService _translateService;
    private readonly ISaveService _saveService;

    public LanguageSettingsController (ITranslateService translateService, ISaveService saveService)
    {
        _translateService = translateService;
        _saveService = saveService;
    }

    public string CurrentLanguage
    {
        get
        {
            var saved = _saveService.GameProgress?.Language;
            return string.IsNullOrEmpty(saved) ? AutoLang : saved;
        }
    }

    public void EnsureDefault ()
    {
        var saved = _saveService.GameProgress?.Language;
        if (!string.IsNullOrEmpty(saved)) return;

        _saveService.GameProgress.Language = AutoLang;
        _saveService.Save();
    }

    public void ApplySavedLanguage ()
    {
        var saved = CurrentLanguage;
        if (saved == AutoLang)
            _translateService.SetDefaulteLanguage();
        else
            _translateService.SwitchLanguage(saved);
    }

    public void Select (string language)
    {
        if (language == AutoLang)
            _translateService.SetDefaulteLanguage();
        else
            _translateService.SwitchLanguage(language);

        Save(language);
    }

    public string ResolveToggleLanguage (IReadOnlyList<string> available)
    {
        var saved = CurrentLanguage;
        var actual = _translateService.CurrentLanguage;

        if (saved == AutoLang && available.Contains(AutoLang))
            return AutoLang;

        if (saved != AutoLang && available.Contains(saved))
            return saved;

        if (!string.IsNullOrEmpty(actual) && available.Contains(actual))
            return actual;

        return available.Contains(AutoLang) ? AutoLang : available.FirstOrDefault();
    }

    private void Save (string language)
    {
        _saveService.GameProgress.Language = language;
        _saveService.Save();
    }
}