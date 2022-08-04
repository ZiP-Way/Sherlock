using FluffyUnderware.Curvy.Controllers;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class Player : MonoBehaviour
{
    [SerializeField, HideInInspector] private SplineController _splineController = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    private void Awake()
    {
        LevelRoad.SpawnPlayer.Subscribe(spline =>
        {
            _splineController.Spline = spline;
            _splineController.Position = 0;
            _splineController.OffsetRadius = 0;
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _splineController = GetComponent<SplineController>();
    }
#endif
}
