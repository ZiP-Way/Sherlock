using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Controllers;
using UnityEngine;

public class Trace : MonoBehaviour
{
    [SerializeField, HideInInspector] private SplineController _splineController = default;

    public void Toggle(bool state)
    {
        if (gameObject.activeInHierarchy == state) return;

        gameObject.SetActive(state);
    }

#if UNITY_EDITOR
    public void SetPositionOnSpline(CurvySpline spline, float pos)
    {
        _splineController.Spline = spline;
        _splineController.Position = pos;
    }

    private void OnValidate()
    {
        _splineController = GetComponent<SplineController>();
    }
#endif
}
