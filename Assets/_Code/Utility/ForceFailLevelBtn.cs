using Data;
using EditorExtensions.Attributes;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Utility {
   /// <summary>
   /// Example how to force a level failed state
   /// </summary>
   public class ForceFailLevelBtn : MonoBehaviour {
      [SerializeField, RequireInput]
      private Button _button = default;

      [SerializeField, HideInInspector]
      private ObservableDestroyTrigger _ltt = default;
      
      private void Awake() {
         Hub.LevelGenerationCompleted.Subscribe(_ => _button.interactable = true).AddTo(_ltt);
         _button.onClick.AddListener(OnFailLevelClick);
      }

      private void OnFailLevelClick() {
         _button.interactable = false;

         //Hub.LevelFailed.Fire();
      }

#if UNITY_EDITOR
      protected virtual void OnValidate() {
         _button = GetComponentInChildren<Button>(true);
         if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
      }
#endif
   }
}