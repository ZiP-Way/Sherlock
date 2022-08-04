using SignalsFramework;
using UniRx;
using UnityEngine;

public class DummyAnimationsEvents : MonoBehaviour
{
    #region "Signals"

    public readonly Subject<Unit> DiscontentAnimationEnd = new Subject<Unit>();

    #endregion

    private void DiscontentEnd()
    {
        DiscontentAnimationEnd.Fire();
    }
}
