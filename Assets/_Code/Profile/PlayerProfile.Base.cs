using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Profile.Surrogates;
using UniRx;
using UnityEngine;

namespace Profile {
    /// <summary>
    /// Player data storage logic and serialization.
    /// 
    /// Data is written to the Application.persistentDataPath using BinaryFormatter,
    /// and loaded before scene logic is applied. So its safe to access it anytime other
    /// than RuntimeOnLoad initialization time.
    /// </summary>
    /// <remarks>This data can be deleted via Tools menu</remarks>
    public static partial class PlayerProfile {
        #region [Properties]

        public const string DefaultName = "Player";
        public const int MaxNameLength = 10;

        public static string FullPath {
            get {
                if (_fullPath != null) return _fullPath;
                
                string path = Path.Combine(Application.persistentDataPath, DirPath);
                path = Path.Combine(path, FileName);

                _fullPath = path;

                return _fullPath;
            }
        }

        private static string _fullPath;

        public static string DirPath => _dirPath ??= Path.Combine(Application.persistentDataPath, DirName);

        private static string _dirPath;

        private const string DirName = "Data";
        private const string FileName = "PlayerData.dat";

        #endregion

        #region [Fields -- Generic]

        private static bool _loadedPreviously;
        private static bool _savedThisFrame;

        private static PlayerData _data;
        private static readonly BinaryFormatter Bf = new BinaryFormatter();
        
        public static readonly ColorSerializationSurrogate ColorSS = new ColorSerializationSurrogate();
        public static readonly SurrogateSelector Selector = new SurrogateSelector();

        #endregion
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            Selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), ColorSS);
            Bf.SurrogateSelector = Selector;
            
            LoadData();
            PostProcessLoadedData();
            HandleVersioning();
            
            _loadedPreviously = true;

            MainThreadDispatcher.StartUpdateMicroCoroutine(FrameReset());
            Observable.EveryApplicationFocus()
                      .Subscribe(isFocused => {
                                     if (!isFocused) SaveData();
                                 });

            Observable.EveryApplicationPause()
                      .Subscribe(isPaused => {
                                     if (isPaused) SaveData();
                                 });
        }

        #region [Save / Load]

        private static IEnumerator FrameReset() {
            while (true) {
                _savedThisFrame = false;
                yield return null;
            }
        }

        private static void LoadData() {
            if (!File.Exists(FullPath)) {
                _data = new PlayerData();
                return;
            }

            FileStream fs = null;
            try {
                fs = new FileStream(FullPath, FileMode.Open);
                _data = (PlayerData) Bf.Deserialize(fs);
            } catch (Exception ex) {
                Debug.LogWarning("PlayerProfile:: Failed to load player data (" + ex.Message + "). Using new instance");
            } finally {
                _data ??= new PlayerData();

                try {
                    fs?.Close();
                } catch (Exception ex) {
                    // Ignore IO exceptions
                    Debug.LogWarning("PlayerProfile:: Failed to load data stream. Reason: "+ex.Message);
                }
            }
        }

        public static void SaveData() {
            if (!_loadedPreviously) return;
            if (_savedThisFrame) return;

            Directory.CreateDirectory(DirPath);

            FileStream fs = null;
            try {
                fs = new FileStream(FullPath, FileMode.Create);
                Bf.Serialize(fs, _data);
            } catch (Exception ex) {
                Debug.LogWarning("PlayerProfile:: Failed to save player data: " + ex.Message);
            } finally {
                try {
                    fs?.Close();
                } catch (Exception ex) {
                    // Ignore IO exceptions
                    Debug.LogWarning("PlayerProfile:: Failed to save data. Reason: "+ex.Message);
                }
            }

            _savedThisFrame = true;
        }

        #endregion
    }
}