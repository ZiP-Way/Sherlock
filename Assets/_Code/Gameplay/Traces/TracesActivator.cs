using SignalsFramework;
using UnityEngine;

public class TracesActivator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ShowingTracesAnimation _showingTracesAnimation = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            _showingTracesAnimation.Run.Fire();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            _showingTracesAnimation.Stop.Fire();
        }
    }
}