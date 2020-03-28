using UnityEngine;
using System.Collections;
using System;

namespace ArrowGame
{
    [RequireComponent(typeof(Controller2D))]
    public class Player : MonoBehaviour
    {
        #region Movement related
        [Header("Movement related")]
        public float maxJumpHeight = 4;
        public float minJumpHeight = 1;
        public float timeToJumpApex = .4f;
        float accelerationTimeAirborne = .2f;
        float accelerationTimeGrounded = .01f;
        float moveSpeed = 6;
        float gravity;
        float maxJumpVelocity;
        float minJumpVelocity;
        Vector3 velocity;
        float velocityXSmoothing;
        #endregion

        #region Referenes
        [SerializeField] private Transform gunTransform;
        [SerializeField] private GameObject bullet;
        public Controller2D controller { get; private set; }
        private Animator _Anim;
        #endregion

        #region private variables
        private PlayerAnimationState playerAnimationState = PlayerAnimationState.None;
        private PlayerLocation playerLocation = PlayerLocation.None;
        public Vector2 directionalInput { get; private set; }
        private float animScale;
        private bool jumpStored = false;
        #endregion

        #region Unity base methods
        private void Awake()
        {
            _Anim = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            controller = GetComponent<Controller2D>();

            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

            animScale = _Anim.transform.localScale.x;
        }

        void Update()
        {
            CalculateVelocity();
            CheckPlayerDirection();

            controller.Move(velocity * Time.deltaTime, directionalInput);

            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }

            if (controller.collisions.below)
            {
                ChangePlayerLocation(PlayerLocation.Grounded);

                if (jumpStored)
                {
                    OnJumpInputDown();
                    jumpStored = false;
                }
            }
            else
                ChangePlayerLocation(PlayerLocation.InAir);
        }

        #endregion

        #region States related
        void ChangeAnimationState(PlayerAnimationState state)
        {
            if (playerAnimationState == state)
                return;

            switch (state)
            {
                case PlayerAnimationState.Idle:
                    _Anim.Play("Idle");
                    break;
                case PlayerAnimationState.Run:
                    _Anim.Play("Run");
                    break;
                case PlayerAnimationState.Jump:
                    _Anim.Play("JumpStart");
                    break;
                case PlayerAnimationState.Attack:
                    _Anim.Play("Fire");
                    break;
                case PlayerAnimationState.Death:
                    _Anim.Play("Death");
                    break;
            }

            playerAnimationState = state;
        }

        void ChangePlayerLocation(PlayerLocation location)
        {
            if (playerLocation == location)
                return;

            switch (location)
            {
                case PlayerLocation.Grounded:
                    ChangeAnimationState(PlayerAnimationState.Idle);
                    break;
                case PlayerLocation.InAir:
                    break;
            }

            playerLocation = location;
        }
        #endregion

        #region Getter and Setter
        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }
        #endregion

        #region Movement related code
        public void OnJumpInputDown()
        {
            if (controller.collisions.below || controller.collisions.secondaryContact)
            {
                velocity.y = maxJumpVelocity;

                ChangeAnimationState(PlayerAnimationState.Jump);
            }
            else if (controller.collisions.jumpBuffer)
            {
                //simulate a jump after player lands on the platform
                jumpStored = true;
            }
        }

        public void OnJumpInputUp()
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        private void CheckPlayerDirection()
        {
            Vector3 tempScale = _Anim.transform.localScale;
            tempScale.x = animScale * controller.collisions.faceDir;
            _Anim.transform.localScale = tempScale;
        }

        public void Shoot()
        {
            Instantiate(bullet, gunTransform.position, gunTransform.rotation);
        }

        void CalculateVelocity()
        {
            if (playerAnimationState != PlayerAnimationState.Jump)
            {
                if (directionalInput.x != 0)
                {
                    ChangeAnimationState(PlayerAnimationState.Run);
                }
                else
                {
                    ChangeAnimationState(PlayerAnimationState.Idle);
                }
            }

            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
        }
        #endregion
    }
}