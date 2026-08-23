using BigBalls.Infrastructure;
using BigBalls.Saves;
using BigBalls.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LevelButtonView : MonoBehaviour
{
    [SerializeField] private Image _lockImage;
    [SerializeField] private List<Image> _stars;
    [SerializeField] private LevelButton _levelButton;

    private ISaveService _saveService;

    private void Awake()
    {
        if (_levelButton.LevelID == LevelID.Level1)
            _lockImage.gameObject.SetActive(false);
    }

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;

        var levels = _saveService.GameProgress.Levels;

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].LevelID == (int)_levelButton.LevelID)
            {
                if (i > 0)
                {
                    LevelSaveData previousLevel = levels[i - 1];
                    Button button = _levelButton.GetComponent<Button>();
                    Debug.Log(1234);
                    if (previousLevel != null && previousLevel.IsComplite)
                    {
                        _lockImage.gameObject.SetActive(true);
                        button.interactable = false;
                    }
                    else
                    {
                        _lockImage.gameObject.SetActive(false);
                        button.interactable = true;
                    }
                }
                else
                {
                    _lockImage.gameObject.SetActive(false);
                }

                RenderStars(levels[i].CompliteWaves);
            }
        }
    }

    private void RenderStars(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _stars[i].gameObject.SetActive(true);
        }
    }
}
