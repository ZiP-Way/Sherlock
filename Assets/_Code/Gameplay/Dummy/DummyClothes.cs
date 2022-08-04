using SignalsFramework;
using System.Collections.Generic;
using UnityEngine;

public class DummyClothes : MonoBehaviour
{
    [SerializeField] private bool _isMustBeIndividual = false;
    [SerializeField, HideInInspector] private ClothingItems[] _clothingItems = default;

    #region "Properties"

    public List<int> ItemsHashCodes => _dummyItemsHashCodes;

    #endregion

    #region "Fields"

    private List<int> _dummyItemsHashCodes = new List<int>();

    #endregion

    private void Start()
    {
        SetRandomClothing();
    }

    public void SetRandomClothing()
    {
        foreach (var item in _clothingItems)
        {
            _dummyItemsHashCodes.Add(item.GenerateRandomItem().HashCode);
        }

        if (_isMustBeIndividual)
            SimilarDummiesDetection.DummyClothesGenerated.Fire(this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _clothingItems = GetComponentsInChildren<ClothingItems>();
    }
#endif
}
