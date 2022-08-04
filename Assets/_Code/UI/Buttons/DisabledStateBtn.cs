using EditorExtensions.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons {
    /// <summary>
    /// Displays an extra state when button is considered "disabled"
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DisabledStateBtn : MonoBehaviour {
        [SerializeField]
        private GameObject _offState = default;

        [SerializeField, RootOnly, RequireInput]
        private Button _button = default;
        
        #region [Properties]

        public Button.ButtonClickedEvent OnClick => _button.onClick;
        
        #endregion
        
        public void SetState(bool state) {
            _offState.SetActive(!state);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            TryGetComponent(out _button);
        }
#endif
    }
}
