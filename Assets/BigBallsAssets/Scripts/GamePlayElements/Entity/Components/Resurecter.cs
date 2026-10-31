using System.Collections;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;

public class Resurecter
{
    private const int ImmuneTime = 2;
    private readonly IWindowService _windowService;

    private IPlayerProvider _playerProvider;
    private IEntityRepository _entityRepository;
    private IPlayerFactory _playerFactory;
    private IUIFactory _uIFactory;
    private ILouseService _louseService;
    private ICoroutineRunner _coroutineRunner;
    private ITimeService _timeService;
    private WaitForSeconds _wait = new WaitForSeconds(ImmuneTime);

    public Resurecter (IPlayerProvider playerProvider,
        IEntityRepository entityRepository,
        IPlayerFactory playerFactory,
        IUIFactory uIFactory,
        ILouseService louseService,
        ICoroutineRunner coroutineRunner,
        ITimeService timeService,
        IWindowService windowService)
    {
        _playerProvider = playerProvider;
        _entityRepository = entityRepository;
        _playerFactory = playerFactory;
        _louseService = louseService;
        _coroutineRunner = coroutineRunner;
        _uIFactory = uIFactory;
        _timeService = timeService;
        _windowService = windowService;
    }

    private Player Player => _playerProvider.Player;

    public void Resurect ()
    {
        StatHolder statHolder = _entityRepository.GetStatHolder(Player);

        if (statHolder.TryGetStat(out Stat health, StatType.Health))
        {
            health.AddCurrentValue(health.MaxValue / 2);
            health.SetImmune(true);
            _coroutineRunner.StartCoroutine(WaitDisableImmune(health));
        }

        Player.DeathHandler.Subscribe();
        Player.gameObject.SetActive(true);
        _louseService.SetLouseReason(Player.DeathHandler);
        _playerFactory.DeathSubscribe();
        _windowService.Open<HUD>();
        _timeService.SmoothEditTimescale(1, 0.2f);
    }

    private IEnumerator WaitDisableImmune (Stat health)
    {
        yield return _wait;
        health.SetImmune(false);
    }
}
