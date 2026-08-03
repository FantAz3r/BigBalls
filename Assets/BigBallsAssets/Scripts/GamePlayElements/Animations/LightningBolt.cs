using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningBolt : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _jumpTime = 0.2f;
    [SerializeField] private float _endDelay = 0.2f;
    [SerializeField] private Vector3 _offset;

    private Coroutine _routine;
    private WaitForSeconds _delay;
    private WaitForSeconds _wait;

    public event Action<LightningBolt> LightFinished;

    private void Awake ()
    {
        _delay = new WaitForSeconds(_jumpTime);
        _wait = new WaitForSeconds(_endDelay);
        _line.positionCount = 0;
    }

    public void StartLightning (List<Transform> targets, int jumps)
    {
        if (targets == null || targets.Count == 0)
            return;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(AddPoints(targets, jumps));
    }

    private IEnumerator AddPoints (List<Transform> targets, int jumps)
    {
        _line.positionCount = 0;

        int count = Mathf.Min(targets.Count, jumps + 1);

        for (int i = 0; i < count; i++)
        {
            if (targets[i] == null)
                continue;

            _line.positionCount++;
            _line.SetPosition(_line.positionCount - 1, targets[i].position + _offset);

            if (i > 1)
            {
                yield return _delay;
            }
        }

        yield return _wait;
        _line.positionCount = 0;
        _routine = null;
        LightFinished?.Invoke(this);
    }

    private void OnDisable ()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _line.positionCount = 0;
    }
}
