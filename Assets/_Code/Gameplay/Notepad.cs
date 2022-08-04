using Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class Notepad : MonoBehaviour
{
    [SerializeField] private GameObject[] _marks = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static Subject<Unit> DoSign = new Subject<Unit>();

    #endregion

    #region "Fields"

    private int _countOfActiveMarks = 0;

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            ResetMarks();
        }).AddTo(_ltt);

        Notepad.DoSign.Subscribe(_ =>
        {
            if (_marks.Length == _countOfActiveMarks) return;

            _marks[_countOfActiveMarks].SetActive(true);
            _countOfActiveMarks++;
        }).AddTo(_ltt);
    }

    private void ResetMarks()
    {
        _countOfActiveMarks = 0;

        foreach (var mark in _marks)
        {
            mark.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
