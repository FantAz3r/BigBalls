using TMPro;
using UnityEngine;
using VContainer;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private bool _showFPS = true;
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private TMP_Text _textFPS;

    private float _deltaTime = 0f;
    private float _fps = 0f;
    private float _updateInterval = 0.5f;
    private float _timeSinceLastUpdate = 0f;

    [Inject]
    public void Construct(ISaveService saveService) => ShowFPS(saveService.GameProgress.ShowFPS);

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        _timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (_timeSinceLastUpdate >= _updateInterval)
        {
            _fps = 1f / _deltaTime;
            _timeSinceLastUpdate = 0f;
        }
    }

    private void OnGUI()
    {
        if (_showFPS == false)
            return;

        _textFPS.color = GetColorByFPS(_fps);
        _textFPS.text = $"FPS: {Mathf.Round(_fps)}";
    }

    public void ShowFPS(bool isOn)
    {
        _showFPS = isOn;
        gameObject.SetActive(isOn);
    }

    private Color GetColorByFPS(float fps)
    {
        if (fps >= 55) return Color.green;
        if (fps >= 30) return Color.yellow;
        return Color.red;
    }
}