using System.Text;
using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private Color _color = Color.white;

    private readonly IPoolService _poolService;
    private readonly ISaveService _saveService;
    private readonly ICameraProvider _cameraProvider;

    private DamageText _prefab;
    private Vector3 _offset = new Vector3(0, 0.6f, 0);

    public DamageTextFactory (IPoolService poolService, ISaveService saveService, ICameraProvider cameraProvider, IResourceLoader resourceLoader)
    {
        _poolService = poolService;
        _saveService = saveService;
        _cameraProvider = cameraProvider;
        _prefab = resourceLoader.Load<DamageText>();
    }

    public DamageText Create (Vector3 position, float damage)
        => Create(position, damage, null, null, default);

    public DamageText Create (Vector3 position, float damage, Color color)
        => Create(position, damage, null, null, color);

    public DamageText Create (Vector3 position, float damage, string prefix)
        => Create(position, damage, prefix, null, default);

    public DamageText Create (Vector3 position, float damage, string prefix, Color color)
        => Create(position, damage, prefix, null, color);

    public DamageText Create (Vector3 position, float damage, char prefix)
        => Create(position, damage, prefix.ToString(), null, default);

    public DamageText Create (Vector3 position, float damage, char prefix, Color color)
        => Create(position, damage, prefix.ToString(), null, color);

    public DamageText Create (Vector3 position, float damage, string prefix, string postfix, Color color)
    {
        if (!_saveService.GameProgress.ShowDamageNumbers)
            return null;

        DamageText damageText = _poolService.Get(_prefab, position + _offset);
        damageText.Init(_cameraProvider);

        Color finalColor = color == default ? _color : color;

        string text = BuildText(prefix, (int) damage, postfix);

        damageText.SetDamageText(text, finalColor);

        damageText.Removed += OnRemoved;
        return damageText;
    }

    private static string BuildText (string prefix, int damage, string postfix)
    {
        bool hasPrefix = !string.IsNullOrEmpty(prefix);
        bool hasPostfix = !string.IsNullOrEmpty(postfix);

        if (!hasPrefix && !hasPostfix)
            return damage.ToString();

        int capacity = damage.ToString().Length
                       + (hasPrefix ? prefix.Length : 0)
                       + (hasPostfix ? postfix.Length : 0);

        var sb = new StringBuilder(capacity);

        if (hasPrefix)
            sb.Append(prefix);

        sb.Append(damage);

        if (hasPostfix)
            sb.Append(postfix);

        return sb.ToString();
    }

    public void OnRemoved (DamageText damageText)
    {
        _poolService.Release(damageText);
        damageText.Removed -= OnRemoved;
    }
}