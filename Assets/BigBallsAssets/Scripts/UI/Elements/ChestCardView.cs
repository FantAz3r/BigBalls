using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestCardView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private TMP_Text _rarityText;
    [SerializeField] private Image _rarityBackground;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;

    [Header("Rarity Colors")]
    [SerializeField] private Color _commonColor = Color.gray;
    [SerializeField] private Color _rareColor = Color.blue;
    [SerializeField] private Color _epicColor = Color.magenta;
    [SerializeField] private Color _legendaryColor = Color.yellow;
    [SerializeField] private Color _goldColor = Color.yellow;

    public RectTransform RectTransform => _rectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;

    public void RenderCard (ICardModel cardModel) => RenderCard(cardModel, 1);
    public void RenderCard (ICardModel cardModel, int count)
    {
        gameObject.SetActive(true);

        if (cardModel == null)
        {
            Debug.LogError("CardModel is null");
            return;
        }

        _iconImage.sprite = cardModel.Config.Icon;
        _amountText.text = $"x{count}";
        _rarityText.text = cardModel.Config.Rarity.ToString();

        SetRarityColor(cardModel.Config.Rarity);

        _canvasGroup.alpha = 0f;
        _rectTransform.localScale = Vector3.zero;
    }

    public void RenderGold (Sprite sprite,  int amount)
    {
        gameObject.SetActive(true);

        _iconImage.sprite = sprite;
        _amountText.text = $"+{amount}";
        _rarityText.text = "Gold";

        SetRarityColor(Rarity.Common, true);

        _canvasGroup.alpha = 0f;
    }

    private void SetRarityColor (Rarity rarity, bool isGold = false)
    {
        Color color = rarity switch
        {
            Rarity.Common => _commonColor,
            Rarity.Rare => _rareColor,
            Rarity.Epic => _epicColor,
            Rarity.Legendary => _legendaryColor,
            _ => _commonColor
        };

        if (isGold)
        {
            color = _goldColor;
        }

        if (_rarityBackground != null)
        {
            _rarityBackground.color = color;
        }
    }

    public Sequence CreateAppearAnimation (float delay = 0f)
    {
        Sequence sequence = DOTween.Sequence();

        if (delay > 0f)
        {
            sequence.AppendInterval(delay);
        }

        sequence.Append(_canvasGroup.DOFade(1f, 0.3f));
        sequence.Join(_rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        sequence.SetUpdate(true);
        return sequence;
    }

    public Sequence CreateCollectAnimation (Transform targetTransform, float duration = 0.5f)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_rectTransform.DOMove(targetTransform.position, duration).SetEase(Ease.InBack));
        sequence.Join(_rectTransform.DOScale(0.5f, duration));
        sequence.Join(_canvasGroup.DOFade(0f, duration * 0.8f));
        sequence.SetUpdate(true);
        return sequence;
    }

    public Sequence CreateUpgradeAnimation (ICardModel cardModel)
    {
        Sequence sequence = DOTween.Sequence();

        // Пульсация при получении карты
        sequence.Append(_rectTransform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad));
        sequence.Append(_rectTransform.DOScale(1f, 0.2f).SetEase(Ease.InQuad));
        sequence.SetUpdate(true);
        // Обновление информации
        sequence.AppendCallback(() => RenderCard(cardModel));

        return sequence;
    }
}
