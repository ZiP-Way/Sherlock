using Data;
using DG.Tweening;
using FluffyUnderware.Curvy.Controllers;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10f;

    [Header("Components")]
    [SerializeField] private ObjectDetection _rightObjectDetection = default;
    [SerializeField] private ObjectDetection _leftObjectDetection = default;

    [SerializeField] private ObjectDetection _forwardObjectDetection = default;
    [SerializeField] private ObjectDetection _backObjectDetection = default;

    [SerializeField] private RoadDetection _rightRoadDetection = default;
    [SerializeField] private RoadDetection _leftRoadDetection = default;

    [SerializeField, HideInInspector] private SplineController _splineController = default;
    [SerializeField, HideInInspector] private BoxCollider _boxCollider = default;
    [SerializeField, HideInInspector] private ObservableDestroyTrigger _ltt = default;

    #region "Signals"

    public static readonly Subject<Unit> StopMovement = new Subject<Unit>();

    #endregion

    #region "Fields"

    private Sequence _pushBackAnimation = default;

    private bool _isCanMoving = true;

    private bool _isCanMovingRight = true;
    private bool _isCanMovingLeft = true;

    private bool _isCanMovingForward = true;
    private bool _isCanMovingBack = true;

    private bool _isLeftSideOnGround = true;
    private bool _isRightSideOnGround = true;

    #endregion

    private void Awake()
    {
        _pushBackAnimation = DOTween.Sequence();
        BuildAnimation();

        _rightObjectDetection.IsCollidingWithObject.Subscribe(isCollidedWithObject => _isCanMovingRight = isCollidedWithObject).AddTo(_ltt);
        _leftObjectDetection.IsCollidingWithObject.Subscribe(isCollidedWithObject => _isCanMovingLeft = isCollidedWithObject).AddTo(_ltt);

        _forwardObjectDetection.IsCollidingWithObject.Subscribe(isCollidedWithObject => _isCanMovingForward = isCollidedWithObject).AddTo(_ltt);
        _backObjectDetection.IsCollidingWithObject.Subscribe(isCollidedWithObject => _isCanMovingBack = isCollidedWithObject).AddTo(_ltt);

        _rightRoadDetection.OnGrounded.Subscribe(isGrounded => _isRightSideOnGround = isGrounded).AddTo(_ltt);
        _leftRoadDetection.OnGrounded.Subscribe(isGrounded => _isLeftSideOnGround = isGrounded).AddTo(_ltt);

        PlayerMovement.StopMovement.Subscribe(_ =>
        {
            Stop();
        }).AddTo(_ltt);

        Hub.GameStarted.Subscribe(_ =>
        {
            _isCanMoving = true;
            _boxCollider.enabled = true;
        }).AddTo(_ltt);

        Joystick.Dragging.Where(_ => _isCanMoving).Subscribe(direction =>
        {
            MoveRightBack(direction);
            MoveForwardBack(direction);
        }).AddTo(_ltt);

        Joystick.PointerUp.Subscribe(_ =>
        {
            _splineController.Speed = 0;
        }).AddTo(_ltt);

        ObstacleDetection.PlayerColliderWithObject.Subscribe(_ =>
        {
            _pushBackAnimation.Restart();
        }).AddTo(this);

        Stop();
    }

    private void BuildAnimation()
    {
        _pushBackAnimation.Pause();
        _pushBackAnimation.SetAutoKill(false);

        _pushBackAnimation.OnPlay(() => _isCanMoving = false);
        _pushBackAnimation.AppendCallback(() => _splineController.MovementDirection = MovementDirection.Backward);
        _pushBackAnimation.Append(DOTween.To(() => _splineController.Speed, x => _splineController.Speed = x, 0, 1f));
        _pushBackAnimation.AppendCallback(() => _splineController.MovementDirection = MovementDirection.Forward);
        _pushBackAnimation.OnComplete(() => _isCanMoving = true);
    }

    private void MoveRightBack(Vector2 direction)
    {
        if (direction.x > 0 && !_isCanMovingRight)
        {
            return;
        }

        if (direction.x > 0 && !_isRightSideOnGround)
        {
            return;
        }

        if (direction.x < 0 && !_isCanMovingLeft)
        {
            return;
        }

        if (direction.x < 0 && !_isLeftSideOnGround)
        {
            return;
        }

        _splineController.OffsetRadius += -direction.x * Time.fixedDeltaTime * _movementSpeed;
    }

    private void MoveForwardBack(Vector3 direction)
    {
        if (direction.y < 0)
        {
            _splineController.MovementDirection = MovementDirection.Backward;
        }

        if (direction.y >= 0)
        {
            _splineController.MovementDirection = MovementDirection.Forward;
        }

        if (direction.y > 0 && !_isCanMovingForward)
        {
            direction.y = 0;
        }

        if (direction.y < 0 && !_isCanMovingBack)
        {
            direction.y = 0;
        }

        float currentSpeed = Mathf.Lerp(_splineController.Speed, Mathf.Abs(direction.y * _movementSpeed), 0.5f);
        _splineController.Speed = currentSpeed;
    }

    private void Stop()
    {
        _isCanMoving = false;
        _boxCollider.enabled = false;

        _splineController.Speed = 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_ltt == null) _ltt = gameObject.SetupDestroyTrigger();
        _splineController = GetComponent<SplineController>();
        _boxCollider = GetComponent<BoxCollider>();
    }
#endif
}