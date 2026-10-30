using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class BoomerangAxe : MonoBehaviour
{
    [SerializeField] private Vector3 _flatLocalEuler;
    [SerializeField] private ParticleObject _particleObject;

    private Transform _homeHand;
    private float _flightTime;
    private float _returnTime;
    private float _damage;
    private float _radius;
    private Vector3 _targetPoint;
    private bool _clockwise;
    private bool _isFlying;
    private bool _hasHitPlayer;
    private float _t;
    private float _spinSpeed = 540f;
    private IDamageService _damageService;
    private Transform _parent;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Quaternion _flightBaseRotation;

    [Inject]
    public void Construct (IDamageService damageService)
    {
        _damageService = damageService;
    }

    public void Setup (Transform hand, float flight, float ret, float dmg, float radius)
    {
        _parent = transform.parent;
        _homeHand = hand;
        _flightTime = flight;
        _returnTime = ret;
        _damage = dmg;
        _radius = radius;
    }

    public void Launch (Vector3 target, bool clockwise)
    {
        _targetPoint = target;
        _clockwise = clockwise;
        _t = 0f;
        _isFlying = true;
        _hasHitPlayer = false;
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _particleObject.gameObject.SetActive(true);

        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;

        _flightBaseRotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(_flatLocalEuler);

        transform.SetParent(null, worldPositionStays: true);
        transform.localScale = Vector3.one;
    }

    private void Update ()
    {
        if (!_isFlying) return;

        if (_homeHand == null)
        {
            ReturnToStart();
            return;
        }

        _t += Time.deltaTime;
        float speed = _clockwise ? _spinSpeed : -_spinSpeed;
        transform.rotation = Quaternion.AngleAxis(speed * _t, Vector3.up) * _flightBaseRotation;

        if (_t <= _flightTime)
        {
            float k = _t / _flightTime;
            Vector3 start = _homeHand.position;
            transform.position = CalculateArcPosition(start, _targetPoint, k, 1f);
        }
        else if (_t <= _flightTime + _returnTime)
        {
            float k = (_t - _flightTime) / _returnTime;
            Vector3 from = _targetPoint;
            Vector3 to = _homeHand.position;
            transform.position = CalculateArcPosition(from, to, k, -1f);
        }
        else
        {
            ReturnToStart();
        }
    }

    private void ReturnToStart ()
    {
        _isFlying = false;
        transform.SetParent(_parent, worldPositionStays: false);
        transform.localRotation = _startRotation;
        transform.localPosition = _startPosition;
        _particleObject.gameObject.SetActive(false);
    }

    private Vector3 CalculateArcPosition (Vector3 from, Vector3 to, float k, float sideSign)
    {
        Vector3 forward = to - from;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = transform.forward;

        Vector3 sideDir = Vector3.Cross(Vector3.up, forward.normalized)
                        * (_clockwise ? sideSign : -sideSign);

        float arcFactor = Mathf.Sin(Mathf.PI * k);

        return Vector3.Lerp(from, to, k) + sideDir * (_radius * arcFactor);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (!_isFlying || _hasHitPlayer)
            return;

        if (other.TryGetComponent(out Player player))
        {
            _hasHitPlayer = true;
            _damageService.ApplyDamage(player, _damage);
        }
    }

    private void OnDisable ()
    {
        if (_isFlying && _parent != null)
        {
            transform.SetParent(_parent, worldPositionStays: false);
            transform.localRotation = _startRotation;
            transform.localPosition = _startPosition;
        }
    }
}