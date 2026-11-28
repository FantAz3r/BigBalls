using UnityEngine;
using UnityEngine.UI;


namespace BigBalls.UI
{
    [RequireComponent(typeof(Button))]

    public class ButtonClickHandler : MonoBehaviour
    {
        public Button Button { get; private set; }

        protected virtual void Awake ()
        {
            Button = GetComponent<Button>();
            Button?.onClick.AddListener(OnClick);
        }

        private void OnDestroy ()
        {
            Button?.onClick.RemoveListener(OnClick);
        }

        protected virtual void OnClick () { }
    }
}