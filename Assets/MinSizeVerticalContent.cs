using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ContentSizeFitter))]
public class MinSizeVerticalContent : MonoBehaviour
{
    [Tooltip("ћинимальна€ высота контента. ≈сли объектов мало, Content не сожметс€ меньше этого значени€.")]
    [SerializeField] private float _minHeight = 500f;

    private RectTransform _rectTransform;
    private ContentSizeFitter _fitter;

    private void Awake ()
    {
        _rectTransform = GetComponent<RectTransform>();
        _fitter = GetComponent<ContentSizeFitter>();

        // ѕринудительно настраиваем ContentSizeFitter
        // √оризонтальный размер не трогаем (Unconstrained), 
        // а вертикальный ставим PreferredSize, чтобы он т€нулс€ под детей.
        _fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        _fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void OnEnable ()
    {
        Recalculate();
    }

    public void Recalculate ()
    {
        // 1. «аставл€ем Unity пересчитать предпочтительную высоту исход€ из детей (LayoutGroup)
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

        // 2. ѕолучаем ту высоту, котора€ получилась бы у Content без ограничений
        float preferredHeight = LayoutUtility.GetPreferredHeight(_rectTransform);

        // 3. ≈сли предпочтительна€ высота меньше нашей минимальной, 
        // принудительно раст€гиваем Content до минимальной.
        if (preferredHeight < _minHeight)
        {
            // ѕровер€ем, чтобы не спамить изменени€ каждый кадр (оптимизаци€)
            if (!Mathf.Approximately(_rectTransform.sizeDelta.y, _minHeight))
            {
                _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _minHeight);
            }
        }
        // ≈сли детей много и высота больше минимальной, ContentSizeFitter 
        // сам отлично справл€етс€, и мы ничего не делаем.
    }
}