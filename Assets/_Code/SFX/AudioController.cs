using Data;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class AudioController : MonoBehaviour
{
    [Header("MainTheme Audio Settings")]
    [SerializeField] private float _fadeDuration = 2f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _doSignAudio = default;
    [SerializeField] private AudioSource _mainThemeMusic = default;
    [SerializeField] private AudioSource _traceCollectingAudio = default;
    [SerializeField] private AudioSource _collideWithObstacleAudio = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    private Sequence _mainThemeFade = default;
    private float _mainThemeVolume = 0;

    #endregion

    private void Awake()
    {
        _mainThemeFade = DOTween.Sequence();
        BuildAnimation();

        _mainThemeVolume = _mainThemeMusic.volume;

        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            if (_mainThemeFade.IsPlaying())
                _mainThemeFade.Pause();

            _mainThemeMusic.volume = _mainThemeVolume;
            _mainThemeMusic.Play();
        }).AddTo(_ltt);

        ChoiceDetection.PlayerChose.Subscribe(_ =>
        {
            _mainThemeFade.Restart();
        }).AddTo(_ltt);

        TraceCollecting.TraceCollected.Subscribe(_ =>
        {
            _traceCollectingAudio.Play();
        }).AddTo(_ltt);

        ObstacleDetection.PlayerColliderWithObject.Subscribe(_ =>
        {
            _collideWithObstacleAudio.Play();
        }).AddTo(_ltt);

        Notepad.DoSign.Subscribe(_ =>
        {
            _doSignAudio.Play();
        }).AddTo(_ltt);
    }

    private void BuildAnimation()
    {
        _mainThemeFade.SetAutoKill(false);
        _mainThemeFade.Pause();

        _mainThemeFade.Append(_mainThemeMusic.DOFade(0, _fadeDuration));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
