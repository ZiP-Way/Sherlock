using Data;
using Data.Levels;
using EditorExtensions.Attributes;
using Profile;
using TMPro;
using Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

namespace UI.Screens
{
    /// <summary>
    /// Game UI screen logic
    /// </summary>
    public class GameScreen : MonoBehaviour
    {
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [SerializeField, RequireInput]
        private ABTweener _fadeTweener = default;

        [SerializeField, RequireInput]
        private SoftCurrencyPanel _softCurrencyPanel = default;

        [SerializeField, RequireInput]
        private TMP_Text _currentLevelTxt = default;

        [SerializeField]
        private string _levelCompleteStr = "Level {0}";

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Fields]

        private bool _isActive;
        private LevelMetaData _metaData = default;

        #endregion

        private void Awake()
        {
            Hub.LoadLevel.Subscribe(_ =>
            {
                Toggle(false);
            }).AddTo(_ltt);

            Hub.GameStarted.Subscribe(_ =>
            {
                ResetToInitialState();
                Toggle(true);
            }).AddTo(_ltt);

            Hub.LevelGenerationCompleted.Subscribe(metaData =>
            {
                _metaData = metaData;
            }).AddTo(_ltt);

            Hub.LevelComplete.Subscribe(_ => Toggle(false)).AddTo(_ltt);
            Hub.LevelFailed.Subscribe(_ => Toggle(false)).AddTo(_ltt);

            // Ensure that awake on child component runs
            if (!_root.activeSelf) _root.SetActive(true);
            _root.SetActive(false);
        }

        private void Start()
        {
            Toggle(false);
        }

        private void ResetToInitialState()
        {
            _softCurrencyPanel.SetValue(PlayerProfile.SoftCurrency);
            _currentLevelTxt.text = string.Format(_levelCompleteStr, (_metaData.VisualLevelIndex + 1).CachedString());
        }

        private void Toggle(bool state)
        {
            _isActive = state;

            if (state)
            {
                _fadeTweener.DoB();
            }
            else
            {
                _fadeTweener.DoA();
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
            if (_softCurrencyPanel == null) _softCurrencyPanel = GetComponentInChildren<SoftCurrencyPanel>(true);
        }
#endif
    }
}
