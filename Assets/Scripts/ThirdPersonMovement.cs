using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;
    public float movementSpeed = 5.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private bool isMoving = false;
    private bool isSprinting = false;
    private string animParamSpeed = "Speed";

    // TODO: Move to options when created
    private const KeyCode SprintKey = KeyCode.LeftShift;

    // y movement paramaters
    Vector3 velocity;
    public float jumpHeight = 0.05f;
    public float SprintSpeed = 1.5f;
    private float originalStepOffset;

    private Animator playerAnim;

    private bool isClimbable = true;
    private float maxClimbAngle = 60f;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        originalStepOffset = controller.stepOffset;
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.GameIsPaused && PlayerStats.isAlive)
        {
            bool isJumping = Input.GetAxis("Jump") > 0;

            // Keyboard input (jump)
            if (!controller.isGrounded)
            {
                // Prevents some wierd jumping bugs while moving across stairs.
                controller.stepOffset = 0;
                velocity.y += (Physics.gravity.y * Time.deltaTime * Time.deltaTime);
            }
            else
            {
                controller.stepOffset = originalStepOffset;
                velocity.y = 0f;
            }

            // TODO: need to check more stuff for double jumping or other kinds of jumps.
            if (isJumping && controller.isGrounded)
            {
                velocity.y = jumpHeight;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            isMoving = direction.magnitude >= 0.1f;

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

                controller.Move(moveDir.normalized * (isClimbable ? movementSpeed : 0.5f) * Time.deltaTime * (isSprinting ? SprintSpeed : 1));

            }

            // This must be last and as close as possible to other movements so it will allways go down.
            controller.Move(velocity);

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
}
