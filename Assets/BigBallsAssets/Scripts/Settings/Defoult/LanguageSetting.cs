using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LanguageSetting : MonoBehaviour
{
    private const string AutoLang = "auto";

    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private List<LanguageToggleData> _languageToggles;

    private ITranslateService _translateService;
    private bool _isInitializing = false;
    private ISaveService _saveService;

    [System.Serializable]
    public class LanguageToggleData
    {
        public Toggle Toggle;
        public string Language;
    }

    [Inject]
    public void Construct (ITranslateService translateService, ISaveService saveService)
    {
        _translateService = translateService;
        _saveService = saveService;
        LoadAndApplyLanguage();
    }

    private void Start ()
    {
        foreach (var data in _languageToggles)
        {
            if (data.Toggle != null)
            {
                data.Toggle.onValueChanged.AddListener(isOn => OnToggleChanged(isOn, data.Language));
            }
        }
    }

    private void OnDestroy ()
    {
        foreach (var data in _languageToggles)
        {
            if (data.Toggle != null)
            {
                data.Toggle.onValueChanged.RemoveAllListeners();
            }
        }
    }

    private void OnToggleChanged (bool isOn, string language)
    {
        if (isOn == false || _isInitializing)
            return;

        if (language == AutoLang)
        {
            _translateService.SetDefaulteLanguage();
            return;
        }

        _translateService.SwitchLanguage(language);
    }

    private void LoadAndApplyLanguage ()
    {
        _isInitializing = true;
        string currentLanguage = _saveService.GameProgress.Language;

        foreach (var data in _languageToggles)
        {
            if (data.Toggle != null)
            {
                data.Toggle.isOn = (data.Language == currentLanguage);
            }
        }

        _isInitializing = false;
    }
}
