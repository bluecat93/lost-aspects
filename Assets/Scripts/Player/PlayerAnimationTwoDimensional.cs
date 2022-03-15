using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTwoDimensional : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    bool forwardPressed = false;
    bool leftPressed = false;
    bool rightPressed = false;
    bool backPressed = false;
    bool dashPressed = false;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maxRunVelocity = 1f;
    public float maxBackWalkVelocity = -1f;
    public float maxDashVelocity = 2.0f;
    int velocityZHash;
    int velocityXHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Variables for refactoring
        velocityZHash = Animator.StringToHash("VelocityZ");
        velocityXHash = Animator.StringToHash("VelocityX");
    }

    // Update is called once per frame
    void Update()
    {
        // checks when dash is pressed and changes maximum velocity if true
        float currentMaxVelocity = dashPressed ? maxDashVelocity : maxRunVelocity;
        KeyPressChecks();
        MovementChecks(currentMaxVelocity);
        MovementDeceleration(currentMaxVelocity);
        AnimatePlayer();
    }
    private void KeyPressChecks()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        leftPressed = Input.GetKey(KeyCode.A);
        rightPressed = Input.GetKey(KeyCode.D);
        backPressed = Input.GetKey(KeyCode.S);
        dashPressed = Input.GetKey(KeyCode.LeftShift);
    }
    private void MovementChecks(float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;
        if (backPressed && velocityZ > -1f)
            velocityZ -= Time.deltaTime * acceleration;
        if (leftPressed && velocityX > -currentMaxVelocity)
            velocityX -= Time.deltaTime * acceleration;
        if (rightPressed && velocityX < currentMaxVelocity)
            velocityX += Time.deltaTime * acceleration;
        // Debug.Log("Velocity Z: " + velocityZ);
    }
    private void MovementDeceleration(float currentMaxVelocity)
    {
        if (!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;
        if (!backPressed && velocityZ < 0.0f)
            velocityZ = 0.0f;
        if (!backPressed && !forwardPressed && velocityZ != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05))
            velocityZ = 0.0f;
        if (!leftPressed && velocityX < 0.0f)
            velocityX += Time.deltaTime * deceleration;
        if (!rightPressed && velocityX > 0.0f)
            velocityX -= Time.deltaTime * deceleration;
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
            velocityX = 0.0f;
        if (forwardPressed && dashPressed && velocityZ > currentMaxVelocity)
            velocityZ = currentMaxVelocity;
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
                velocityZ = currentMaxVelocity;
        }

    }
    private void AnimatePlayer()
    {
        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
    }
}
