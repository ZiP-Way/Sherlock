using System.Collections.Generic;
using Data;
using Data.Levels;
using EditorExtensions.Attributes;
using Profile;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Levels
{
    /// <summary>
    /// Example how to use signals for level generation.
    /// Can be removed or extended at will.
    /// </summary>
    public class LevelSpawner : MonoBehaviour
    {
        [SerializeField, RequireInput]
        private LevelList _levelList = default;

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        private int _currentLevel = -1;
        private bool _isRestart = false;

        private void Awake()
        {
            SceneManager.LoadSceneAsync("DummyPhotoScene", LoadSceneMode.Additive);
            Hub.LoadLevel.Subscribe(_ => UnLoadLevel()).AddTo(_ltt);

            Hub.LevelFailed.Subscribe(_ =>
            {
                _isRestart = true;
            }).AddTo(_ltt);

            Hub.LevelComplete.Subscribe(_ =>
            {
                _isRestart = false;
            }).AddTo(_ltt);
        }

        private void Start()
        {
            Hub.LoadLevel.Fire();
        }

        private void UnLoadLevel()
        {
            if (_currentLevel >= 0)
            {
                AsyncOperation levelUnload = SceneManager.UnloadSceneAsync("Level" + _currentLevel); // current level
                levelUnload.completed += x => GenerateLevel();
            }
            else
            {
                GenerateLevel();
            }
        }

        private void GenerateLevel()
        {
            int visualLevelIndex = PlayerProfile.CurrentLevel;
            int level = visualLevelIndex;

            List<LevelData> allLevels = _levelList.AllLevels;

            if (_isRestart)
            {
                level = _currentLevel;
            }
            else
            {
                level = level >= allLevels.Count ? GenerateRandomLevel(allLevels.Count) : level;
            }

            LevelData currentData = allLevels[level];
            _currentLevel = level;

            AsyncOperation levelLoad = SceneManager.LoadSceneAsync("Level" + level, LoadSceneMode.Additive); // level

            levelLoad.completed += x =>
            {
                Hub.LevelGenerationCompleted.Fire(new LevelMetaData
                {
                    LevelData = currentData,
                    ActualLevelIndex = level,
                    VisualLevelIndex = visualLevelIndex
                });
            };
        }

        private int GenerateRandomLevel(int levelCount)
        {
            int levelId = Random.Range(0, levelCount);
            if (levelId == _currentLevel)
            {
                if (levelId == 0)
                {
                    levelId++;
                }
                else if (levelId == levelCount - 1)
                {
                    levelId--;
                }
                else
                {
                    levelId++;
                }
            }
            return levelId;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        }
#endif
    }
}