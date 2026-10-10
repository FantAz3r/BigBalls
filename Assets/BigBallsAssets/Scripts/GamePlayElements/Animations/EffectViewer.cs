using AYellowpaper.SerializedCollections;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;

public class EffectViewer : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<EffectType, ParticleObject> _effects = new SerializedDictionary<EffectType, ParticleObject>();

    private IObjectResolver _resolver;

    [Inject]
    public void Construct(IObjectResolverProvider objectResolverProvider)
    {
        _resolver = objectResolverProvider.CurrentResolver;

        foreach (var effect in _effects)
        {
            _resolver.Inject(effect.Value);
        }
    }

    public void EnableEffect(EffectType type, float duration = 0) => _effects[type].Play(duration);
}
