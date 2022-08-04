using UnityEngine;

public class ClothingItem : MonoBehaviour
{
    #region "Properties"

    public int HashCode => this.gameObject.name.GetHashCode();

    #endregion

    public void Toggle(bool state)
    {
        if (state == gameObject.activeInHierarchy) return;
        gameObject.SetActive(state);
    }
}
