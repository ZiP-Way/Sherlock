using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utility;
using Data;

public class SmokeFX : MonoBehaviour
{
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            Toggle(false);
        }).AddTo(_ltt);
    }

    public void Toggle(bool state)
    {
        if (state == gameObject.activeInHierarchy) return;

        gameObject.SetActive(state);
    }

    public void SetXLocalPosition(float x)
    {
        transform.localPosition = new Vector3(x,
            transform.localPosition.y,
            transform.localPosition.z);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
