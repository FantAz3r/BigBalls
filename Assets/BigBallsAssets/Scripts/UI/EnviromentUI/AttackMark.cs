using UnityEngine;

public class AttackMark : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake ()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetLength(float value)
    {
    }
}
