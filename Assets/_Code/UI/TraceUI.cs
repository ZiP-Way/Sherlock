using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class TraceUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _root = default;
    [SerializeField] private TMP_Text _text = default;
    [SerializeField] private Image _traceIcon = default;

    [Header("Animation Settings")]
    [SerializeField] private Vector3 _startScaleValue = Vector3.zero;
    [SerializeField] private float _doBiggerDuration = 0.3f;
    [SerializeField] private float _delayBeforeFade = 0.1f;
    [SerializeField] private float _doFadeDuration = 0.3f;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    public static readonly Subject<Unit> Show = new Subject<Unit>();

    #endregion

    #region "Fields"

    private Sequence _sequence = default;

    #endregion

    private void Awake()
    {
        BuildAnimation();

        TraceUI.Show.Subscribe(_ =>
        {
            _sequence.Restart();
        }).AddTo(_ltt);
    }

    private void BuildAnimation()
    {
        _sequence = DOTween.Sequence();
        _sequence.SetAutoKill(false);
        _sequence.Pause();

        _root.localScale = _startScaleValue;

        _sequence.OnPlay(() => _root.gameObject.SetActive(true));
        _sequence.Append(_root.DOScale(1.1f, _doBiggerDuration));
        _sequence.Append(_root.DOScale(1f, _doBiggerDuration / 2));
        _sequence.AppendInterval(_delayBeforeFade);
        _sequence.Append(_text.DOFade(0, _doFadeDuration));
        _sequence.Join(_traceIcon.DOFade(0, _doFadeDuration));
        _sequence.OnComplete(() => _root.gameObject.SetActive(false));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
