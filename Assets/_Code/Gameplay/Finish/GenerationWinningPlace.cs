using Data;
using SignalsFramework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class GenerationWinningPlace : MonoBehaviour
{
    [SerializeField] private Camera _trueDummyCamera = default;

    [SerializeField, HideInInspector] private Place[] _places = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    private void Awake()
    {
        Hub.GameStarted.Subscribe(_ =>
        {
            SetRandomWinningPlace();
        }).AddTo(_ltt);
    }

    private void SetRandomWinningPlace()
    {
        int randomIndex = Random.Range(0, _places.Length);
        _places[randomIndex].IsWinningPlace = true;
        _trueDummyCamera.transform.localPosition= new Vector3(_places[randomIndex].transform.localPosition.x,
            _trueDummyCamera.transform.localPosition.y,
            _trueDummyCamera.transform.localPosition.z);

        CopyDummyClothes.Initialization.Fire(_places[randomIndex].GetComponentInChildren<DummyClothes>().ItemsHashCodes);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _places = GetComponentsInChildren<Place>();
    }
#endif
}
