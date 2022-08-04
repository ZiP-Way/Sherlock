using Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSys;
using Utility;

public class PlayerGravity : MonoBehaviour, IUpdatable
{
    [SerializeField] private float _gravityForce = 20f;
    [SerializeField] private CharacterController _characterController = default;

    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    private float _currentGravityForce = 0f;

    #endregion

    private void Awake()
    {
        Hub.LevelComplete.Subscribe(_ => this.StopUpdate()).AddTo(_ltt);
        Hub.LevelFailed.Subscribe(_ => this.StopUpdate()).AddTo(_ltt);

        LevelRoad.SpawnPlayer.Subscribe(spawnPointPosition =>
        {
            this.StartUpdate();
        }).AddTo(_ltt);
    }

    public void OnSystemUpdate(float deltaTime)
    {
        Profiler.BeginSample("Uptade :: Player gravity");

        if (!_characterController.isGrounded) _currentGravityForce -= _gravityForce * deltaTime;
        else _currentGravityForce = 0;

        _characterController.SimpleMove(new Vector3(0, _currentGravityForce, 0));

        Profiler.EndSample();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _characterController = GetComponent<CharacterController>();
    }
#endif
}
