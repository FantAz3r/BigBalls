using UnityEngine;
using VContainer;

public class BossEffects : MonoBehaviour
{
    [SerializeField] private EffectViewer _effectViewer;
    [SerializeField] private EnemyAnimator _enemyAnimator;
    private SoundSpawner _soundSpawner;

    [Inject]
    public void Construct(SoundSpawner soundSpawner)
    {
        _soundSpawner = soundSpawner;
    }

    private void OnEnable ()
    {
        if (_enemyAnimator != null)
        {
            _enemyAnimator.LeftSteped += OnLeftSteped;
            _enemyAnimator.RightSteped += OnRightSteped;
        }
    }

    private void OnDisable ()
    {
        if (_enemyAnimator != null)
        {
            _enemyAnimator.LeftSteped -= OnLeftSteped;
            _enemyAnimator.RightSteped -= OnRightSteped;
        }
    }

    private void OnLeftSteped()
    {
        _effectViewer.EnableEffect(EffectType.LeftStep);
        _soundSpawner.Spawn(SoundType.BossStep);
    }

    private void OnRightSteped ()
    {
        _effectViewer.EnableEffect(EffectType.RightStep);
        _soundSpawner.Spawn(SoundType.BossStep);
    }
}
