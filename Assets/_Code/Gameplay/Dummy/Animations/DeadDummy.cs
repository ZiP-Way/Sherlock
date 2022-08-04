using UnityEngine;

public class DeadDummy : MonoBehaviour
{
    [SerializeField, HideInInspector] private Animator _animator = default;

    #region "Fields"

    private DummyAnimatorHashCodes _hashCodes = new DummyAnimatorHashCodes();

    #endregion

    private void Awake()
    {
        _animator.Play(_hashCodes.DeadAnimation);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }
#endif
}
