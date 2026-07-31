using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float _startZ = 30f;
    [SerializeField] private float _endZ = -10f;
    [SerializeField] private float _duration = 5f;

    private void Start ()
    {
        AnimateBackground();
    }

    private void AnimateBackground ()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _startZ);

        transform.DOMoveZ(_endZ, _duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy ()
    {
        transform.DOKill();
    }
}
