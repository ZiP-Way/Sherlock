using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utility;
using Data;
using Profile;

public class WalkingDummy : MonoBehaviour, IObstacle
{
    [SerializeField, HideInInspector] private AudioSource _stepAudio = default;
    [SerializeField, HideInInspector] private Animator _animator = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    private DummyAnimatorHashCodes _hashCodes = new DummyAnimatorHashCodes();
    private CompositeDisposable _disposables = new CompositeDisposable();

    #endregion

    private void Awake()
    {
        Hub.GameStarted.Where(_ => !PlayerProfile.SoundDisabled).Subscribe(_ =>
        {
            Observable.Timer(System.TimeSpan.FromSeconds(_stepAudio.clip.length)).Repeat().Subscribe(_ =>
            {
                _stepAudio.pitch = Random.Range(0.9f, 1.1f);
                _stepAudio.Play();
            }).AddTo(_disposables);
        }).AddTo(_ltt);

        _animator.Play(_hashCodes.WalkingAnimation);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }

    public void Pass()
    {

    }

    private void Toggle(bool state)
    {
        if (state == gameObject.activeInHierarchy) return;
        gameObject.SetActive(state);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _animator = GetComponent<Animator>();
        _stepAudio = GetComponent<AudioSource>();
    }
#endif
}
