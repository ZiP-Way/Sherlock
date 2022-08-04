using Data;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class Finish : MonoBehaviour
{
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<bool> PlayerFinished = new Subject<bool>();

    #endregion

    private void Awake()
    {
        Finish.PlayerFinished.First().Subscribe(isWin =>
        {
            if (isWin)
            {
                Profile.PlayerProfile.CurrentLevel++;
                Hub.LevelComplete.Fire();
            }
            else
            {
                Hub.LevelFailed.Fire();
            }
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
