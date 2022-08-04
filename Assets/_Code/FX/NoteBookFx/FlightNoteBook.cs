using Pooling;
using SignalsFramework;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;

public class FlightNoteBook : MonoBehaviour, IGenericPoolElement, IUpdatable
{
    #region "Signals"

    public static readonly Subject<Unit> Complete = new Subject<Unit>();

    #endregion

    #region "Fields"

    private Vector3 _startPosition = default;
    private Vector3 _endPosition = default;

    private float _startTime = default;
    private float _duration = default;

    #endregion

    private void OnDisable()
    {
        this.StopUpdate();
    }

    public void AttachTo(Transform container)
    {
        transform.SetParent(container);
        transform.localScale = Vector3.one;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void DoAnimation(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        _startTime = Time.time;

        _startPosition = startPoint;
        _endPosition = endPoint;
        _duration = duration;

        this.StartUpdate();
    }

    public void OnSystemUpdate(float deltaTime)
    {
        Profiler.BeginSample("FlightCoin:: OnSystemUpdate");

        DoMovement();

        Profiler.EndSample();
    }

    private void DoMovement()
    {
        float percentage = (Time.time - _startTime) / _duration;
        transform.position = Vector3.Lerp(_startPosition, _endPosition, percentage);

        if (percentage >= 1f)
        {
            OnComplete();
        }
    }

    public void OnComplete()
    {
        FlightNoteBook.Complete.Fire();

        this.StopUpdate();
        Decommission();
    }

    #region [IGenericPoolElement implementation]

    public int PoolRef { get; set; }
    public bool IsAvailable => false;
    public bool IsCommissioned { get; set; }
    public bool UsesAutoPool { get; set; }

    public void Commission() { gameObject.SetActive(true); }

    public void Decommission()
    {
        if (!IsCommissioned) return;

        gameObject.SetActive(false);

        this.ReturnToPool();
    }

    public void OnDestroy() { this.RemoveFromPool(); }

    #endregion
}
