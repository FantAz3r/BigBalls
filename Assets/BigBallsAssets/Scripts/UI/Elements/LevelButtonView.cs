using System.Collections.Generic;
using BigBalls.Infrastructure;
using BigBalls.Saves;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[RequireComponent(typeof(Button))]
public class LevelButtonView : MonoBehaviour
{
    [SerializeField] private Image _lockImage;
    [SerializeField] private List<Image> _stars;
    [SerializeField] private LevelButton _levelButton;

    private Button _button;
    private ISaveService _saveService;

    [Inject]
    public void Construct (ISaveService saveService)
    {
        _button = _levelButton.GetComponent<Button>();
        _saveService = saveService;

        var levels = _saveService.GameProgress.Levels;
        int myId = (int) _levelButton.LevelID;

        LevelSaveData currentLevel = levels.Find(l => l.LevelID == myId);

        if (myId == (int) LevelID.Level1)
        {
            SetUnlocked(true);
            RenderStars(currentLevel?.CompliteWaves ?? 0);
            return;
        }

        LevelSaveData previousLevel = levels.Find(l => l.LevelID == myId - 1);
        bool unlocked = previousLevel != null && previousLevel.IsComplite;
        SetUnlocked(unlocked);

        RenderStars(currentLevel?.CompliteWaves ?? 0);
    }

    private void SetUnlocked (bool unlocked)
    {
        _button.interactable = unlocked;
        _lockImage.gameObject.SetActive(!unlocked);
    }

    private void RenderStars (int count)
    {
        for (int i = 0; i < count; i++)
        {
            _stars[i].gameObject.SetActive(true);
        }
    }
}
