using SignalsFramework;
using UniRx;
using UnityEngine;
using UpdateSys;

public class RoadDetection : MonoBehaviour, ILateUpdatable
{
    [SerializeField] private LayerMask _roadLayer = default;
    [SerializeField] private float _rayDistance = 5;

    #region "Signals"

    public readonly Subject<bool> OnGrounded = new Subject<bool>();

    #endregion

    #region "Fields"

    private RaycastHit _hit = default;
    private bool _isGrounded = false;

    #endregion

    private void OnEnable()
    {
        this.StartLateUpdate();
    }

    private void OnDisable()
    {
        this.StopLateUpdate();
    }

    public void OnSystemLateUpdate(float deltaTime)
    {
        UnityEngine.Profiling.Profiler.BeginSample("LateUpdate :: RoadDetection");

        if (Physics.Raycast(transform.position, -transform.up, out _hit, _rayDistance, _roadLayer))
        {
            Toggle(true);
        }
        else
        {
            Toggle(false);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void Toggle(bool state)
    {
        if (state == _isGrounded) return;

        _isGrounded = state;
        OnGrounded.Fire(_isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * _rayDistance);
    }
}
