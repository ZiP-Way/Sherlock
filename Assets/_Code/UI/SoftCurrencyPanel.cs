using System;
using Data;
using EditorExtensions.Attributes;
using Profile;
using SignalsFramework;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;
using Utility;

namespace UI {
   /// <summary>
   /// Soft Currency display logic.
   /// </summary>
   /// <remarks>Use AnimateValuesToProfile to interpolate values on win screens, etc.</remarks>
   public class SoftCurrencyPanel : MonoBehaviour, IUpdatable {
      [FoldOutStart("Animation properties")]
      [SerializeField]
      private float _allocateDuration = 0.8f;

      [FoldOutEnd]
      [SerializeField]
      private float _delayBeforeAnim = 0.4f;

      [Space]
      [SerializeField]
      private bool _updateByDefault = true;
      
      [Space]
      [SerializeField, RequireInput]
      private TMP_Text _valueText = default;

      [SerializeField, HideInInspector]
      private ObservableDestroyTrigger _ltt = default;
      
      #region [Signals]
      
      /// <summary>
      /// Fired when value animation interpolation is complete
      /// </summary>
      public readonly Subject<Unit> AnimationComplete = new Subject<Unit>();
      
      #endregion

      #region [Fields]

      private float _startTime;

      private long _startCurrency;
      private long _endCurrency;

      private IDisposable _sub;

      #endregion

      private void Awake() {
         // Required, since subscription is not always active and cheats needs to update values correctly
         Hub.ForceRefreshSoftCurrency.Subscribe(_ => SetValue(PlayerProfile.SoftCurrency)).AddTo(_ltt);
         
         SubscribeToValueChange();
      }

      private void SubscribeToValueChange() {
         if (!_updateByDefault) return;
         
         if (_sub != null) return;
         _sub = Hub.SoftCurrencyChanged.Subscribe(SetValue).AddTo(_ltt);
      }

      private void OnEnable() {
         if (!_updateByDefault) return;
         
         SetValue(PlayerProfile.SoftCurrency);
      }

      public void SetValue(long softCurrency) {
         _valueText.text = softCurrency.CachedString();
      }

      public void AnimateValuesToProfile(long currencyAdded) {
         _startTime = Time.time + _delayBeforeAnim;

         _endCurrency =  PlayerProfile.SoftCurrency;
         _startCurrency = _endCurrency - currencyAdded;
         
         this.StartUpdate();
      }

      public void OnSystemUpdate(float dt) {
         Profiler.BeginSample("SoftCurrencyPanel:: OnSystemUpdate");
         
         DoAllocateAnimation();
         
         Profiler.EndSample();
      }

      private void DoAllocateAnimation() {
         float percentage = (Time.time - _startTime) / _allocateDuration;
         long value = (long) Mathf.Lerp(_startCurrency, _endCurrency, percentage);
         
         _valueText.text = value.CachedString();

         if (percentage >= 1f) {
            OnAnimComplete();
         }
      }

      private void OnAnimComplete() {
         this.StopUpdate();
         AnimationComplete.Fire();
      }

#if UNITY_EDITOR
      protected virtual void OnValidate() {
         if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
      }
#endif
   }
}