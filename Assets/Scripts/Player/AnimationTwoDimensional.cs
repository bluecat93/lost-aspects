using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Player
{
    public class AnimationTwoDimensional : NetworkBehaviour
    {
        [SyncVar] float velocityZ = 0.0f;
        [SyncVar] float velocityX = 0.0f;

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

        [SyncVar] int velocityZHash;
        [SyncVar] int velocityXHash;

        Animator _animator;

        private Animator Anmtr
        {
            get
            {
                if (this._animator == null)
                    this._animator = GetComponentInChildren<Animator>();

                return this._animator;
            }
        }

        ThirdPersonMovement _ThirdPersonMovement;

        private ThirdPersonMovement ThrdPrsnMvmnt
        {
            get
            {
                if (this._ThirdPersonMovement == null)
                    this._ThirdPersonMovement = GetComponent<ThirdPersonMovement>();

                return this._ThirdPersonMovement;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (hasAuthority)
            {
                // Variables for refactoring
                velocityZHash = Animator.StringToHash("VelocityZ");
                velocityXHash = Animator.StringToHash("VelocityX");
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (ThrdPrsnMvmnt.PlayerModel.activeSelf != false && hasAuthority)
            {
                // checks when dash is pressed and changes maximum velocity if true
                float currentMaxVelocity = dashPressed ? maxDashVelocity : maxRunVelocity;
                KeyPressChecks();
                MovementChecks(currentMaxVelocity);
                MovementDeceleration(currentMaxVelocity);
                AnimatePlayer();
            }
        }
        private void KeyPressChecks()
        {
            this.forwardPressed = Input.GetKey(KeyCode.W);
            this.leftPressed = Input.GetKey(KeyCode.A);
            this.rightPressed = Input.GetKey(KeyCode.D);
            this.backPressed = Input.GetKey(KeyCode.S);
            this.dashPressed = Input.GetKey(KeyCode.LeftShift);
        }
        private void MovementChecks(float currentMaxVelocity)
        {
            if (this.forwardPressed && this.velocityZ < currentMaxVelocity)
                this.velocityZ += Time.deltaTime * this.acceleration;
            if (this.backPressed && this.velocityZ > -1f)
                this.velocityZ -= Time.deltaTime * this.acceleration;
            if (this.leftPressed && this.velocityX > -currentMaxVelocity)
                this.velocityX -= Time.deltaTime * this.acceleration;
            if (this.rightPressed && this.velocityX < currentMaxVelocity)
                this.velocityX += Time.deltaTime * this.acceleration;
        }
        private void MovementDeceleration(float currentMaxVelocity)
        {
            if (!this.forwardPressed && this.velocityZ > 0.0f)
                this.velocityZ -= Time.deltaTime * this.deceleration;
            if (!this.backPressed && this.velocityZ < 0.0f)
                this.velocityZ = 0.0f;
            if (!this.backPressed && !this.forwardPressed && this.velocityZ != 0.0f && (this.velocityZ > -0.05f && this.velocityZ < 0.05))
                this.velocityZ = 0.0f;
            if (!this.leftPressed && this.velocityX < 0.0f)
                this.velocityX += Time.deltaTime * this.deceleration;
            if (!this.rightPressed && this.velocityX > 0.0f)
                this.velocityX -= Time.deltaTime * this.deceleration;
            if (!this.leftPressed && !this.rightPressed && this.velocityX != 0.0f && (this.velocityX > -0.05f && this.velocityX < 0.05f))
                this.velocityX = 0.0f;
            if (this.forwardPressed && this.dashPressed && this.velocityZ > currentMaxVelocity)
                this.velocityZ = currentMaxVelocity;
            else if (this.forwardPressed && this.velocityZ > currentMaxVelocity)
            {
                this.velocityZ -= Time.deltaTime * this.deceleration;
                if (this.velocityZ > currentMaxVelocity && this.velocityZ < (currentMaxVelocity + 0.05f))
                    this.velocityZ = currentMaxVelocity;
            }

        }
        private void AnimatePlayer()
        {
            this.Anmtr.SetFloat(velocityZHash, velocityZ);
            this.Anmtr.SetFloat(velocityXHash, velocityX);
        }
    }
}