using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class SimilarDummiesDetection : MonoBehaviour
{
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<DummyClothes> DummyClothesGenerated = new Subject<DummyClothes>();

    #endregion

    #region "Fields"

    private List<DummyClothes> _dummiesClothes = new List<DummyClothes>();

    #endregion


    private void Awake()
    {
        SimilarDummiesDetection.DummyClothesGenerated.Subscribe(generatedDummyClothes =>
        {
            if (_dummiesClothes.Count == 0)
            {
                _dummiesClothes.Add(generatedDummyClothes);
                return;
            }

            for (int i = 0; i < _dummiesClothes.Count; i++)
            {
                if (DummiesItemsIsEqual(_dummiesClothes[i], generatedDummyClothes))
                {
                    Debug.Log("DummiesItemsIsEqual");

                    generatedDummyClothes.SetRandomClothing();
                    return;
                }
            }

            _dummiesClothes.Add(generatedDummyClothes);
        }).AddTo(_ltt);
    }

    private bool DummiesItemsIsEqual(DummyClothes firstDummy, DummyClothes secondDummy)
    {
        for (int i = 0; i < firstDummy.ItemsHashCodes.Count; i++)
        {
            if (firstDummy.ItemsHashCodes[i] != secondDummy.ItemsHashCodes[i])
            {
                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
    }
#endif
}
