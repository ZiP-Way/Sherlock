using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UpdateSys;
using Utility;

public class WatchOn : MonoBehaviour, ILateUpdatable
{
    [SerializeField] private LayerMask _placeLayer = default;
    [SerializeField] private float _rayDistance = 5;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    #endregion

    #region "Fields"

    private RaycastHit _hit = default;
    private Place _currentPlace = default;

    #endregion

    private void Awake()
    {
        FinishLine.PlayerCrossedLine.Subscribe(_ =>
        {
            this.StartLateUpdate();
        }).AddTo(_ltt);

        Finish.PlayerFinished.Subscribe(_ =>
        {
            this.StopLateUpdate();
        }).AddTo(_ltt);
    }

    private void OnDisable()
    {
        this.StopLateUpdate();
    }

    public void OnSystemLateUpdate(float deltaTime)
    {
        UnityEngine.Profiling.Profiler.BeginSample("LateUpdate :: WatchOn");

        if (Physics.Raycast(transform.position, transform.forward, out _hit, _rayDistance, _placeLayer))
        {
            if (_hit.collider.TryGetComponent(out Place place))
            {
                if (place != _currentPlace)
                {
                    if (_currentPlace != null)
                        _currentPlace.IsSelected = false;

                    place.IsSelected = true;

                    _currentPlace = place;
                }
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * _rayDistance);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
