using Data;
using Data.Levels;
using SignalsFramework;
using TMPro;
using UI.ProgressBar;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class DummyPhoto : MonoBehaviour
{
    [SerializeField] private string _currentItemString = "{0}/{1}";

    [SerializeField] private TMP_Text _currentItemText = default;
    [SerializeField] private ProgressBarUI _progressBar = default;

    [SerializeField] private GameObject _progressRoot = default;

    [SerializeField] private ParticleSystem _portraitPlaceParticleSystem = default;

    [SerializeField] private Image _portraitImage = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Properties"

    private DummyItem[] _dummyItems = default;

    private bool _isAllUnlocked = false;

    private float _valueOfOneTrace = 0f;
    private int _currentItemCount = -1;
    private int _countOfCollectedTraces = 0;

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(metaData =>
        {
            Initialization(metaData);
            _isAllUnlocked = false;
        }).AddTo(_ltt);

        FlightNoteBook.Complete.Where(_ => !_isAllUnlocked).Subscribe(_ =>
        {
            _portraitPlaceParticleSystem.Play();

            _countOfCollectedTraces++;
            ChangeProgress(_countOfCollectedTraces);
        }).AddTo(_ltt);

        ObstacleDetection.PlayerColliderWithObject.Where(_ => !_isAllUnlocked).Subscribe(_ =>
        {
            if (_countOfCollectedTraces <= 0) return;

            _countOfCollectedTraces--;
            ChangeProgress(_countOfCollectedTraces);

            TraceUI.Show.Fire();
        }).AddTo(_ltt);
    }

    private void Initialization(LevelMetaData metaData)
    {
        _progressRoot.SetActive(true);

        _dummyItems = metaData.LevelData.DummyItems;
        _countOfCollectedTraces = 0;

        _currentItemCount = 0;
        _valueOfOneTrace = 1.0f / _dummyItems[_currentItemCount].TargetTraceCount;

        _portraitImage.gameObject.SetActive(true);
        _progressBar.ProgressChanged(0, true);
        _currentItemText.text = string.Format(_currentItemString, _currentItemCount, _dummyItems.Length);
    }

    private void ChangeProgress(int countOfCollectedTraces)
    {
        _progressBar.ProgressChanged(_valueOfOneTrace * countOfCollectedTraces);

        if (countOfCollectedTraces >= _dummyItems[_currentItemCount].TargetTraceCount)
        {
            _currentItemCount++;
            _countOfCollectedTraces = 0;

            CopyDummyClothes.ShowNextItem.Fire();

            if (_currentItemCount != _dummyItems.Length)
                _valueOfOneTrace = 1.0f / _dummyItems[_currentItemCount].TargetTraceCount;

            _currentItemText.text = string.Format(_currentItemString, _currentItemCount, _dummyItems.Length);

            if (_currentItemCount == 1) _portraitImage.gameObject.SetActive(false);

            if (_currentItemCount >= _dummyItems.Length)
            {
                _isAllUnlocked = true;
                DisableProgress();
            }
        }
    }

    private void DisableProgress()
    {
        _currentItemCount = -1;
        _progressRoot.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}