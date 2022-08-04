using SignalsFramework;
using UniRx;
using UnityEngine;
using UpdateSys;

public class ObjectDetection : MonoBehaviour, ILateUpdatable
{
    [SerializeField] private float _rayDistance = 5;
    [SerializeField] private LayerMask _collideLayers = default;

    [Header("Points")]
    [SerializeField] private Transform _rightPoint = default;
    [SerializeField] private Transform _midlePoint = default;
    [SerializeField] private Transform _leftPoint = default;

    #region "Signals"

    public readonly Subject<bool> IsCollidingWithObject = new Subject<bool>();

    #endregion

    #region "Fields"

    private RaycastHit _hit = default;
    private bool _isCollided = false;

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
        UnityEngine.Profiling.Profiler.BeginSample("LateUpdate :: ObjectDetection");

        if (Physics.Raycast(_midlePoint.position, _midlePoint.forward, out _hit, _rayDistance, _collideLayers) ||
            Physics.Raycast(_rightPoint.position, _rightPoint.forward, out _hit, _rayDistance, _collideLayers) ||
            Physics.Raycast(_leftPoint.position, _leftPoint.forward, out _hit, _rayDistance, _collideLayers))
        {
            if (!_hit.collider.isTrigger)
                Toggle(false);
        }
        else
        {
            Toggle(true);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void Toggle(bool state)
    {
        if (state == _isCollided) return;

        _isCollided = state;
        IsCollidingWithObject.Fire(_isCollided);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(_midlePoint.position, _midlePoint.forward * _rayDistance);
        Gizmos.DrawRay(_rightPoint.position, _rightPoint.forward * _rayDistance);
        Gizmos.DrawRay(_leftPoint.position, _leftPoint.forward * _rayDistance);
    }
#endif
}
