using Data;
using Pooling;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class FlightNoteBookFX : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Vector2 _randomOffset = default;

    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _randomDuration = 0.2f;

    [Header("Components")]
    [SerializeField] private GameObject _prefab = default;
    [SerializeField] private Transform _spawnPoint = default;
    [SerializeField] private Transform _destination = default;
    [SerializeField] private Transform _container = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    private readonly HashSet<FlightNoteBook> _elements = new HashSet<FlightNoteBook>();

    #endregion

    private void Awake()
    {
        Hub.Cleanup.Subscribe(_ =>
        {
            AllElementsDecommission();
        }).AddTo(_ltt);

        TraceCollecting.TraceCollected.Subscribe(_ =>
        {
            DoFX();
        }).AddTo(_ltt);
    }

    private void OnDisable()
    {
        AllElementsDecommission();
    }

    private void AllElementsDecommission()
    {
        foreach (FlightNoteBook element in _elements)
        {
            element.Decommission();
        }

        _elements.Clear();
    }

    private void DoFX()
    {
        FlightNoteBook flightNoteBook = _prefab.Pool<FlightNoteBook>();

        flightNoteBook.AttachTo(_container);

        Vector3 randomPos = _spawnPoint.position;
        randomPos.x += Random.Range(-_randomOffset.x, _randomOffset.x);
        randomPos.y += Random.Range(-_randomOffset.y, _randomOffset.y);

        flightNoteBook.SetPosition(randomPos);

        float duration = _duration + Random.Range(-_randomDuration, _randomDuration);
        flightNoteBook.DoAnimation(randomPos, _destination.position, duration);

        _elements.Add(flightNoteBook);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
