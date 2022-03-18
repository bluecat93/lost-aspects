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

    // y movement paramaters
    Vector3 velocity;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float dashModifier = 1.5f;

    // Crouching variables, player height at start and collider position
    [SerializeField] private float playerStartHeight;
    [SerializeField] private float colliderStartHeight;
    [SerializeField] private float crouchColliderPositionY = 0.05f;
    [SerializeField] private float heightChange = 0.5f;

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    #region properties

    private Rigidbody rgbody;

    private Rigidbody Rgbody
    {
        get
        {
            if (rgbody == null)
                rgbody = GetComponent<Rigidbody>();
            return rgbody;
        }
    }

    private PlayerStats playerStats;

    private PlayerStats PlayerStats
    {
        get
        {
            if (playerStats == null)
                playerStats = GetComponent<PlayerStats>();

            return playerStats;
        }
    }


    private bool IsDashing
    {
        get
        {
            return Input.GetAxis("Dash") > 0 && !IsExhausted;
        }
    }

    private bool IsExhausted
    {
        get
        {
            return playerStats.currentStamina <= 0;
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

    private CapsuleCollider CpslCollider { get; set; }

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

    private Animator PlayerAnim { get; set; }

    private float OriginalStepOffset { get; set; }

    #endregion


    // private string animParamSpeed = "Speed";


    // Start is called before the first frame update
    void Start()
    {
        this.CpslCollider = GetComponent<CapsuleCollider>();

        this.PlayerAnim = GetComponentInChildren<Animator>();
        this.OriginalStepOffset = controller.stepOffset;
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
        playerStartHeight = controller.height;
        colliderStartHeight = controller.center.y;

        this.IsClimbable = true;
    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    async void Update()
    {
        HandleCrouchInput();
        HandleDodgeInput();

        if (!PauseMenu.GameIsPaused && PlayerStats.isAlive)
        {

            if (controller.isGrounded)
            {
                if (velocity.y < -jumpHeight)
                {
                    // Debug.Log("velocity.y = " + velocity.y);
                    PlayerStats playerStats = transform.gameObject.GetComponent<PlayerStats>();
                    if (-velocity.y > 30)
                    {
                        velocity.y *= 3.5f;
                    }
                    else if (-velocity.y > 20)
                    {
                        velocity.y *= 2f;
                    }
                    int fallDamage = (int)(-velocity.y - jumpHeight - playerStats.fallDamageReduction);
                    if (fallDamage < 0)
                    {
                        fallDamage = 0;
                    }
                    // Debug.Log("Damage taken from fall damage: " + fallDamage);
                    playerStats.TakePercentileDamage(fallDamage);
                }

                controller.stepOffset = this.OriginalStepOffset;
                velocity.y = 0;
            }
            else
            {
                velocity.y += (Physics.gravity.y * Time.deltaTime);

                // Prevents some wierd jumping bugs while moving across stairs.
                controller.stepOffset = 0;
            }

            if (!PlayerStats.isAlive)
            {
                velocity = Vector3.zero;
            }

            // Keyboard input (jump)
            bool isJumping = Input.GetAxis("Jump") > 0;

            // TODO: need to check more stuff for double jumping or other kinds of jumps.
            if (isJumping && controller.isGrounded)
            {
                velocity.y = jumpHeight;
            }


            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            bool isMoving = direction.magnitude >= 0.1f;

            Vector3 finalMoving = Vector3.zero;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;

            Vector3 directionMod = (this.IsClimbable || !this.IsGrounded ? Vector3.forward : Vector3.back);
            this.MoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * directionMod;

            if (isMoving)
            {
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);


                finalMoving = this.MoveDirection.normalized * (this.IsClimbable ? movementSpeed : 0.5f) * Time.deltaTime * (this.IsDashing && !this.IsExhausted ? dashModifier : 1);
                Debug.Log(this.IsDashing && !this.IsExhausted ? dashModifier : 1);
            }
            else if (!this.IsClimbable)
            {
                finalMoving = this.MoveDirection * -1 * 0.5f * Time.deltaTime;
            }

            playerStats.ChangeStamina(this.IsDashing ? 1 : -1);

            finalMoving += velocity * Time.deltaTime;
            controller.Move(finalMoving);

            // Rotates the character according to where he looks.
            if (Input.GetAxisRaw("Camera Unlocked") == 0)
            {
                GameObject mainCamera = GameObject.Find("Player/Main Camera");
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        this.IsClimbable = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up)) <= controller.slopeLimit;
    }

    private void HandleCrouchInput()
    {

        if (Input.GetKeyDown(CrouchKey) && !this.IsCrouching)
        {
            this.PlayerAnim.SetBool("Crouching", true);
            this.IsCrouching = true;
            controller.height = playerStartHeight * 0.5f;
            controller.center = new Vector3(controller.center.x, heightChange, controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight / 2;

        }
        else if (Input.GetKeyDown(CrouchKey) && this.IsCrouching)
        {
            this.PlayerAnim.SetBool("Crouching", false);
            this.IsCrouching = false;
            controller.height = playerStartHeight;
            controller.center = new Vector3(controller.center.x, colliderStartHeight, controller.center.z);
            this.CpslCollider.height = this.CapsuleColliderStartingHeight;
        }
    }
    private void HandleDodgeInput()
    {
        if (Input.GetButtonDown("Dodge"))
        {
            this.PlayerAnim.SetTrigger("Roll");
            StartCoroutine(DodgeCoroutine());
        }
    }

    IEnumerator DodgeCoroutine()
    {
        float startTime = Time.time;

        // if (Time.time >= startTime + dashTime)
        // {
        //     playerAnim.ResetTrigger("Roll");
        // }

        while (Time.time < startTime + this.DashTime)
        {
            controller.Move(this.MoveDirection * this.DashSpeed * Time.deltaTime);
            yield return null;
        }

    }

}
