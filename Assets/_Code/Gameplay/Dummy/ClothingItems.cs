using UnityEngine;

public class ClothingItems : MonoBehaviour
{
    [SerializeField, HideInInspector] private ClothingItem[] _items = default;

    private void Awake()
    {
        DisableAllItems();
    }

    public ClothingItem GenerateRandomItem()
    {
        int randomId = Random.Range(0, _items.Length);

        ClothingItem item = _items[randomId];
        item.Toggle(true);

        return item;
    }

    public void SetItem(int itemHashCode)
    {
        foreach(ClothingItem item in _items)
        {
            if(itemHashCode == item.HashCode)
            {
                item.Toggle(true);
                break;
            }
        }
    }

    public void DisableAllItems()
    {
        foreach (var item in _items)
        {
            item.Toggle(false);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _items = GetComponentsInChildren<ClothingItem>(true);
    }
#endif
}
