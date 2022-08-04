using UnityEngine;

public class Obstacle : MonoBehaviour, IObstacle
{
    public void Pass()
    {

    }

    private void Toggle(bool state)
    {
        if (state == gameObject.activeInHierarchy) return;
        gameObject.SetActive(state);
    }
}
