using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer.Unity;

public class DamageTextFactory
{
    private readonly IResourceLoader _resourceLoader;
    private readonly IObjectResolverProvider _objectResolverProvider;
    private DamageText _prefab;

    public DamageTextFactory(IResourceLoader resourceLoader, IObjectResolverProvider objectResolverProvider)
    {
        _resourceLoader = resourceLoader;
        _objectResolverProvider = objectResolverProvider;
        _prefab = _resourceLoader.Load<DamageText>();
    }

    public DamageText Create(Vector3 position, float damage)
    {
        DamageText damageText = _objectResolverProvider.CurrentResolver.Instantiate(_prefab, position, Quaternion.identity);
        damageText.SetDamageText((int)damage);
        return damageText;
    }
}
