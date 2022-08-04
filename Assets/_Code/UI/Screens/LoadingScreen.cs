using Data;
using DG.Tweening;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class LoadingScreen : MonoBehaviour
{
    [Header("Animations Settings")]
    [SerializeField] private float _showingAnimationSpeed = 0.25f;
    [SerializeField] private float _hidingAnimationSpeed = 0.25f;
    [SerializeField] private float _delayBeforeShowing = 1f;

    [Header("Components")]
    [SerializeField] private Image _image = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<Unit> ShowLoadingScreen = new Subject<Unit>();

    #endregion

    #region "Fields"

    private Sequence _showingAniamtion = default;
    private Sequence _hidingAnimation = default;

    private CompositeDisposable _disposables = default;

    #endregion

    private void Awake()
    {
        _disposables = new CompositeDisposable();

        BuildShowingAnimation();
        BuildHidingAnimation();

        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            _image.gameObject.SetActive(true);

            Observable.Timer(System.TimeSpan.FromSeconds(_delayBeforeShowing)).Subscribe(_ =>
            {
                Hide();
            }).AddTo(_disposables);
        }).AddTo(_ltt);

        LoadingScreen.ShowLoadingScreen.Subscribe(_ =>
        {
            Show();
        }).AddTo(_ltt);
    }

    private void BuildHidingAnimation()
    {
        _hidingAnimation = DOTween.Sequence();
        _hidingAnimation.SetAutoKill(false);
        _hidingAnimation.Pause();

        _hidingAnimation.Append(_image.DOFade(0, _hidingAnimationSpeed));
        _hidingAnimation.OnComplete(() => _image.gameObject.SetActive(false));
    }

    private void BuildShowingAnimation()
    {
        _showingAniamtion = DOTween.Sequence();
        _showingAniamtion.SetAutoKill(false);
        _showingAniamtion.Pause();

        _showingAniamtion.OnPlay(() => _image.gameObject.SetActive(true));
        _showingAniamtion.Append(_image.DOFade(1, _showingAnimationSpeed));
        _showingAniamtion.OnComplete(() => Hub.LoadLevel.Fire());
    }

    private void Hide()
    {
        if (!_hidingAnimation.IsPlaying())
        {
            _hidingAnimation.Restart();
        }
    }

    private void Show()
    {
        if (!_showingAniamtion.IsPlaying())
        {
            _showingAniamtion.Restart();
        }
    }

    private void OnEnable()
    {
        if (_disposables == null)
            _disposables = new CompositeDisposable();
    }

    private void OnDisable()
    {
        if (_disposables != null)
            _disposables.Dispose();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
