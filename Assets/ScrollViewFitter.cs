using UnityEngine;

public class ScrollViewFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _rectTransform.sizeDelta = new Vector2(4000f, 4000f);
    }

    private void OnEnable()
    {
        _rectTransform.sizeDelta = new Vector2(4000f, 4000f);
    }
}
