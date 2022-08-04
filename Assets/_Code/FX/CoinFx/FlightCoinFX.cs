using System.Collections.Generic;
using Data;
using EditorExtensions.Attributes;
using Pooling;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;
using Utility;

namespace FX.CoinFX
{
    public class FlightCoinFX : MonoBehaviour, IUpdatable
    {
        [Header("Coin flight animation")]
        [SerializeField]
        private float _durationPerCoin = 0.3f;

        [SerializeField]
        private float _delayBetweenSpawns = 0.1f;

        [SerializeField]
        private float _rndDelay = 0.02f;

        [SerializeField]
        private float _coinsToSpawn = 20;

        [SerializeField]
        private int _coinSpawnBatch = 5;

        [SerializeField]
        private Vector2 _randomizationOffset = new Vector2(100, 100);

        [SerializeField, RequireInput]
        private GameObject _coinPrefab = default;

        [SerializeField, RequireInput]
        private Transform _coinStart = default;

        [SerializeField, RequireInput]
        private Transform _coinDestination = default;

        [SerializeField, RequireInput]
        private Transform _attachToTrm = default;

        [SerializeField, HideInInspector]
        private ObservableDestroyTrigger _ltt = default;

        #region [Signals]

        public readonly Subject<Unit> Complete = new Subject<Unit>();

        #endregion

        #region [Fields]

        private readonly HashSet<FlightCoin> _coins = new HashSet<FlightCoin>();
        private int _spawnedCoins;

        private float _delayTimer;

        #endregion

        private void Awake()
        {
            Hub.Cleanup.Subscribe(_ => DecommissionCoins()).AddTo(_ltt);
        }

        private void OnDisable()
        {
            DecommissionCoins();
            this.StopUpdate();
        }

        private void DecommissionCoins()
        {
            foreach (FlightCoin coin in _coins)
            {
                coin.Decommission();
            }

            _coins.Clear();
        }

        public void DoAnimation()
        {
            _coins.Clear();
            _spawnedCoins = 0;

            this.StartUpdate();
        }

        public void OnSystemUpdate(float dt)
        {
            Profiler.BeginSample("FlightCoin:: OnSystemUpdate");

            _delayTimer -= Time.deltaTime;
            if (_delayTimer > 0)
            {
                Profiler.EndSample();
                return;
            }

            SpawnMoveCoins();

            if (_spawnedCoins >= _coinsToSpawn)
            {
                OnComplete();
            }

            Profiler.EndSample();
        }

        private void SpawnMoveCoins()
        {
            for (int i = 0; i < _coinSpawnBatch && _spawnedCoins < _coinsToSpawn; i++)
            {
                FlightCoin coin = _coinPrefab.Pool<FlightCoin>();
                coin.AttachTo(_attachToTrm);

                Vector3 pos = _coinStart.position;
                pos.x += Random.Range(-_randomizationOffset.x, _randomizationOffset.x);
                pos.y += Random.Range(-_randomizationOffset.y, _randomizationOffset.y);

                coin.SetPosition(pos);

                coin.DoAnimation(pos, _coinDestination, _durationPerCoin);

                _coins.Add(coin);

                _spawnedCoins++;
                _delayTimer = _delayBetweenSpawns + Random.Range(-_rndDelay, _rndDelay);
            }
        }

        private void OnComplete()
        {
            this.StopUpdate();
            Complete.Fire();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        }
#endif
    }
}