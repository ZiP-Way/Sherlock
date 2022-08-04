using DG.Tweening;
using EditorExtensions.Attributes;
using UnityEngine;

namespace UI.Utility {
   /// <summary>
   /// Changes DoTween tweeners paused state based on OnEnable / OnDisable
   /// </summary>
   public class PauseTweenersOnState : MonoBehaviour {
      [SerializeField, RequireInput]
      private DOTweenAnimation _animation = default;

      private void OnEnable() { _animation.DOPlay(); }

      private void OnDisable() { _animation.DOPause(); }

#if UNITY_EDITOR
      protected virtual void OnValidate() { TryGetComponent(out _animation); }
#endif
   }
}