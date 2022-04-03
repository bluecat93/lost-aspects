using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // Eden ref: where do this 3 params get assigned?
    public CharacterController controller;
    public Transform playerCamera;

    float turnSmoothVelocity;

    #region serializable fields

    // y movement paramaters
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float dashModifier = 1.5f;


    // Eden Ref: please check if those viarables need to be on class level or can we move them to the relevant functions
    // Crouching variables, player height at start and collider position
    [SerializeField] private float playerStartHeight;
    [SerializeField] private float colliderStartHeight;
    [SerializeField] private float crouchColliderPositionY = 0.05f;
    [SerializeField] private float heightChange = 0.5f;

    [SerializeField] private float movementSpeed = 5.0f;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    // Dodging Variables
    [SerializeField] private float dodgeSpeed = 10f;
    [SerializeField] private float dodgeTime = 0.5f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool IsGrounded = true;
    [Tooltip("Useful for rough ground. If you are not sure, keep on 0")]
    public float GroundedOffset = 0.0f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    #endregion

    #region properties

    private Vector3 _velocity;

    private Vector3 Velocity
    {
        get
        {
            if (_velocity == null)
                _velocity = Vector3.zero;
            return _velocity;
        }
        set
        {
            _velocity = value;
        }
    }

    private float VerticalVelocity { get; set; }

    private Rigidbody _rgbody;

    private Rigidbody Rgbody
    {
        get
        {
            if (this._rgbody == null)
                this._rgbody = GetComponent<Rigidbody>();
            return this._rgbody;
        }
    }

    private Animator _playerAnim;

    private Animator PlayerAnim
    {
        get
        {
            if (this._playerAnim == null)
                this._playerAnim = GetComponentInChildren<Animator>();

            return this._playerAnim;
        }
    }

    private PlayerStats _playerStats;

    private PlayerStats PlyrStats
    {
        get
        {
            if (this._playerStats == null)
                this._playerStats = GetComponent<PlayerStats>();

            return this._playerStats;
        }
    }

    private CapsuleCollider _cpslCollider;

    private CapsuleCollider CpslCollider
    {
        get
        {
            if (this._cpslCollider == null)
                this._cpslCollider = GetComponent<CapsuleCollider>();
            return this._cpslCollider;
        }
    }

    private bool IsDashing
    {
        get
        {
            return Input.GetAxis("Dash") > 0;
        }
    }

    private bool IsExhausted
    {
        get
        {
            return this.PlyrStats.currentStamina <= 0;
        }
    }

    private bool IsMoving
    {
        get
        {
            return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        }
    }

    public bool IsCrouching { get; set; }

    public bool IsRolling { get; set; }

    // public bool IsGrounded
    // {
    //     get
    //     {
    //         return Physics.Raycast(transform.position, Vector3.down, 0.2f);
    //     }
    // }


    public bool IsJumping
    {
        get
        {
            return Input.GetAxis("Jump") > 0;
        }
    }

    private bool IsClimbable { get; set; }

    private float GroundAngle { get; set; }

    private Vector3 MoveDirection { get; set; }

    private float CapsuleColliderStartingHeight
    {
        get
        {
            return this.CpslCollider.height;
        }
    }
    private float OriginalStepOffset { get; set; }
    private float AnimationBlend { get; set; }
    private float TargetRotation { get; set; }
    // TODO: eden helpo, it is called by ref so i cant use indexer or something... just change the rotation velocity to get set and see the error
    private float RotationVelocity;

    // Timeout delta
    private float FallTimeoutDelta { get; set; }
    private float JumpTimeoutDelta { get; set; }

    private float TerminalVelocity = -53.0f;

    private Vector3 TargetDirection { get; set; }

    #endregion

    // Eden ref: do we need this?
    // private string animParamSpeed = "Speed";

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalStepOffset = controller.stepOffset;
        this.playerStartHeight = controller.height;
        this.colliderStartHeight = controller.center.y;

        // Locking cursor to not show it while moving.
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isGamePaused && this.PlyrStats.isAlive)
        {
            this.HandleCrouchInput();
            //this.HandleStopWhenDead();
            this.HandleJump();
            this.GroundedCheck();
            this.HandleDodgeInput();
            this.HandleMovement();

        }
    }

    private void HandleMovement()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = IsDashing ? dashModifier * movementSpeed : movementSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (!this.IsMoving) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float speed;

        //not useing analog at the moment so i commented that for now. TODO: insert analog movement later... maybe?
        //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        float inputMagnitude = 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }
        AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3(horizontal, 0.0f, vertical).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude



        TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
        //float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity, rotationSmoothTime);

        // rotate to face input direction relative to camera position
        // transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);



        // rotate to face camera direction
        if (Input.GetAxisRaw("Camera Unlocked") == 0)
        {
            transform.rotation = new Quaternion(0.0f, playerCamera.rotation.y, 0.0f, playerCamera.rotation.w);
        }
        TargetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

        // move the player
        if (!this.IsRolling)
            controller.Move(TargetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        // if (_hasAnimator)
        // {
        //     _animator.SetFloat(_animIDSpeed, _animationBlend);
        //     _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        // }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        // if (_hasAnimator)
        // {
        //     _animator.SetBool(_animIDGrounded, Grounded);
        // }
    }

    private void HandleJump()
    {
        if (IsGrounded)
        {
            // reset the fall timeout timer
            FallTimeoutDelta = FallTimeout;

            // update animator if using character
            // if (_hasAnimator)
            // {
            //     _animator.SetBool(_animIDJump, false);
            //     _animator.SetBool(_animIDFreeFall, false);
            // }

            // stop our velocity dropping infinitely when grounded
            if (VerticalVelocity < 0.0f)
            {
                //TODO: add fall damage here.

                VerticalVelocity = -2f;
            }

            // Jump
            if (IsJumping && JumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

                // update animator if using character
                // if (_hasAnimator)
                // {
                //     _animator.SetBool(_animIDJump, true);
                // }
            }

            // jump timeout
            if (JumpTimeoutDelta >= 0.0f)
            {
                JumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            JumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (FallTimeoutDelta >= 0.0f)
            {
                FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                // if (_hasAnimator)
                // {
                //     _animator.SetBool(_animIDFreeFall, true);
                // }
            }

            // if we are not grounded, do not jump
            //_input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (VerticalVelocity > TerminalVelocity)
        {
            VerticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }



    // Eden ref: please annotate this function
    private void DeprecatedHandleJump()
    {
        if (this.controller.isGrounded)
        {
            if (this.Velocity.y < -this.jumpHeight)
            {
                // Debug.Log("velocity.y = " + velocity.y);
                PlayerStats playerStats = transform.gameObject.GetComponent<PlayerStats>();
                if (-this.Velocity.y > 30)
                {
                    this._velocity.y *= 3.5f;
                }
                else if (-this.Velocity.y > 20)
                {
                    this._velocity.y *= 2f;
                }

                int fallDamage = (int)(-this.Velocity.y - this.jumpHeight - this.PlyrStats.fallDamageReduction);

                if (fallDamage < 0)
                {
                    fallDamage = 0;
                }
                // Debug.Log("Damage taken from fall damage: " + fallDamage);
                this.PlyrStats.TakePercentileDamage(fallDamage, false);
            }

            this.controller.stepOffset = this.OriginalStepOffset;
            this._velocity.y = 0;
        }
        else
        {
            this._velocity.y += (Physics.gravity.y * Time.deltaTime);

            // Prevents some wierd jumping bugs while moving across stairs.
            this.controller.stepOffset = 0;
        }

        if (!this.PlyrStats.isAlive)
        {
            Velocity = Vector3.zero;
        }

        // Keyboard input (jump)


        // TODO: need to check more stuff for double jumping or other kinds of jumps.
        if (this.IsJumping && this.controller.isGrounded)
        {
            this._velocity.y = this.jumpHeight;
        }
    }



    private void HandleStopWhenDead()
    {
        this.Velocity = this.PlyrStats.isAlive ? this.Velocity : Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        this.GroundAngle = Vector3.Angle(hit.normal, Vector3.up);
        this.IsClimbable = Mathf.Round(this.GroundAngle) <= this.controller.slopeLimit;
    }

    // Eden ref: please annotate this function
    private void HandleCrouchInput()
    {
        if (Input.GetButtonDown("Crouch") && !this.IsCrouching)
        {
            // Debug.Log("Crouch on");
            this.PlayerAnim.SetBool("Crouching", true);
            this.IsCrouching = true;
            this.controller.height = this.playerStartHeight * 0.5f;
            this.controller.center = new Vector3(this.controller.center.x, this.heightChange, this.controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight / 2;

        }
        else if (Input.GetButtonDown("Crouch") && this.IsCrouching)
        {
            // Debug.Log("Crouch off");
            this.PlayerAnim.SetBool("Crouching", false);
            this.IsCrouching = false;
            this.controller.height = this.playerStartHeight;
            this.controller.center = new Vector3(controller.center.x, this.colliderStartHeight, controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight;
        }
    }

    // Eden ref: please annotate this function
    private void HandleDodgeInput()
    {
        if (Input.GetButtonDown("Dodge") && !this.IsRolling && this.IsGrounded)
        {
            StartCoroutine(DodgeCoroutine());
        }
    }

    // Eden ref: please annotate this function
    IEnumerator DodgeCoroutine()
    {

        float startTime = Time.time;
        this.IsRolling = true;
        this.PlayerAnim.SetTrigger("Roll");

        while (Time.time < startTime + this.dodgeTime)
        {

            //Vector3 nonMovingDirection = Quaternion.Euler(0.0f, playerCamera.eulerAngles.y, 0.0f) * Vector3.forward;
            Vector3 dodgeDirection = this.IsMoving ? this.TargetDirection.normalized : (this.TargetDirection * -1).normalized;
            //Vector3 dodgeDirection = this.TargetDirection;

            this.controller.Move(dodgeDirection * this.dodgeSpeed * Time.deltaTime + new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        this.IsRolling = false;
    }








    private void DeprecatedHandleMovement()
    {
        // Gets vertical and horizontal axises
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Calculates current direction
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = direction.magnitude >= 0.1f;

        Vector3 finalMoving = Vector3.zero;

        // Calculates target angle according to mouse movement and keys
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;

        // Calculates if moving forward is possible
        Vector3 directionMod = (this.IsClimbable || !this.IsGrounded ? Vector3.forward : Vector3.back);

        // Sets the diraction according to relevant parameters
        this.MoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * directionMod;

        if (isMoving)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            this.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            finalMoving = this.MoveDirection.normalized * (this.IsClimbable ? movementSpeed : 0.5f) * Time.deltaTime * (this.IsDashing && !this.IsExhausted ? dashModifier : 1);
        }
        // Slides player from unclimbable slopes
        else if (!this.IsClimbable)
        {
            finalMoving = this.MoveDirection * -1 * 0.5f * Time.deltaTime;
        }


        // Handles stamina
        this.PlyrStats.ChangeStamina(this.IsDashing ? 1 : -1);

        // Moves player
        finalMoving += Velocity * Time.deltaTime;

        // Handles going down slopes
        if (this.IsGrounded && !this.IsJumping && Velocity.y <= 0)
            finalMoving.y = finalMoving.y * this.GroundAngle;

        this.controller.Move(finalMoving);
    }

}






