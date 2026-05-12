using UnityEngine;
using System.Collections;

public interface ICoroutineRunner: IService
{
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine routine);
    void StopAllCoroutines();
}