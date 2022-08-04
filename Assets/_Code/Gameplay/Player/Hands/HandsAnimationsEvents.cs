using SignalsFramework;
using UnityEngine;

public class HandsAnimationsEvents : MonoBehaviour
{
    private void DoSign()
    {
        Notepad.DoSign.Fire();
    }
}
