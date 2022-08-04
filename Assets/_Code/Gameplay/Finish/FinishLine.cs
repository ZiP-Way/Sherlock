using Data;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using Utility;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField, HideInInspector] private BoxCollider _boxCollider = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<Unit> PlayerCrossedLine = new Subject<Unit>();

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            _boxCollider.enabled = true;
        }).AddTo(_ltt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _boxCollider.enabled = false;
            FinishLine.PlayerCrossedLine.Fire();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _boxCollider = GetComponent<BoxCollider>();
    }
#endif
}
