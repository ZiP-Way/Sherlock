using Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class DummyCuffer : MonoBehaviour
{
    [SerializeField] private GameObject[] _handcuffs = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<int> Cuff = new Subject<int>();

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            foreach (var handcuff in _handcuffs)
            {
                handcuff.SetActive(false);
            }
        }).AddTo(_ltt);

        DummyCuffer.Cuff.Subscribe(handcuffId =>
        {
            _handcuffs[handcuffId].SetActive(true);
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
