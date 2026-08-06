using System;
using BigBalls.Factories;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class PlayerSpawnAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _spawnPosition = new Vector3(0, 0, -6);
    [SerializeField] private float _spawnDuration = 1.0f;
    [SerializeField] private float _offsetBelow = 15f;
    [SerializeField] private Ease _spawnEase = Ease.OutQuad;

    private Action _action;
    private IPlayerFactory _playerFactory;


    [Inject]
    public void Construct(IPlayerFactory playerFactory)
    {
        _playerFactory = playerFactory;
        _action = () => _playerFactory.OnPlayerSpawned();
        StartSpawnAnimation(_action);
    }

    private void StartSpawnAnimation (Action action )
    {
        Vector3 startPosition = new Vector3(_spawnPosition.x, _spawnPosition.y, _spawnPosition.z - _offsetBelow);
        transform.position = startPosition;

        transform.DOMove(_spawnPosition, _spawnDuration)
            .SetEase(_spawnEase)
            .OnComplete(() =>
            {
                action?.Invoke();
            });
    }
}
