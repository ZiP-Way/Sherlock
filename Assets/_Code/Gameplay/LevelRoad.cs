using Data;
using FluffyUnderware.Curvy;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class LevelRoad : MonoBehaviour
{
    [SerializeField] private CurvySpline _playerSpline = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<CurvySpline> SpawnPlayer = new Subject<CurvySpline>();

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            LevelRoad.SpawnPlayer.Fire(_playerSpline);
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
