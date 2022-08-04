using Data;
using Data.Levels;
using Data.RateUs;
using EditorExtensions.Attributes;
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
    /// Logic for the Fail Screen UI
    /// </summary>
    public class FailScreen : MonoBehaviour
    {
        [SerializeField, RequireInput]
        private GameObject _root = default;

        [Space]
        [SerializeField, RequireInput]
        private TMP_Text _levelFailedText = default;

        [SerializeField]
        private string _levelFailedStr = "Level {0} failed!";

        [SerializeField, RequireInput]
        private ABTweener _fadeTweener = default;

        [SerializeField, RequireInput]
        private Button _restartBtn = default;

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Fields]

        private bool _isActive;

        #endregion

        private void Awake()
        {
            Hub.LoadLevel.Subscribe(_ =>
            {
                if (_isActive) Toggle(false);
            }).AddTo(_ltt);

            Hub.LevelGenerationCompleted.Subscribe(metaData =>
            {
                SetupState(metaData);
            }).AddTo(_ltt);

            Hub.GameStarted.Subscribe(x =>
            {
                if (_isActive) Toggle(false);
            }).AddTo(_ltt);

            Hub.LevelFailed.Subscribe(_ =>
            {
                if (!_isActive) Toggle(true);
            }).AddTo(_ltt);

            _restartBtn.onClick.AddListener(GoToLobby);

            // Ensure that awake on child component runs
            if (!_root.activeSelf) _root.SetActive(true);
            _root.SetActive(false);
        }

        private void SetupState(LevelMetaData metaData)
        {
            _levelFailedText.text = string.Format(_levelFailedStr, (metaData.VisualLevelIndex + 1).CachedString());
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

        private void GoToLobby()
        {
            if (!_isActive) return;

            LoadingScreen.ShowLoadingScreen.Fire();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        }
#endif
    }
}