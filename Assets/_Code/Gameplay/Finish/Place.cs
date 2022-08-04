using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class Place : MonoBehaviour
{
    [SerializeField] private Camera _wrondDummyCamera = default;

    [Header("VFX Components")]
    [SerializeField] private SmokeFX _smokeFx = default;
    [SerializeField] private WatchOnDummyFX _watchOnDummyFx = default;

    [Header("SFX Components")]
    [SerializeField] private AudioSource _audioSource = default;

    [SerializeField, HideInInspector] private DummyAnimationsController _dummyAnimationsController = default;
    [SerializeField, HideInInspector] private DummyAnimationsEvents _dummyAnimationsEvents = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Properties"

    public bool IsWinningPlace
    {
        get
        {
            return _isWinningPlace;
        }
        set
        {
            _isWinningPlace = value;
        }
    }
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _watchOnDummyFx.Toggle(true);
            _watchOnDummyFx.SetXLocalPosition(transform.localPosition.x);

            _isSelected = value;
        }
    }

    #endregion

    #region "Fields"

    private bool _isWinningPlace = false;
    private bool _isSelected = false;

    #endregion

    private void Awake()
    {
        _dummyAnimationsEvents.DiscontentAnimationEnd.Subscribe(_ =>
        {
            Finish.PlayerFinished.Fire(_isWinningPlace);
        }).AddTo(_ltt);

        ChoiceDetection.PlayerChose.Where(_ => _isSelected).Subscribe(_ =>
        {
            PlayerMovement.StopMovement.Fire();

            if (!_isWinningPlace)
            {
                _wrondDummyCamera.transform.localPosition = new Vector3(transform.localPosition.x,
                    _wrondDummyCamera.transform.localPosition.y,
                    _wrondDummyCamera.transform.localPosition.z);

                _dummyAnimationsController.DoDiscontentedLight();
            }
            else
            {
                _dummyAnimationsController.DoDiscontentedHard();
            }

            _audioSource.Play();

            _smokeFx.SetXLocalPosition(transform.localPosition.x);
            _smokeFx.Toggle(true);
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();

        _dummyAnimationsController = GetComponentInChildren<DummyAnimationsController>();
        _dummyAnimationsEvents = GetComponentInChildren<DummyAnimationsEvents>();
    }
#endif
}
