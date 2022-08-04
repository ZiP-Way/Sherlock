using System.Collections.Generic;
using Data;
using EditorExtensions.Attributes;
using Pooling;
using SignalsFramework;
using Tweening;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;

namespace FX.CoinFX
{
    public class FlightCoin : MonoBehaviour, IGenericPoolElement, IUpdatable
    {
        [SerializeField]
        private float _durationRnd = 0.02f;

        [SerializeField]
        private float _fadeInDelay = 0.1f;

        [FoldOutStart("Components")]
        [SerializeField, RootOnly]
        private GameObject _gameObject = default;

        [FoldOutEnd]
        [SerializeField, RootOnly]
        private Transform _transform = default;

        [SerializeField]
        private List<ABTweener> _tweeners = new List<ABTweener>();

        #region [Fields]

        private Vector3 _initialPoint;
        private Transform _endPoint;
        private float _duration;
        private float _startTime;

        #endregion

        private void OnDisable() { this.StopUpdate(); }

        public void AttachTo(Transform parent)
        {
            _transform.SetParent(parent, false);
            _transform.localScale = Vector3.one;
        }

        public void SetPosition(Vector3 pos) { _transform.position = pos; }

        public void DoAnimation(Vector3 initialPoint, Transform endPoint, float duration)
        {
            _duration = duration + Random.Range(-_durationRnd, _durationRnd);
            _startTime = Time.time + _fadeInDelay;

            _initialPoint = initialPoint;
            _endPoint = endPoint;

            foreach (ABTweener tweener in _tweeners)
            {
                tweener.ForceA();
                tweener.DoB();
            }

            this.StartUpdate();
        }

        public void OnSystemUpdate(float dt)
        {
            Profiler.BeginSample("FlightCoin:: OnSystemUpdate");

            DoMovement();

            Profiler.EndSample();
        }

        private void DoMovement()
        {
            float percentage = (Time.time - _startTime) / _duration;
            _transform.position = Vector3.Lerp(_initialPoint, _endPoint.position, percentage);

            if (percentage >= 1f)
            {
                OnComplete();
            }
        }

        private void OnComplete()
        {
            Hub.CoinFlightComplete.Fire();

            this.StopUpdate();
            Decommission();
        }

        #region [IGenericPoolElement implementation]

        public int PoolRef { get; set; }
        public bool IsAvailable => false;
        public bool IsCommissioned { get; set; }
        public bool UsesAutoPool { get; set; }

        public void Commission() { _gameObject.SetActive(true); }

        public void Decommission()
        {
            if (!IsCommissioned) return;

            _gameObject.SetActive(false);

            this.ReturnToPool();
        }

        public void OnDestroy() { this.RemoveFromPool(); }

        #endregion

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            _gameObject = gameObject;
            _transform = transform;
        }
#endif
    }
}