using UnityEngine;

namespace BigBalls.Services
{
    public interface IPoolService
    {
        T GetObject<T>(string name) where T : MonoBehaviour;
        void ReleaseObject<T>(T obj) where T : MonoBehaviour;
    }
}