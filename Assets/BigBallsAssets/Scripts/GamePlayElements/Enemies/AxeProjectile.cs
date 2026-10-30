using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class AxeProjectile : MonoBehaviour
{
    private Vector3 _dir;
    private float _speed, _damage, _life = 4f;
    private IDamageService _damageService;
    private IPlayerProvider _playerProvider;
    private IPoolService _poolService;

    [Inject]
    public void Construct (IDamageService damageService, IPlayerProvider playerProvider, IPoolService poolService)
    {
        _damageService = damageService;
        _playerProvider = playerProvider;
        _poolService = poolService;
    }

    public void Launch (Vector3 pos, Vector3 dir, float speed, float damage)
    {
        transform.position = pos;
        _dir = dir.normalized;
        _speed = speed;
        _damage = damage;
        _life = 4f;
        gameObject.SetActive(true);
    }

    private void Update ()
    {
        transform.position += _dir * (_speed * Time.deltaTime);
        //transform.Rotate(Vector3.right, 180 * Time.deltaTime, Space.World);

        _life -= Time.deltaTime;

        if (_life <= 0f)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            _damageService.ApplyDamage(player, _damage);
            _poolService.Release(this);
        }
        else if (other.gameObject.TryGetComponent<Wall>(out _))
        {
            _poolService.Release(this);
        }
    }
}
