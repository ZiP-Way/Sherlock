using FluffyUnderware.Curvy;
using UnityEditor;
using UnityEngine;

public class TracesGeneration : MonoBehaviour
{
#if UNITY_EDITOR
    public enum GeneratePosition
    {
        Start,
        Midle,
        End
    }

    [SerializeField] private int _tracesCount = 10;
    [SerializeField] private GeneratePosition _generationPosition = GeneratePosition.Start;

    [Header("Components")]
    [SerializeField] private Trace _tracePrefab = default;
    [SerializeField] private CurvySpline _spline = default;
    [SerializeField] private Transform _container = default;

    [ContextMenu("Generate Traces")]
    private void GenerateTraces()
    {
        Clear();

        if (_generationPosition == GeneratePosition.Start)
        {
            float tracePositionOnSpline = _spline.Length / _tracesCount;

            for (int i = 0; i < _tracesCount; i++)
            {
                Trace trace = PrefabUtility.InstantiatePrefab(_tracePrefab, _container) as Trace;
                trace.SetPositionOnSpline(_spline, tracePositionOnSpline * i);
            }
        }

        if (_generationPosition == GeneratePosition.Midle)
        {
            float tracePositionOnSpline = _spline.Length / (_tracesCount + 1);

            for (int i = 1; i <= _tracesCount; i++)
            {
                Trace trace = PrefabUtility.InstantiatePrefab(_tracePrefab, _container) as Trace;
                trace.SetPositionOnSpline(_spline, tracePositionOnSpline * i);
            }
        }

        if (_generationPosition == GeneratePosition.End)
        {
            float tracePositionOnSpline = _spline.Length / _tracesCount;

            for (int i = 1; i <= _tracesCount; i++)
            {
                Trace trace = PrefabUtility.InstantiatePrefab(_tracePrefab, _container) as Trace;
                trace.SetPositionOnSpline(_spline, tracePositionOnSpline * i);
            }
        }
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        Trace[] _traces = GetComponentsInChildren<Trace>();

        foreach (Trace generatedTrace in _traces)
        {
            DestroyImmediate(generatedTrace.gameObject);
        }
    }

    private void OnValidate()
    {
        _spline = GetComponentInChildren<CurvySpline>();
    }
#endif
}
