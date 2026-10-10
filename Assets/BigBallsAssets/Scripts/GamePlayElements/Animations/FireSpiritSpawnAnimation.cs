using DG.Tweening;
using UnityEngine;

public class FireSpiritSpawnAnimation : MonoBehaviour, ISpawnAnimation
{
    [Header("Settings")]
    [SerializeField] private float _jumpPower = 3f;
    [SerializeField] private float _jumpDuration = 0.8f;
    [SerializeField] private Ease _jumpEase = Ease.OutQuad;

    public void PlaySpawn (Vector3 spawnPoint, bool wantResetTransform = true)
    {
        if (wantResetTransform)
        {
            transform.position = spawnPoint;
        }

        Vector3 targetPosition = transform.position;

        transform.DOJump(targetPosition, _jumpPower, 1, _jumpDuration)
            .SetEase(_jumpEase);

        transform.DOPunchScale(Vector3.one * 0.5f, _jumpDuration);
    }
}
