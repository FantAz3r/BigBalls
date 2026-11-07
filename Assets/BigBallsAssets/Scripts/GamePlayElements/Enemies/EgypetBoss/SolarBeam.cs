using BigBalls.GameplayObjects;
using UnityEngine;

public class SolarBeam : MonoBehaviour
{
    [SerializeField] private MeshRenderer _telegraphVisual; // показывается 0.5с
    [SerializeField] private GameObject _impactVisual;      // сам луч

    private float _windup;
    private float _active;
    private float _damage;
    private LayerMask _mask;
    private bool _baked;
    private bool _fired;

    public void Init (float windup, float active, float damage, LayerMask mask)
    {
        _windup = windup; _active = active; _damage = damage; _mask = mask;

        if (_telegraphVisual) _telegraphVisual.enabled = true;
        if (_impactVisual) _impactVisual.SetActive(false);
    }

    private void Update ()
    {
        if (!_baked)
        {
            _windup -= Time.deltaTime;
            if (_windup <= 0f)
            {
                _baked = true;
                if (_telegraphVisual) _telegraphVisual.enabled = false;
                if (_impactVisual) _impactVisual.SetActive(true);
                FireOnce();
            }
        }
        else if (!_fired)
        {
            _active -= Time.deltaTime;
            if (_active <= 0f)
            {
                _fired = true;
                Destroy(gameObject);
            }
        }
    }

    private void FireOnce ()
    {
        // Урон один раз за «удар», либо каждый кадр — по вкусу.
        // Пример: OverlapBox по вертикальной колонне сверху вниз.
        var hits = Physics.OverlapBox(transform.position, transform.localScale * 0.5f, Quaternion.identity, _mask);
        for (int i = 0; i < hits.Length; i++)
            if (hits[i].TryGetComponent(out IDamageable dmg))
                dmg.TakeDamage(_damage);
    }
}