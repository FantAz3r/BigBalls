using UnityEngine;
using System.Collections;

namespace BigBalls.Services
{
    public interface ICoroutineRunner : IService
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}