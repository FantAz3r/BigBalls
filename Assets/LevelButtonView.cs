using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LevelButtonView : MonoBehaviour
{
    [SerializeField] private Image _lockImage;
    [SerializeField] private List<Image> _stars;

    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;

    }

    private void RenderStars(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _stars[i].gameObject.SetActive(true);
        }
    }
}
