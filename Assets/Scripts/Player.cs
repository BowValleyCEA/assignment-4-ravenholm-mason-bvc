using UnityEngine;

public class Player : MonoBehaviour
{
    public const string OUTER_HEAD_NAME = "OuterHead";
    public const string INNER_HEAD_NAME = "InnerHead";

    protected struct RigidbodyProperties
    {
        public CollisionDetectionMode CollisionDetectionMode;
        public bool UseGravity;

        public readonly void ApplyTo(Rigidbody rigidbody)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode;
            rigidbody.useGravity = UseGravity;
        }

        public readonly void RemoveFrom(Rigidbody rigidbody)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.useGravity = false;
        }
    }

    private readonly RaycastHit[] _raycastHitResults = new RaycastHit[32];
    private float _forwardAxis;
    private float _strafeAxis;
    private bool _queuedJump;
    private CharacterController _characterController;
    private Transform _outerHead;
    private Transform _innerHead;
    private RigidbodyProperties _currentlyHeldRigidbodyProperties;
    private Rigidbody _currentlyHeldRigidbody;
    private Collider _currentlyHeldRigidbodyCollider;

    public Vector3 Velocity;

    [SerializeField]
    public float WalkSpeed = 20.0F;

    [SerializeField]
    public float JumpStrength = 15.0F;

    [SerializeField]
    public float DecelerationRate = 5.0F;

    [SerializeField]
    public float FallRate = 1.0F;

    [SerializeField]
    public float AbsoluteTerminalVelocity = 40.0F;

    [SerializeField]
    public Vector3 FloorNormal = Vector3.up;

    [SerializeField]
    public float MaximumFloorAngleDegrees = 45.0F;

    [SerializeField]
    public float MaximumRaycastDistance = 4.0F;

    [SerializeField]
    public float GravityGunForce = 10.0F;

    [SerializeField]
    public float HoldDistance = 2.0F;

    public bool IsOnFloor { get; protected set; }

    #region Engine Messages

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _characterController = GetComponent<CharacterController>();
        _outerHead = transform.Find(OUTER_HEAD_NAME);
        _innerHead = _outerHead.Find(INNER_HEAD_NAME);
    }

    public void FixedUpdate()
    {
        // You could add && !IsOnFloor to this, but it would actually screw up
        // the floor check for the next frame. We technically want to actually
        // move down by some small amount every single frame.
        if (Velocity.y > -AbsoluteTerminalVelocity)
        {
            Velocity.y -= FallRate;
        }

        Vector3 moveVector = new();

        moveVector += _outerHead.transform.forward * _forwardAxis;
        moveVector += _outerHead.transform.right * _strafeAxis;
        moveVector.Normalize();
        moveVector *= WalkSpeed;
        Velocity += moveVector * Time.fixedDeltaTime;
        _characterController.Move(Velocity * Time.fixedDeltaTime);

        if (_queuedJump && IsOnFloor)
        {
            Velocity.y = JumpStrength;
        }

        _queuedJump = false;
        IsOnFloor = false;

        var scaledDeceleration = DecelerationRate * Time.fixedDeltaTime;

        Velocity.x = Mathf.Lerp(Velocity.x, 0, scaledDeceleration);
        Velocity.z = Mathf.Lerp(Velocity.z, 0, scaledDeceleration);

        if (_currentlyHeldRigidbody)
        {
            _currentlyHeldRigidbody.Move(_innerHead.position + _innerHead.forward * HoldDistance, Quaternion.identity);
        }
    }

    public void Update()
    {
        _forwardAxis = Input.GetAxisRaw("Vertical");
        _strafeAxis = Input.GetAxisRaw("Horizontal");
        _queuedJump |= Input.GetButtonDown("Jump");

        var yawDelta = Input.GetAxis("Mouse X");
        var pitchDelta = Input.GetAxis("Mouse Y");

        _outerHead.Rotate(Vector3.up, yawDelta);
        _innerHead.Rotate(Vector3.left, pitchDelta);

        if (Input.GetButtonDown("Fire2"))
        {
            if (!_currentlyHeldRigidbody)
            {
                MaybePickUpRigidbody();
            }
            else
            {
                LetGoOfCurrentlyHeldRigidbody();
            }
        }

        if (Input.GetButtonDown("Fire1") && _currentlyHeldRigidbody)
        {
            FlingCurrentlyHeldRigidbody();
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider)
        {
            IsOnFloor = Mathf.Acos(Vector3.Dot(hit.normal, FloorNormal)) < MaximumFloorAngleDegrees;
            Velocity = Vector3.ProjectOnPlane(Velocity, hit.normal);

            if (_currentlyHeldRigidbody && hit.collider == _currentlyHeldRigidbodyCollider)
            {
                LetGoOfCurrentlyHeldRigidbody();
            }
        }
    }

    #endregion Engine Messages

    public void MaybePickUpRigidbody()
    {
        var raycastResultCount = Physics.RaycastNonAlloc(
            new Ray(_innerHead.position, _innerHead.forward),
            _raycastHitResults,
            MaximumRaycastDistance,
            LayerMask.GetMask(Globals.LAYER_DYNAMIC));

        if (raycastResultCount > 0)
        {
            _currentlyHeldRigidbody = _raycastHitResults[0].transform.GetComponent<Rigidbody>();
            _currentlyHeldRigidbodyCollider = _currentlyHeldRigidbody.GetComponent<Collider>();

            _currentlyHeldRigidbodyProperties = new RigidbodyProperties
            {
                CollisionDetectionMode = _currentlyHeldRigidbody.collisionDetectionMode,
                UseGravity = _currentlyHeldRigidbody.useGravity,
            };

            _currentlyHeldRigidbodyProperties.RemoveFrom(_currentlyHeldRigidbody);
        }
    }

    public void LetGoOfCurrentlyHeldRigidbody()
    {
        _currentlyHeldRigidbodyProperties.ApplyTo(_currentlyHeldRigidbody);
        _currentlyHeldRigidbody = null;
    }

    public void FlingCurrentlyHeldRigidbody()
    {
        _currentlyHeldRigidbody.AddForce(_innerHead.forward * GravityGunForce, ForceMode.Impulse);
        LetGoOfCurrentlyHeldRigidbody();
    }
}
