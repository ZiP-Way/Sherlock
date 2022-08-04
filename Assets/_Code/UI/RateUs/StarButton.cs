using EditorExtensions.Attributes;
using SignalsFramework;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.RateUs {
    public class StarButton : MonoBehaviour {
        [SerializeField, RequireInput]
        private GameObject _enabledGO;

        [SerializeField, RequireInput]
        private GameObject _disabledGO;

        [SerializeField, RootOnly]
        private Button _button = default;
        
        #region [Fields]

        private int _index;
        private Subject<int> _onPressed;

        private bool _state;
        
        #endregion

        private void Awake() {
            _button.onClick.AddListener(() => ToggleState(!_state, true));
        }

        public void Init(int index, Subject<int> onPressed) {
            _index = index;
            _onPressed = onPressed;
        }

        public void ToggleState(bool state, bool sendEvent = false) {
            _state = state;
            
            _enabledGO.SetActive(state);
            _disabledGO.SetActive(!state);

            if (sendEvent) {
                _onPressed?.Fire(_index);
            }
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            _button = GetComponent<Button>();
        }
#endif
    }
}
