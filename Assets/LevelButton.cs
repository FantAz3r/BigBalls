using UnityEngine;
using VContainer;

public class LevelButton : ButtonClickHandler
{
    [SerializeField] private LevelID _levelID;
    private ILevelLoadingService _loadingService; 

    [Inject]
    public void Construct(ILevelLoadingService levelLoadingService)
    {
        _loadingService = levelLoadingService;
    }

    protected override void OnClick() => _loadingService.Load(_levelID);
    
}
