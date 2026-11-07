using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

public class LanguageSettingView : MonoBehaviour
{
    [SerializeField] private List<LanguageToggleData> _languageToggles;

    private LanguageSettingsController _controller;
    private readonly Dictionary<Toggle, UnityAction<bool>> _handlers = new();

    [Inject]
    public void Construct (LanguageSettingsController controller) => _controller = controller;

    private void OnEnable ()
    {
        _controller.ApplySavedLanguage();
        SyncFromModel();

        foreach (var data in _languageToggles)
        {
            if (data.Toggle == null)
                continue;

            UnityAction<bool> handler = isOn => OnToggleChanged(isOn, data.Language);
            _handlers[data.Toggle] = handler;
            data.Toggle.onValueChanged.AddListener(handler);
        }
    }

    private void OnDisable ()
    {
        foreach (var kv in _handlers)
            kv.Key.onValueChanged.RemoveListener(kv.Value);
        _handlers.Clear();
    }

    private void SyncFromModel ()
    {
        var available = _languageToggles
            .Where(data => data.Toggle != null)
            .Select(data => data.Language)
            .ToList();

        var target = _controller.ResolveToggleLanguage(available);

        foreach (var data in _languageToggles)
            if (data.Toggle != null)
                data.Toggle.SetIsOnWithoutNotify(data.Language == target);
    }

    private void OnToggleChanged (bool isOn, string language)
    {
        if (isOn == false)
            return;

        _controller.Select(language);
    }

    [System.Serializable]
    public class LanguageToggleData
    {
        public Toggle Toggle;
        public string Language;
    }
}