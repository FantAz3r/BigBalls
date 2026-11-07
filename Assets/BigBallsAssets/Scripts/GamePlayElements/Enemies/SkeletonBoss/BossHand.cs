using System;
using System.Collections;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    public void Launch (Vector3 from, Vector3 to, float travelTime, float returnTime, Action onFinished)
    {
        StartCoroutine(MoveRoutine(from, to, travelTime, returnTime, onFinished));
    }

    private IEnumerator MoveRoutine (Vector3 from, Vector3 to, float travelTime, float returnTime, Action onFinished)
    {
        yield return MoveTo(from, to, travelTime);
        yield return MoveTo(to, from, returnTime);
        onFinished?.Invoke();
    }

    private IEnumerator MoveTo (Vector3 a, Vector3 b, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(a, b, t / time);
            yield return null;
        }
        transform.position = b;
    }

    public void Despawn () => Destroy(gameObject);
}