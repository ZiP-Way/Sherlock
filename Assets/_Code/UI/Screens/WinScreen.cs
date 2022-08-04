using System;
using Data;
using Data.Levels;
using Data.RateUs;
using EditorExtensions.Attributes;
using FX.CoinFX;
using Profile;
using SignalsFramework;
using TMPro;
using Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Screens
{
    /// <summary>
    /// Logic for the level completion UI
    /// </summary>
    public class WinScreen : MonoBehaviour
    {
        [SerializeField]
        private float _delayAfterFlightCompletion = 0.5f;

        [Space]
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [SerializeField, RequireInput]
        private ABTweener _fadeTweener = default;

        [SerializeField, RequireInput]
        private SoftCurrencyPanel _softCurrencyPanel = default;

        [SerializeField, RequireInput]
        private FlightCoinFX _coinFX = default;

        [Space]
        [SerializeField, RequireInput]
        private TMP_Text _levelCompleteText = default;

        [SerializeField]
        private string _levelCompleteStr = "Level {0} completed!";

        [SerializeField, RequireInput]
        private TMP_Text _rewardValueText = default;

        [SerializeField, RequireInput]
        private Button _continueBtn = default;

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Fields]

        private int _completionReward;
        private bool _isActive;

        private LevelMetaData _metaData;

        #endregion

        private void Awake()
        {
            Hub.LoadLevel.Subscribe(_ =>
            {
                Toggle(false);
            }).AddTo(_ltt);

            Hub.LevelGenerationCompleted.Subscribe(metaData =>
            {
                _completionReward = metaData.LevelData.CompletionReward;
                _metaData = metaData;
            }).AddTo(_ltt);

            Hub.GameStarted.Subscribe(x =>
            {
                if (_isActive) Toggle(false);
            }).AddTo(_ltt);

            Hub.LevelComplete.Subscribe(_ => ShowAllocateRewards()).AddTo(_ltt);

            // Load next level when coin animation is done
            // Could be different, e.g. go to main menu instead
            _softCurrencyPanel.AnimationComplete
                              .Delay(TimeSpan.FromSeconds(_delayAfterFlightCompletion))
                              .Subscribe(_ => LoadingScreen.ShowLoadingScreen.Fire())
                              .AddTo(_ltt);

            _continueBtn.onClick.AddListener(OnContinueClick);

            // Ensure that awake on child component runs
            if (!_root.activeSelf) _root.SetActive(true);
            _root.SetActive(false);
        }

        private void SetupInitialState()
        {
            _continueBtn.interactable = true;
            LevelData levelData = _metaData.LevelData;

            _levelCompleteText.text = string.Format(_levelCompleteStr, (_metaData.VisualLevelIndex + 1).CachedString());
            _rewardValueText.text = $"+{levelData.CompletionReward.CachedString()}";
            _softCurrencyPanel.SetValue(PlayerProfile.SoftCurrency - _completionReward);
        }

        private void OnContinueClick()
        {
            if (!_isActive) return;

            _continueBtn.interactable = false;
            _isActive = false;

            _coinFX.DoAnimation();
            _softCurrencyPanel.AnimateValuesToProfile(_completionReward);
        }

        private void ShowAllocateRewards()
        {
            PlayerProfile.SoftCurrency += _completionReward;
            Toggle(true);
        }

        private void Toggle(bool state)
        {
            if (state)
            {
                SetupInitialState();
                _fadeTweener.DoB();
            }
            else
            {
                _fadeTweener.DoA();
            }

            _isActive = state;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        }
#endif
    }
}
