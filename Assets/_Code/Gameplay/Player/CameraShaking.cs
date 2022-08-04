using Cinemachine;
using UnityEngine;
using UniRx;
using UpdateSys;

public class CameraShaking : MonoBehaviour, IUpdatable
{
    [Header("Shake Settings")]
    [SerializeField] private float _frequencyGain = 1f;
    [SerializeField] private float _amplitudeGain = 1f;
    [SerializeField] private float _duration = 1f;

    [SerializeField, HideInInspector] private CinemachineBasicMultiChannelPerlin _cinemachinePerlinChannel = default;

    #region "Fields"

    private float _startTime = 0;

    private float _from = 0;
    private float _to = 0;

    #endregion

    private void Awake()
    {
        _cinemachinePerlinChannel.m_FrequencyGain = _frequencyGain;
        _cinemachinePerlinChannel.m_AmplitudeGain = 0;

        Joystick.PointerDown.Subscribe(_ =>
        {
            _startTime = Time.time;

            _from = _cinemachinePerlinChannel.m_AmplitudeGain;
            _to = _amplitudeGain;

            this.StartUpdate();
        }).AddTo(this);

        Joystick.PointerUp.Subscribe(_ =>
        {
            _startTime = Time.time;

            _from = _cinemachinePerlinChannel.m_AmplitudeGain;
            _to = 0;

            this.StartUpdate();
        }).AddTo(this);
    }

    private void OnDisable()
    {
        this.StopUpdate();
    }

    public void OnSystemUpdate(float deltaTime)
    {
        float percentage = (Time.time - _startTime) / _duration;
        _cinemachinePerlinChannel.m_AmplitudeGain = Mathf.Lerp(_from, _to, percentage);

        if (percentage > 1)
        {
            this.StopUpdate();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _cinemachinePerlinChannel = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
#endif
}
