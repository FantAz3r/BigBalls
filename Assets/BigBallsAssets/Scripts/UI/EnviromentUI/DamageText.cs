using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveUpDistance = 1f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        FaceCamera();

        _canvasGroup.alpha = 1f;
        _text.transform.localPosition = Vector3.zero;

        _text.transform.DOLocalMoveY(moveUpDistance, animationDuration).SetEase(Ease.OutCubic);
        _canvasGroup.DOFade(0, fadeDuration).SetDelay(animationDuration - fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void FaceCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetDamageText(int damage)
    {
        _text.text = damage.ToString();
    }
}
