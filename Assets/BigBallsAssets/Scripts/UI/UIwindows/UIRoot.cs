using UnityEngine;

public class UIRoot : MonoBehaviour
{
    [field: SerializeField] public Transform SafeAreaUIHolder { get; private set; }
    [field: SerializeField] public Transform BackgroundUIHolder { get; private set; }
    [field: SerializeField] public FPSCounter FPSCounter { get; private set; }
    [field: SerializeField] public GlobalWalletView GlobalWalletView { get; private set; }
    [field: SerializeField] public Joystick RotateJoystick { get; private set; }
    [field: SerializeField] public Joystick MoveJoystick { get; private set; }
    [field: SerializeField] public JoystickButton AttackButton { get; private set; }

    private void Awake ()
    {
        BackgroundUIHolder.SetAsFirstSibling();
    }
}
