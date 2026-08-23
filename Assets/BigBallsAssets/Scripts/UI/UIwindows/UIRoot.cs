using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [field: SerializeField] public Transform SafeAreaUIHolder { get; private set; }
    [field: SerializeField] public Transform BackgroundUIHolder { get; private set; }
    [field: SerializeField] public FPSCounter FPSCounter { get; private set; }
    [field: SerializeField] public GlobalWalletView GlobalWalletView { get; private set; }
    
    private void Awake ()
    {
        BackgroundUIHolder.SetAsFirstSibling();
    }
}
