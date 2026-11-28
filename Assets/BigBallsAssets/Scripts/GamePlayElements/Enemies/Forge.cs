using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Forge : Enemy
{
    [SerializeField] private BallConfig _config;

    private ICoroutineRunner _coroutineRunner;
    private IPlayerProvider _playerProvider;
    private IEnemyFactory _enemyFactory;
    private IBallFactory _ballFactory;
    private IIdentifierService _identifierService;
    private bool _canSpawn = true;
    private WaitForSeconds _spawnDelay;

    [Inject]
    public void Construct (
        ICoroutineRunner coroutineRunner,
        IEnemyFactory enemyFactory,
        IPlayerProvider playerProvider,
        IBallFactory ballFactory,
        IIdentifierService identifierService)
    {
        _coroutineRunner = coroutineRunner;
        _enemyFactory = enemyFactory;
        _playerProvider = playerProvider;
        _ballFactory = ballFactory;
        _identifierService = identifierService;
    }

    protected override void ChildInit ()
    {

        _coroutineRunner.StartCoroutine(SpawnRoutine());
        StatHolder.TryGetStat(out Stat spawnSpeed, StatType.AttackSpeed);
        OnStatChanged(spawnSpeed);
    }

    private void OnStatChanged (IReadonlyStat stat)
    {
        _spawnDelay = new WaitForSeconds((1 / stat.CurrentValue));
    }

    private IEnumerator SpawnRoutine ()
    {
        while (_canSpawn)
        {
            yield return _spawnDelay;

            BallModel spirit = new BallModel(_identifierService.ID, _config);
            Ball ball = _ballFactory.Create(spirit, EntityType.Enemy, FirePoint, GetStrategies(), Quaternion.identity);
            ball.Subscribe();
            ball.EventHandler.Spawn(ball);

            Player player = _playerProvider.Player;
            ball.Mover.SetDirection(new Vector2(player.transform.position.x, player.transform.position.z));
            ball.transform.rotation = Quaternion.LookRotation(player.transform.position, Vector3.up);
        }
    }

    private List<ICollisionStrategy> GetStrategies ()
    {
        return new List<ICollisionStrategy>
        {
            new ReflectCollisionStrategy(true),
            new HitPlayerStrategy(),
        };
    }
}
