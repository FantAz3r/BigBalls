using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
public class EnemyHitedAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _punchStrength = 0.5f;
    [SerializeField] private float _punchDuration = 0.2f;
    [SerializeField] private int _vibrations = 5;
    [SerializeField] private float _elasticity = 0.5f;

    private Tween _activeTween;
    private Vector3 _position;

    private void Awake ()
    {
        _position = transform.localPosition;
    }

    public void OnHit (Ball ball)
    {
        _activeTween?.Kill();

        Vector3 direction = (transform.position - ball.transform.position);
        direction.y = 0;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
        }
        else
        {
            direction = new Vector3(0, 0, Mathf.Sign(direction.z));
        }

        _activeTween = transform.DOPunchPosition(direction * _punchStrength, _punchDuration, _vibrations, _elasticity)
            .OnComplete(() =>
            {
                transform.localPosition = _position;
                _activeTween = null;
            })
            .OnKill(() =>
            {
                transform.localPosition = _position;
                _activeTween = null;
            });
    }
}