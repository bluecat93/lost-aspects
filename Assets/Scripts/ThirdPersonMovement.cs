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
        // velocity.y = jumpHeight;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(controller.isGrounded);

        if (!PauseMenu.GameIsPaused && PlayerStats.isAlive)
        {
            // keyboard input (jump)
            if (!controller.isGrounded)
            {
                velocity.y += (Physics.gravity.y * Time.deltaTime * Time.deltaTime);
                controller.stepOffset = 0; // prevents some wierd jumping bugs while moving across stairs.
            }
            else
            {
                controller.stepOffset = originalStepOffset;
                velocity.y = 0f;
            }
            if (Input.GetAxis("Jump") > 0 && controller.isGrounded) //TODO need to check more stuff for double jumping or other kinds of jumps.
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

                // Add this to strafe instead of walk to the side i guess
                // if (Input.GetKey(KeyCode.W))
                // {
                //     transform.rotation = Quaternion.Euler(0f, angle, 0f);
                // }

                // Remove this when activating the If above
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * (isClimbable ? Vector3.forward : Vector3.back);

                controller.Move(moveDir.normalized * movementSpeed * Time.deltaTime * (isSprinting ? SprintSpeed : 1));

            }

            // This must be last and as close as possible to other movements so it will allways go down.
            controller.Move(velocity);
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        isClimbable = Vector3.Angle(hit.normal, Vector3.up) <= maxClimbAngle;
    }
}
