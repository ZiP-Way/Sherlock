using SignalsFramework;
using UnityEngine;

public class DummyAnimationsController : MonoBehaviour
{
    [SerializeField, HideInInspector] private Animator _animator = default;

    #region "Fields"

    private DummyAnimatorHashCodes _hashCodes = new DummyAnimatorHashCodes();

    private int _countOfIdleAnimations = 4;
    private int _countOfDiscontentAnimations = 3;

    #endregion

    private void Awake()
    {
        SetRandomIdleAnimation();
    }

    private void SetRandomIdleAnimation()
    {
        int randomIdleAnimationId = Random.Range(1, _countOfIdleAnimations + 1);
        _animator.SetInteger(_hashCodes.IdleAnimationId, randomIdleAnimationId);
    }

    private void SetDiscontentAnimation(int animationId)
    {
        _animator.SetInteger(_hashCodes.DiscontentAnimationId, animationId);
    }

    public void DoDiscontentedLight()
    {
        int randomIndex = Random.Range(2, _countOfDiscontentAnimations + 1);
        SetDiscontentAnimation(randomIndex);
        _animator.SetTrigger(_hashCodes.DoDiscontented);

        DummyCuffer.Cuff.Fire(randomIndex - 1);
    }

    public void DoDiscontentedHard()
    {
        SetDiscontentAnimation(1);
        _animator.SetTrigger(_hashCodes.DoDiscontented);

        DummyCuffer.Cuff.Fire(0);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }
#endif
}
