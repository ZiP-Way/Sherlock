using Data;
using Data.Levels;
using EditorExtensions.Attributes;
using Profile;
using SignalsFramework;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Screens
{
    /// <summary>
    /// Lobby Screen display logic
    /// </summary>
    public class LobbyScreen : MonoBehaviour
    {
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [SerializeField, RequireInput]
        private Button _playBtn = default;

        [SerializeField, RequireInput]
        private SoftCurrencyPanel _softCurrencyPanel = default;

        [SerializeField, RequireInput]
        private GameObject _overlayClickBlocker = default;

        [SerializeField, RequireInput]
        private TMP_Text _currentLevelTxt = default;

        [SerializeField]
        private string _levelCompleteStr = "Level {0}";

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Fields]

        private bool _startGameRequested;
        private bool _isActive;
        private LevelMetaData _metaData = default;

        #endregion

        private void Awake()
        {
            Hub.LevelGenerationCompleted.Subscribe(metaData =>
            {
                _metaData = metaData;
                _currentLevelTxt.text = string.Format(_levelCompleteStr, (_metaData.VisualLevelIndex + 1).CachedString());
            }).AddTo(_ltt);

            Hub.LoadLevel.Subscribe(_ =>
            {
                ResetToInitialState();
                Toggle(true);
            }).AddTo(_ltt);

            Hub.GameStarted.Subscribe(_ =>
            {
                _overlayClickBlocker.SetActive(true);
                _startGameRequested = true;

                Toggle(false);
            }).AddTo(_ltt);

            _playBtn.onClick.AddListener(OnPlayBtnClicked);

            _overlayClickBlocker.SetActive(false);
            Toggle(true);
        }

        private void ResetToInitialState()
        {
            _startGameRequested = false;
            _overlayClickBlocker.SetActive(false);

            // Feel free to expand if anything is required
        }

        private void OnPlayBtnClicked()
        {
            if (_startGameRequested) return;
            if (!_isActive) return;

            Hub.GameStarted.Fire();
        }

        private void Toggle(bool state)
        {
            if (_isActive == state) return;

            _root.SetActive(state);
            _isActive = state;

            if (state)
                _softCurrencyPanel.SetValue(PlayerProfile.SoftCurrency);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        }
#endif
    }
}