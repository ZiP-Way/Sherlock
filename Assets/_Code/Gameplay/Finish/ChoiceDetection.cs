using SignalsFramework;
using UniRx;
using UnityEngine;

public class ChoiceDetection : MonoBehaviour
{
    #region "Signals"

    public static readonly Subject<Unit> PlayerChose = new Subject<Unit>();

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            ChoiceDetection.PlayerChose.Fire();
        }
    }
}
