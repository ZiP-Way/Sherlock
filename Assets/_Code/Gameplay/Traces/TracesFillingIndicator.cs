using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Utility;

public class TracesFillingIndicator : MonoBehaviour
{
    [SerializeField] private Image _indicator = default;
    [SerializeField] private GameObject _body = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"
    
    public static readonly Subject<bool> DoToggle = new Subject<bool>();
    
    #endregion

    private void Awake()
    {
        Toggle(false);

        TracesFillingIndicator.DoToggle.Subscribe(state =>
        {
            Toggle(state);
        }).AddTo(_ltt);

        ShowingTracesAnimation.ValueChanged.Subscribe(value =>
        {
            _indicator.fillAmount = value;
        }).AddTo(_ltt);
    }

    public void Toggle(bool state)
    {
        if (state == _body.activeInHierarchy) return;

        _body.SetActive(state);
    }

    public void Fill(float value)
    {
        _indicator.fillAmount += value;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
