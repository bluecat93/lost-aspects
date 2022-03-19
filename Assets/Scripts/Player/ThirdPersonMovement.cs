using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // Eden ref: where do this 3 params get assigned?
    public CharacterController controller;
    public Transform playerCamera;

    float turnSmoothVelocity;

    // TODO: Move to options when created
    private const KeyCode CrouchKey = KeyCode.C;
    private const KeyCode DodgeKey = KeyCode.V;

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
    [SerializeField] private float turnSmoothTime = 0.1f;

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

    public bool IsCrouching { get; set; }

    public bool IsRolling { get; set; }

    public bool IsGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, Vector3.down, 0.1f);
        }
    }


    private bool IsClimbable { get; set; }

    private Vector3 MoveDirection { get; set; }

    private float CapsuleColliderStartingHeight
    {
        get
        {
            return this.CpslCollider.height;
        }
    }

    // Dodging Variables
    public float DashSpeed { get; set; }
    public float DashTime { get; set; }

    private float OriginalStepOffset { get; set; }

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
        if (!PauseMenu.GameIsPaused && this.PlyrStats.isAlive)
        {
            this.HandleStopWhenDead();
            this.HandleJump();
            this.HandleMovement();
            this.HandleCrouchInput();
            this.HandleDodgeInput();
            this.HandleCameraUnlock();
        }
    }

    // Update is called once per frame
    async void Update()
    {
    }

    private void HandleMovement()
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
        this.controller.Move(finalMoving);
    }

    // Eden ref: please annotate this function
    private void HandleJump()
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
                this.PlyrStats.TakePercentileDamage(fallDamage);
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
        bool isJumping = Input.GetAxis("Jump") > 0;

        // TODO: need to check more stuff for double jumping or other kinds of jumps.
        if (isJumping && this.controller.isGrounded)
        {
            this._velocity.y = this.jumpHeight;
        }


    }

    private void HandleCameraUnlock()
    {
        // Rotates the character according to where he looks.
        if (Input.GetAxisRaw("Camera Unlocked") == 0)
        {
            GameObject mainCamera = GameObject.Find("Player/Main Camera");
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, this.transform.eulerAngles.z);
        }
    }

    private void HandleStopWhenDead()
    {
        this.Velocity = this.PlyrStats.isAlive ? this.Velocity : Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        this.IsClimbable = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up)) <= this.controller.slopeLimit;
    }

    // Eden ref: please annotate this function
    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(CrouchKey) && !this.IsCrouching)
        {
            // Debug.Log("Crouch on");
            this.PlayerAnim.SetBool("Crouching", true);
            this.IsCrouching = true;
            this.controller.height = this.playerStartHeight * 0.5f;
            this.controller.center = new Vector3(this.controller.center.x, this.heightChange, this.controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight / 2;

        }
        else if (Input.GetKeyDown(CrouchKey) && this.IsCrouching)
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
        if (Input.GetButtonDown("Dodge"))
        {
            Debug.Log("dodge on");
            this.PlayerAnim.SetTrigger("Roll");
            StartCoroutine(DodgeCoroutine());
        }
    }

    // Eden ref: please annotate this function
    IEnumerator DodgeCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + this.DashTime)
        {
            this.controller.Move(this.MoveDirection * this.DashSpeed * Time.deltaTime);
            yield return null;
        }

    }

}
