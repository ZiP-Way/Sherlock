using SignalsFramework;
using UnityEngine;
using UniRx;

public interface IObstacle
{
    void Pass();
}

public class ObstacleDetection : MonoBehaviour
{
    #region "Signals"

    public static readonly Subject<Unit> PlayerColliderWithObject = new Subject<Unit>();

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IObstacle obstacle))
        {
            obstacle.Pass();
            ObstacleDetection.PlayerColliderWithObject.Fire();
        }
    }
}
