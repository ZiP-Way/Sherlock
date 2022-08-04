using Data;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class HandsAnimationsController : MonoBehaviour
{
    [SerializeField, HideInInspector] private Animator _animator = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Fields"

    private HandsAnimatorHashCodes _hashCodes = new HandsAnimatorHashCodes();
    private bool _isActive = true;

    #endregion

    private void Awake()
    {
        Hub.LevelGenerationCompleted.Subscribe(_ =>
        {
            _isActive = true;
        }).AddTo(_ltt);

        ChoiceDetection.PlayerChose.Subscribe(_ =>
        {
            _isActive = false;
            _animator.SetBool(_hashCodes.isMoving, false);
        }).AddTo(_ltt);

        Joystick.PointerDown.Where(_ => _isActive).Subscribe(_ =>
        {
            _animator.SetBool(_hashCodes.isMoving, true);
        }).AddTo(_ltt);

        Joystick.PointerUp.Where(_ => _isActive).Subscribe(_ =>
        {
            _animator.SetBool(_hashCodes.isMoving, false);
        }).AddTo(_ltt);

        CopyDummyClothes.ShowNextItem.Subscribe(_ =>
        {
            _animator.SetTrigger(_hashCodes.DoSign);
        }).AddTo(_ltt);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _animator = GetComponent<Animator>();
    }
#endif
}
