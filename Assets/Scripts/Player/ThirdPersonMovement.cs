using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;
    private Rigidbody rgbody;
    public float movementSpeed = 5.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private bool isMoving = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isRolling = false;
    private string animParamSpeed = "Speed";

    // TODO: Move to options when created
    private const KeyCode SprintKey = KeyCode.LeftShift;
    private const KeyCode CrouchKey = KeyCode.C;
    private const KeyCode DodgeKey = KeyCode.V;

    // y movement paramaters
    Vector3 velocity;
    public float jumpHeight = 5f;
    public float SprintSpeed = 1.5f;
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
        playerAnim = GetComponentInChildren<Animator>();
        originalStepOffset = controller.stepOffset;
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
        playerStartHeight = controller.height;
        colliderStartHeight = controller.center.y;
        rgbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            controller.stepOffset = originalStepOffset;
            velocity.y = -0.1f;
        }
        else
        {
            velocity.y +=  (Physics.gravity.y * Time.deltaTime);

            // Prevents some wierd jumping bugs while moving across stairs.
            controller.stepOffset = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCrouchInput();
        HandleDodgeInput();
        if (!PauseMenu.GameIsPaused && PlayerStats.isAlive)
        {
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

            isMoving = direction.magnitude >= 0.1f;

            Vector3 finalMoving = Vector3.zero;

            if (isMoving)
            {
                isSprinting = Input.GetKey(SprintKey);

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                // Debug.Log("Angle: " + angle + " targetAngle: " + targetAngle + " turnSmoothVelocity: " + turnSmoothTime + " turnSmoothTime: " + turnSmoothTime + " eulerAngles: " + transform.eulerAngles.y);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // TODO: Since isGrounded is not reliable, need to research raycasts and understand how to use it instead to fix throwback bug
                Vector3 directionMod = (isClimbable || isJumping ? Vector3.forward : Vector3.back);

                // Vector3 directionMod = (isClimbable || isJumping || (!controller.isGrounded && !isJumping) ? Vector3.forward : Vector3.back);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * directionMod;

                finalMoving = moveDir.normalized * (isClimbable ? movementSpeed : 0.5f) * Time.deltaTime * (isSprinting ? SprintSpeed : 1);
               
                //controller.Move(finalMoving);

            }
            finalMoving += velocity * Time.deltaTime;
            controller.Move(finalMoving);

            // This must be last and as close as possible to other movements so it will allways go down.
            //controller.Move(velocity * Time.deltaTime);

            if (Input.GetAxisRaw("Camera Unlocked") == 0)
            {
                // This rotates the character according to where he looks.
                GameObject mainCamera = GameObject.Find("Player/Main Camera");
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var currClimbAngle = Mathf.Round(Vector3.Angle(hit.normal, Vector3.up));

        isClimbable = currClimbAngle <= maxClimbAngle;// || hit.moveDirection.y <= 0;
        // Debug.Log("isClimbable " + isClimbable + ", currClimbAngle " + currClimbAngle + ", maxClimbAngle " + maxClimbAngle + ", hit.moveDirection.y" + hit.moveDirection.y);
    }
    private void HandleCrouchInput()
    {
        if (Input.GetKeyDown(CrouchKey) && !isCrouching)
        {
            playerAnim.SetBool("Crouching", true);
            isCrouching = true;
            controller.height = playerStartHeight * 0.5f;
            controller.center = new Vector3(controller.center.x, heightChange, controller.center.z);
        }
        else if (Input.GetKeyDown(CrouchKey) && isCrouching)
        {
            playerAnim.SetBool("Crouching", false);
            isCrouching = false;
            controller.height = playerStartHeight;
            controller.center = new Vector3(controller.center.x, colliderStartHeight, controller.center.z);
        }
    }
    private void HandleDodgeInput()
    {
        if (Input.GetKey(DodgeKey))
        {
            playerAnim.SetTrigger("Roll");

        }
    }
}
