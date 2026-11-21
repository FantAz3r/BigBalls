using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class HighliterUI : MonoBehaviour
{
    private RectTransform _selfRectTransform;
    private Canvas _highlightCanvas;
    private GraphicRaycaster _highlightRaycaster;

    private void Awake ()
    {
        _selfRectTransform = GetComponent<RectTransform>();
        _highlightCanvas = GetComponent<Canvas>();
        _highlightRaycaster = GetComponent<GraphicRaycaster>();
        Hide();
    }

    public void Show (int sortingIndex)
    {
        _highlightCanvas.overrideSorting = true;
        _highlightCanvas.sortingOrder = sortingIndex;
        _highlightCanvas.vertexColorAlwaysGammaSpace = true;

        _highlightRaycaster.enabled = true;
        RebuildTexts();
    }

    public void Hide ()
    {
        _highlightRaycaster.enabled = false;
        _highlightCanvas.overrideSorting = false;
    }

    public void Destroy ()
    {
        if (_highlightRaycaster != null)
        {
            Destroy(_highlightRaycaster);
            _highlightRaycaster = null;
        }

        if (_highlightCanvas != null)
        {
            Destroy(_highlightCanvas);
            _highlightCanvas = null;
        }

        RebuildTexts();
    }

    private void RebuildTexts()
    {
        TMP_Text[] texts = _selfRectTransform.GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            text.ForceMeshUpdate();
        }
    }
}


