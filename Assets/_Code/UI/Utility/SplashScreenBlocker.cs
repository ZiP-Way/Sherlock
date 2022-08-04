using UnityEngine;
using UnityEngine.Rendering;
using UpdateSys;

namespace UI {
   public class SplashScreenBlocker : MonoBehaviour, IUpdatable {
      [SerializeField]
      private float _delayBeforeDisable = 0.1f;

      #region [Fields]

      private float _delayTimer;
      private bool _delayBeforeHiding;
      
      #endregion
      
      private void OnEnable() { this.StartUpdate(); }

      private void OnDisable() { this.StopUpdate(); }

      public void OnSystemUpdate(float deltaTime) {
         Input.ResetInputAxes();
         
         if (_delayBeforeHiding) {
            _delayTimer -= deltaTime;

            if (_delayTimer <= 0) {
               gameObject.SetActive(false);
            }

            return;
         }

         if (SplashScreen.isFinished) {
            _delayTimer = _delayBeforeDisable;
            _delayBeforeHiding = true;
         }
      }
   }
}
