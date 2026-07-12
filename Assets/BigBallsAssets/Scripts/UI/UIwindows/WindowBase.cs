using UnityEngine;

namespace BigBalls.UI
{
    public class WindowBase : MonoBehaviour, IWindow
    {
        public virtual void Open()
        {
            Debug.Log(gameObject.name);
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            Debug.Log(gameObject.name);
            gameObject.SetActive(false);
        }
    }
}

