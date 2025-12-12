using UnityEngine;

public class NativeBridgeButton : MonoBehaviour
{
    NativeBridge bridge;

    void Awake()
    {
        bridge = FindObjectOfType<NativeBridge>();
    }

    public void ReturnToApp()
    {
        if (bridge != null)
            bridge.ReturnToNativeApp();
    }
}
