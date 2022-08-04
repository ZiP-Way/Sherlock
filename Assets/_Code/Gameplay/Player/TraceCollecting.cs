using SignalsFramework;
using TapticFeedback;
using UniRx;
using UnityEngine;

public class TraceCollecting : MonoBehaviour
{
    #region "Signals"

    public static readonly Subject<Unit> TraceCollected = new Subject<Unit>();

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Trace trace))
        {
            trace.Toggle(false);

            Taptic.Light();

            TraceCollecting.TraceCollected.Fire();
        }
    }
}