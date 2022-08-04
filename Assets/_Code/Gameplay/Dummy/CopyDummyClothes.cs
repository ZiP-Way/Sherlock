using Data;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class CopyDummyClothes : MonoBehaviour
{
    [SerializeField] private ClothingItem[] _defaultClothingItems = default;

    [SerializeField, HideInInspector] private ClothingItems[] _clothingItems = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<List<int>> Initialization = new Subject<List<int>>();
    public static readonly Subject<Unit> ShowNextItem = new Subject<Unit>();

    #endregion

    #region "Fields"

    private List<int> _dummyClothesHashCodes = default;
    private int _currentElementId = -1;

    #endregion

    private void Awake()
    {
        CopyDummyClothes.Initialization.Subscribe(dummyClothesHashCodes =>
        {
            _dummyClothesHashCodes = dummyClothesHashCodes;
        }).AddTo(_ltt);

        CopyDummyClothes.ShowNextItem.Subscribe(_ =>
        {
            _currentElementId++;

            if (_currentElementId == 0)
            {
                foreach (var defaultItem in _defaultClothingItems)
                {
                    defaultItem.Toggle(true);
                }
            }
            else
            {
                if (_clothingItems.Length == _currentElementId)
                {
                    foreach (var defaultItem in _defaultClothingItems)
                    {
                        defaultItem.Toggle(false);
                    }
                }

                _clothingItems[_currentElementId - 1].SetItem(_dummyClothesHashCodes[_currentElementId - 1]);
            }
        }).AddTo(_ltt);

        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            _currentElementId = -1;
            _dummyClothesHashCodes = null;

            foreach (var item in _clothingItems)
            {
                item.DisableAllItems();
            }
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _clothingItems = GetComponentsInChildren<ClothingItems>();
    }
#endif
}
