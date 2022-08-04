using FluffyUnderware.Curvy.Controllers;
using UnityEngine;

public class MovingCar : MonoBehaviour, IObstacle
{
    [SerializeField, HideInInspector] private SplineController _splineController = default;

    #region "Fields"

    private bool _isPassed = false;

    #endregion

    public void Pass()
    {
        if (_isPassed) return;
        _isPassed = true;

        _splineController.MovementDirection =
            _splineController.MovementDirection == MovementDirection.Forward ? MovementDirection.Backward : MovementDirection.Forward;
    }

    public void OnEndPointReached()
    {
        if (_isPassed)
        {
            _splineController.Speed = 0;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _splineController = GetComponent<SplineController>();
    }
#endif
}
