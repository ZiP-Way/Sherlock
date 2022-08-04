using System;
using System.Collections.Generic;
using Data;
using Data.Analytics;
using Data.RateUs;
using DG.Tweening;
using EditorExtensions.Attributes;
using Profile;
using SignalsFramework;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace UI.RateUs {
    public class RateUsScreen : MonoBehaviour {
        [SerializeField]
        private long DelayBetweenShowsInSeconds = 86400;

        // TODO these can be received from Remote Config
        [SerializeField]
        private int _levelsToShowFirstTime = 1;

        [SerializeField]
        private int _levelsToShowSecondTime = 5;

        [SerializeField]
        private int _sessionsToShowAfter = 5;
        // TODO

        [FoldOutStart("Animation properties")]
        [SerializeField]
        private Vector2 _minMaxAnimDuration = new Vector2(0.15f, 0.20f);

        [SerializeField]
        private float _animNoThanksAfter = 1f;

        [FoldOutEnd]
        [SerializeField]
        private float _fadeDuration = 0.15f;

        [FoldOutStart("Rate text setup")]
        [SerializeField, RequireInput]
        private TMP_Text _rateText = default;

        [FoldOutEnd]
        [SerializeField]
        private float _rateTextTransparency = 0.3f;

        [FoldOutStart("Components")]
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [SerializeField, Auto]
        private CanvasGroup _cg = default;

        [SerializeField, RequireInput]
        private Button _rateBtn = default;

        [SerializeField, RequireInput]
        private Transform _noThanksTrm = default;

        [SerializeField, Auto]
        private Transform _rateBtnTrm = default;

        [SerializeField, RequireInput]
        private Button _cancelBtn = default;

        [FoldOutEnd]
        [SerializeField, Auto]
        private List<StarButton> _starButtons = new List<StarButton>();

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Signals]

        private int _lastSelectedIndex;
        private readonly Subject<int> _starButtonPressed = new Subject<int>();

        private bool _transitionToMainMenu;
        private TweenCallback _disableRoot;

        #endregion

        #region [Properties]

        private bool ShouldShowRateUs {
            get
            {
                RateUsData data = PlayerProfile.RateUsData;
#if UNITY_ANDROID
                // Test if the first deny is set (1-4 stars is set and rated)
                if (data.RateUsDeniedCompletely) return false;

                // Check if both time and play sessions has passed
                if (data.RateUsSecondCancel) {
                    // 24h not passed, or time delay not passed
                    if (!IsTimePassed) return false;

                    // Check if user has unlocked X levels
                    return data.RateUsLevels >= LevelsToUnlockToShowSecondTime;
                }

                // Check if the time has passed and X levels has been unlocked
                if (data.RateUsFirstCancel) {
                    // 24h not passed, or time delay not passed
                    if (!IsTimePassed) return false;

                    return data.RateUsLevels >= LevelsToShowFirstTime;
                }

                return PlayerProfile.TotalSessionsPlayed >= SessionsToShowAfter;
#else
                // Check if the time has passed first
                if (!IsTimePassed) return false;

                return data.RateUsSessionsPlayed >= SessionsToShowAfter;
#endif
            }
        }

        private bool IsTimePassed {
            get {
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                RateUsData data = PlayerProfile.RateUsData;

                return now - data.LastCancelTime >= DelayBetweenShowsInSeconds;
            }
        }

        // 5
        private int SessionsToShowAfter => _sessionsToShowAfter;

        // 1
        private int LevelsToShowFirstTime => _levelsToShowFirstTime;
        
        // 5
        private int LevelsToUnlockToShowSecondTime => _levelsToShowSecondTime;

        #endregion

        #region [Fields]

        private string _showReason;

        #endregion

        private void Awake() {
            InitButtons();

            _disableRoot = () => _root.SetActive(false);
            _starButtonPressed.Subscribe(OnStarButtonPressed).AddTo(_ltt);
            _rateBtn.onClick.AddListener(OnRatePressed);
            _cancelBtn.onClick.AddListener(OnCancelPressed);

            Hub.ShowRateUs.Subscribe(OnShowRequest).AddTo(_ltt);

            Hide(true);
        }

        private void OnShowRequest(RateUsRequest request) {
            _transitionToMainMenu = request.ShouldTransitionToLobby;
            _showReason = _transitionToMainMenu ? "auto_condition_met" : "user_action";

#if UNITY_IOS
            // User action or auto trigger check
            if (!_transitionToMainMenu || ShouldShowRateUs) {
                Hub.TrackRateUs.Fire(new RateUsTrackData
                                     {
                                         ShowReason = _showReason,
                                     });

                // Reset time / sessions in case of proc
                if (_transitionToMainMenu) {
                    RateUsData data = PlayerProfile.RateUsData;
                    
                    data.LastCancelTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    data.RateUsSessionsPlayed = 0;
                    
                    PlayerProfile.RateUsData = data;
                }
                
                OpenStore();
            }

            if (_transitionToMainMenu) {
                _transitionToMainMenu = false;
                Hub.RequestLobbyTransition.Fire();
            }
            
            return;
#else
            ToggleRateBtn(false);
            Show(request.IsCalledFromSettings);
#endif
        }

        private void ToggleRateBtn(bool state) {
            _rateText.alpha = state ? 1 : _rateTextTransparency;
            _rateBtn.interactable = state;
        }

        private void InitButtons() {
            for (int i = 0; i < _starButtons.Count; i++) {
                _starButtons[i].Init(i, _starButtonPressed);
            }
        }

        public void Show(bool fromMainMenu = true) {
            if (!fromMainMenu) {
                if (!ShouldShowRateUs) {
                    Hide(true);
                    return;
                }
            }

            PlayShowAnimation();
            ResetButtonState();

            _root.SetActive(true);
        }

        private void PlayShowAnimation() {
            _rateBtnTrm.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            _noThanksTrm.localScale = Vector3.zero;
            _cg.alpha = 0;

            Sequence seq = DOTween.Sequence();
            seq.Append(_cg.DOFade(1f, _fadeDuration));
            seq.Append(_rateBtnTrm.DOScale(Vector3.one, Random.Range(_minMaxAnimDuration.x, _minMaxAnimDuration.y))
                                  .SetEase(Ease.InOutBack));
            seq.AppendInterval(_animNoThanksAfter);

            seq.Append(_noThanksTrm.DOScale(Vector3.one, Random.Range(_minMaxAnimDuration.x, _minMaxAnimDuration.y))
                                   .SetEase(Ease.Linear));
        }

        private void OnRatePressed() {
            if (_lastSelectedIndex < 0) return;

            int stars = _lastSelectedIndex + 1;

            if (stars >= 1) {
                OpenStore();
            }

            // Disable rate us completely in both cases
            RateUsData data = PlayerProfile.RateUsData;
            data.RateUsDeniedCompletely = true;
            PlayerProfile.RateUsData = data;

            Hub.TrackRateUs.Fire(new RateUsTrackData
                                 {
                                     ShowReason = _showReason,
                                     RateResult = stars
                                 });
            Hide();
        }

        private void OpenStore() {
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=game.hyper.detective");
#elif UNITY_IOS
            Device.RequestStoreReview();
#endif
        }

        private void OnCancelPressed() {
            bool modifyBehaviour = _transitionToMainMenu;

            Hide();

            Hub.TrackRateUs.Fire(new RateUsTrackData
                                 {
                                     ShowReason = _showReason,
                                     RateResult = 0
                                 });

            // Modify behaviour of the show only if the user has been shown rate us from the auto-trigger
            // game logic, don't do anything if called from main menu
            if (!modifyBehaviour) return;

            RateUsData data = PlayerProfile.RateUsData;
            
            data.LastCancelTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            data.RateUsLevels = 0;

            if (!data.RateUsFirstCancel) {
                data.RateUsFirstCancel = true;

                PlayerProfile.RateUsData = data;
                return;
            }

            if (!data.RateUsSecondCancel) {
                data.RateUsSecondCancel = true;
            }

            PlayerProfile.RateUsData = data;
        }

        public void Hide(bool instantly = false) {
            if (instantly) {
                _root.SetActive(false);
            } else {
                _cg.DOFade(0, _fadeDuration).OnComplete(_disableRoot);
            }

            if (_transitionToMainMenu) {
                Hub.RequestLobbyTransition.Fire();
                _transitionToMainMenu = false;
            }
        }

        private void ResetButtonState() {
            _lastSelectedIndex = -1;

            foreach (StarButton button in _starButtons) {
                button.ToggleState(false);
            }
        }

        private void OnStarButtonPressed(int index) {
            ResetButtonState();

            _lastSelectedIndex = index;

            for (int i = 0; i < _starButtons.Count; i++) {
                _starButtons[i].ToggleState(i < index + 1);
            }

            ToggleRateBtn(true);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            GetComponentsInChildren(true, _starButtons);

            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
            
            if (_rateBtn != null) {
                _rateBtnTrm = _rateBtn.transform;
                _rateText = _rateBtn.GetComponentInChildren<TMP_Text>();
            }

            if (_root != null) _cg = _root.GetComponentInChildren<CanvasGroup>();
        }
#endif
    }
}