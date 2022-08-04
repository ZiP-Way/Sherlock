using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;
using Utility;

public class ShowingTracesAnimation : MonoBehaviour, IFixedUpdatable
{
    [Header("Animation Settings")]
    [SerializeField] private float _duration = 2;

    [SerializeField, HideInInspector] private Trace[] _traces = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public readonly Subject<Unit> Run = new Subject<Unit>();
    public readonly Subject<Unit> Stop = new Subject<Unit>();

    public static readonly Subject<float> ValueChanged = new Subject<float>();

    #endregion

    #region "Fields"

    private bool _isAlreadyCompleted = false;

    private float _currentValue = 0;
    private float _oneTraceValue = 0;

    private int _countOfShowedTraces = 1;

    #endregion

    private void Awake()
    {
        _oneTraceValue = 1f / _traces.Length;

        TracesToggle(false);

        this.Run.Where(_ => !_isAlreadyCompleted).Subscribe(_ =>
        {
            RunAnimation();
        }).AddTo(_ltt);

        this.Stop.Where(_ => !_isAlreadyCompleted).Subscribe(_ =>
        {
            StopAnimation();
        }).AddTo(_ltt);
    }

    public void OnSystemFixedUpdate(float deltaTime)
    {
        Profiler.BeginSample("ShowingTracesAnimation :: OnSystemFixedUpdate");

        _currentValue += deltaTime / _duration;
        ShowingTracesAnimation.ValueChanged.Fire(_currentValue);

        if (_currentValue >= _oneTraceValue * _countOfShowedTraces)
        {
            _traces[_countOfShowedTraces - 1].Toggle(true);
            _countOfShowedTraces++;
        }

        if (_countOfShowedTraces == _traces.Length + 1)
        {
            _isAlreadyCompleted = true;
            StopAnimation();
        }

        Profiler.EndSample();
    }

    private void RunAnimation()
    {
        this.StartFixedUpdate();

        ShowingTracesAnimation.ValueChanged.Fire(_currentValue);
        TracesFillingIndicator.DoToggle.Fire(true);
    }

    private void StopAnimation()
    {
        this.StopFixedUpdate();

        TracesFillingIndicator.DoToggle.Fire(false);
    }

    private void TracesToggle(bool state)
    {
        foreach (var trace in _traces)
        {
            trace.Toggle(state);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _traces = GetComponentsInChildren<Trace>();
    }
#endif
}
