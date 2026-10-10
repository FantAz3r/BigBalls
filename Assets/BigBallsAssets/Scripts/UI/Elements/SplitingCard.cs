using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplitingCard : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _count;

    public void RenderReward (Sprite icon, int amount = 0)
    {
        _image.sprite = icon;

        if (amount >0)
        {
            _count.gameObject.SetActive(true);
            _count.text = amount.ToString();
        }
        else
            _count.gameObject.SetActive(false);
    }
}
