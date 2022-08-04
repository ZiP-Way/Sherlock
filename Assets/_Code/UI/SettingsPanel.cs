using Data;
using Data.RateUs;
using DG.Tweening;
using EditorExtensions.Attributes;
using Profile;
using SignalsFramework;
using UI.Buttons;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    /// <summary>
    /// Logic related to settings panel
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [FoldOutStart("Animation settings")]
        [SerializeField]
        private float _panelSize = 610.0f;

        [SerializeField]
        private float _panelSizeNoRateUs = 610;

        [SerializeField]
        private float _closePanelSize = 0;

        [SerializeField]
        private float _timeToShowPanel = 0.2f;

        [FoldOutEnd]
        [SerializeField]
        private Vector2 _tweenDurationMinMax = new Vector2(0.15f, 0.20f);

        [FoldOutStart("Components")]
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [SerializeField, RequireInput]
        private Button _settingsBtn = default;

        [SerializeField, RequireInput]
        private RectTransform _settingsBg = default;

        [Space]
        [SerializeField, RequireInput]
        private Transform _vibroButtonTrm = default;

        [SerializeField, RequireInput]
        private DisabledStateBtn _vibroBtn = default;

        [Space]
        [SerializeField, RequireInput]
        private Transform _soundButtonTrm = default;

        [SerializeField, RequireInput]
        private DisabledStateBtn _soundBtn = default;

        [Space]
        [SerializeField, RequireInput]
        private Transform _restorePurchaseButtonTrm = default;

        [SerializeField, RequireInput]
        private Button _restorePurchaseBtn = default;

        [Space]
        [SerializeField, RequireInput]
        private Button _rateUsBtn = default;

        [SerializeField, Auto]
        private Transform _rateUsTrm = default;

        [FoldOutEnd]
        [SerializeField, Auto]
        private GameObject _rateUsGO = default;

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        [SerializeField, HideInInspector]
        private RectTransform _rootRectTransform = default;

        #region [Properties]

        public bool IsActive => _isActive;
        private bool _isActive = false;

        #endregion

        #region [Fields]

        private bool _isTweening = false;

        private static bool ShouldShowRateUs => !PlayerProfile.RateUsData.RateUsDeniedCompletely;

        private TweenCallback _onShowComplete;
        private TweenCallback _onHideAnimationComplete;

        #endregion

        private void Awake()
        {
            SetupButtonStates();

            CreateCallbacks();
            ForceHide();
        }

        private void CreateCallbacks()
        {
            _onShowComplete = () =>
            {
                _isActive = true;
                _isTweening = false;
            };

            _onHideAnimationComplete = () =>
            {
                _root.SetActive(false);
                _isActive = false;
                _isTweening = false;
            };

            _settingsBtn.onClick.AddListener(Toggle);
            _rateUsBtn.onClick.AddListener(OnRateUsBtnClick);

            _vibroBtn.OnClick.AddListener(ToggleVibroState);
            _soundBtn.OnClick.AddListener(ToggleSoundState);

            _restorePurchaseBtn.onClick.AddListener(() => Hub.RestorePurchases.Fire());

            Hub.VibroDisabled.Subscribe(disabled => _vibroBtn.SetState(!disabled)).AddTo(_ltt);
            Hub.SoundDisabled.Subscribe(disabled => _soundBtn.SetState(!disabled)).AddTo(_ltt);

            Hub.GameStarted.Subscribe(_ =>
            {
                _isActive = true;
                Toggle();
            }).AddTo(_ltt);
        }

        private void SetupButtonStates()
        {
            _vibroBtn.SetState(!PlayerProfile.VibroDisabled);
            _soundBtn.SetState(!PlayerProfile.SoundDisabled);
        }

        private void ToggleVibroState()
        {
            PlayerProfile.VibroDisabled = !PlayerProfile.VibroDisabled;
        }

        private void ToggleSoundState()
        {
            PlayerProfile.SoundDisabled = !PlayerProfile.SoundDisabled;
        }

        public void Toggle()
        {
            if (_isTweening) return;

            if (_isActive)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void ForceHide()
        {
            _isTweening = false;
            _isActive = false;

            ZeroOutScale();
        }

        private void ZeroOutScale()
        {
            _settingsBg.sizeDelta = new Vector2(_settingsBg.sizeDelta.x, _closePanelSize);
            _soundButtonTrm.localScale = Vector3.zero;
            _vibroButtonTrm.localScale = Vector3.zero;
            _restorePurchaseButtonTrm.localScale = Vector3.zero;
            _rateUsTrm.localScale = Vector3.zero;
        }

        private void OnRateUsBtnClick()
        {
            Hub.ShowRateUs.Fire(new RateUsRequest
            {
                IsCalledFromSettings = true,
            });
            Hide();
        }

        private void Show()
        {
            _isTweening = true;

            ZeroOutScale();

            bool shouldShowRateUs = ShouldShowRateUs;
            _rateUsGO.SetActive(shouldShowRateUs);

            float panelSizeY = shouldShowRateUs ? _panelSize : _panelSizeNoRateUs;

            _settingsBg.DOSizeDelta(new Vector2(_settingsBg.sizeDelta.x, panelSizeY), _timeToShowPanel)
                       .SetEase(Ease.InOutBack)
                       .SetAutoKill(true);

            _rootRectTransform.sizeDelta = new Vector2(_rootRectTransform.sizeDelta.x, panelSizeY);

            _root.SetActive(true);

            ShowButtons();
        }

        private void ShowButtons()
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.03f);
            seq.Join(TweenIn(_soundButtonTrm));
            seq.Join(TweenIn(_vibroButtonTrm));
            seq.Join(TweenIn(_restorePurchaseButtonTrm));
            seq.Join(TweenIn(_rateUsTrm));

            seq.OnComplete(_onShowComplete);
        }

        private Tweener TweenIn(Transform trm)
        {
            return trm.DOScale(Vector3.one, Random.Range(_tweenDurationMinMax.x, _tweenDurationMinMax.y))
                      .SetEase(Ease.InOutBack)
                      .SetAutoKill(true);
        }

        private Tweener TweenOut(Transform trm)
        {
            return trm.DOScale(new Vector3(0, 0, 1), Random.Range(_tweenDurationMinMax.x, _tweenDurationMinMax.y))
                      .SetEase(Ease.InBack)
                      .SetAutoKill(true);
        }

        private void Hide()
        {
            _isTweening = true;
            _isActive = false;

            DoHideAnimation();
        }

        private void DoHideAnimation()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(TweenOut(_restorePurchaseButtonTrm));
            seq.Join(TweenOut(_vibroButtonTrm));
            seq.Join(TweenOut(_soundButtonTrm));
            seq.Join(TweenOut(_rateUsTrm));

            _settingsBg.DOSizeDelta(new Vector2(_settingsBg.sizeDelta.x, _closePanelSize), _timeToShowPanel)
                       .SetEase(Ease.InBack)
                       .SetAutoKill(true)
                       .OnComplete(_onHideAnimationComplete);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();

            if (_rateUsBtn != null)
            {
                _rateUsTrm = _rateUsBtn.transform;
                _rateUsGO = _rateUsBtn.gameObject;
            }

            if (_vibroButtonTrm != null)
                _vibroButtonTrm.TryGetComponent(out _vibroBtn);

            if (_soundButtonTrm != null)
                _soundButtonTrm.TryGetComponent(out _soundBtn);

            if (_rootRectTransform == null) _rootRectTransform = _root.GetComponent<RectTransform>();
        }
#endif
    }
}