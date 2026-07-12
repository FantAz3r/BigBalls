using UnityEngine;

namespace BigBalls.UI
{
    public class WindowBase : MonoBehaviour, IWindow
    {
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

