using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;
    private float capsuleColliderStartingHeight;
    public CharacterController controller;
    public Transform playerCamera;
    private Rigidbody rgbody;
    public float movementSpeed = 5.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private bool isMoving = false;
    private bool isDashing = false;
    private bool isCrouching = false;
    private bool isRolling = false;
    private string animParamSpeed = "Speed";

    // TODO: Move to options when created
    private const KeyCode CrouchKey = KeyCode.C;
    private const KeyCode DodgeKey = KeyCode.V;

    // y movement paramaters
    Vector3 velocity;
    public float jumpHeight = 5f;
    public float DashSpeed = 1.5f;
    private float originalStepOffset;


    // Crouching variables, player height at start and collider position
    private float playerStartHeight;
    private float colliderStartHeight;
    public float crouchColliderPositionY = 0.05f;
    public float heightChange = 0.5f;
    // Dodging Variables
    public float dodgeCoolDown = 1f;
    public float dodgeInvinsibleDuration = 0.5f;
    public float delayBeforeInvinsible = 0.2f;
    public float dodgePushAmt = 3f;
    private float actCoolDown = 1f;

    private Animator playerAnim;
    private bool isClimbable = true;
    private float maxClimbAngle = 60f;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleColliderStartingHeight = capsuleCollider.height;
        playerAnim = GetComponentInChildren<Animator>();
        originalStepOffset = controller.stepOffset;
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
        playerStartHeight = controller.height;
        colliderStartHeight = controller.center.y;
        rgbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GameIsPaused && PlayerStats.isAlive)
        {
            HandleCrouchInput();
            HandleDodgeInput();
            HandleJump();
            HandleMovement();
            HandleStopWhenDead();
            HandleCameraUnlock();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isClimbable = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up)) <= maxClimbAngle;
    }

    private void HandleMovement()
    {
        // Movement inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Movement vector and diractions
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 finalMoving = Vector3.zero;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;

        Vector3 directionMod = (isClimbable || !CheckIsGrounded() ? Vector3.forward : Vector3.back);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * directionMod;

        isMoving = direction.magnitude >= 0.1f;

        // Calculates diraction and speed of normal movement
        if (isMoving)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            isDashing = Input.GetAxis("Dash") > 0;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            finalMoving = moveDir.normalized * (isClimbable ? movementSpeed : 0.5f) * Time.deltaTime * (isDashing ? DashSpeed : 1);
        }
        // Slides if standing on unclimbable slope
        else if (!isClimbable)
        {
            finalMoving = moveDir * -1 * 0.5f * Time.deltaTime;
        }

        finalMoving += velocity * Time.deltaTime;
        controller.Move(finalMoving);

    }

    private void HandleStopWhenDead()
    {
        velocity = PlayerStats.isAlive ? velocity : Vector3.zero;
    }

    private void HandleJump()
    {
        // TODO: write comments for this function to explain the purpose of each part
        if (controller.isGrounded)
        {
            if (velocity.y < -jumpHeight)
            {
                PlayerStats playerStats = transform.gameObject.GetComponent<PlayerStats>();


                if (velocity.y < -30)
                {
                    velocity.y *= 3.5f;
                }
                else if (velocity.y < -20)
                {
                    velocity.y *= 2f;
                }

                int fallDamage = (int)(-velocity.y - jumpHeight - playerStats.fallDamageReduction);

                playerStats.TakePercentileDamage(fallDamage < 0 ? 0 : fallDamage);
            }
            controller.stepOffset = originalStepOffset;
            velocity.y = 0;
        }
        else
        {
            velocity.y += (Physics.gravity.y * Time.deltaTime);

            // Prevents some wierd jumping bugs while moving across stairs.
            controller.stepOffset = 0;
        }

        // Keyboard input (jump)
        bool isJumping = Input.GetAxis("Jump") > 0;

        // TODO: need to check more stuff for double jumping or other kinds of jumps.
        if (isJumping && controller.isGrounded)
        {
            velocity.y = jumpHeight;
        }

    }
    private void HandleCrouchInput()
    {

        if (Input.GetKeyDown(CrouchKey) && !isCrouching)
        {
            playerAnim.SetBool("Crouching", true);
            isCrouching = true;
            controller.height = playerStartHeight * 0.5f;
            controller.center = new Vector3(controller.center.x, heightChange, controller.center.z);
            capsuleCollider.height = capsuleColliderStartingHeight / 2;

        }
        else if (Input.GetKeyDown(CrouchKey) && isCrouching)
        {
            playerAnim.SetBool("Crouching", false);
            isCrouching = false;
            controller.height = playerStartHeight;
            controller.center = new Vector3(controller.center.x, colliderStartHeight, controller.center.z);
            capsuleCollider.height = capsuleColliderStartingHeight;
        }
    }
    private void HandleDodgeInput()
    {
        if (Input.GetKey(DodgeKey))
        {
            playerAnim.SetTrigger("Roll");
        }
    }

    private void HandleCameraUnlock()
    {
        // Rotates the character according to where he looks.
        if (Input.GetAxisRaw("Camera Unlocked") == 0)
        {
            GameObject mainCamera = GameObject.Find("Player/Main Camera");
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    private bool CheckIsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }
}
