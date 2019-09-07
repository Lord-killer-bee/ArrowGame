using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

namespace ArrowGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement parameters")]
        [SerializeField] private float maxMoveSpeed = 8f;
        [SerializeField] private float moveAcceleration = 8f;
        private float currentMoveSpeed = 0;

        [Header("Jump parameters")]
        [SerializeField] private float jumpForce;

        [SerializeField] private float overlapCircleRadius = 2.5f;

        [SerializeField] private Transform GunTransform;
        [SerializeField] private GameObject Bullet;
 
        private Rigidbody2D _RB;
        private Animator _Anim;

        private PlayerState playerState = PlayerState.None;
        private PlayerLocation playerLocation = PlayerLocation.InAir;

        private Collider2D[] hitColliders;

        public bool isRotated = false;


        [SerializeField] private Text stateText;

        protected void Awake()
        {
            _RB = GetComponent<Rigidbody2D>();
            _Anim = GetComponent<Animator>();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _RB.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            PrintCurrentState();
            CheckIfGrounded();

            if (Input.GetButtonDown("Jump") && playerLocation == PlayerLocation.Grounded)
            {
                ChangeState(PlayerState.Jump);
            }
            else if (Input.GetAxis("Horizontal") != 0 && playerLocation == PlayerLocation.Grounded)
            {
                ChangeState(PlayerState.Run);
            }
            else if(Input.GetAxis("Horizontal") == 0 && playerLocation == PlayerLocation.Grounded)
            {
                ChangeState(PlayerState.Idle);
            }

            if(Input.GetButtonDown("Fire"))
            {
                Shoot();
            }

        }

        void ChangeState(PlayerState state)
        {
            if (playerState == state)
                return;

            OnExitState(state);

            switch (state)
            {
                case PlayerState.Idle:
                    _Anim.Play("Idle");
                    break;
                case PlayerState.Run:
                    _Anim.Play("Run");
                    break;
                case PlayerState.Jump:
                    _Anim.Play("JumpStart");
                    break;
                case PlayerState.Attack:
                    _Anim.Play("Fire");
                    break;
                case PlayerState.Death:
                    _Anim.Play("Death");
                    break;
            }

            playerState = state;
        }

        private void OnExitState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Run:
                    currentMoveSpeed = 0;
                    break;
            }
        }

        void ChangePlayerLocation(PlayerLocation location)
        {
            if (playerLocation == location)
                return;

            switch (location)
            {
                case PlayerLocation.Grounded:
                    break;
                case PlayerLocation.InAir:
                    break;
            }

            playerLocation = location;
        }

        private void FixedUpdate()
        {
            switch (playerState)
            {
                case PlayerState.Run:
                    Move(Input.GetAxis("Horizontal"));
                    break;
                case PlayerState.Jump:
                    Jump();
                    break;

            }
        }

        public void Move(float dir)
        {
            if (currentMoveSpeed < maxMoveSpeed)
            {
                currentMoveSpeed += moveAcceleration * 0.05f;
            }
            else if (currentMoveSpeed >= maxMoveSpeed)
            {
                currentMoveSpeed = maxMoveSpeed;
            }

            if (dir < 0)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                isRotated = true;
            }
            else if (dir > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                isRotated = false;
            }

            var vel = new Vector2(dir, 0) * currentMoveSpeed * Time.fixedDeltaTime;
            vel.y = _RB.velocity.y;
            _RB.velocity = vel;
        }

        public void Jump()
        {
            if (playerLocation == PlayerLocation.Grounded)
            {
                _RB.velocity = Vector2.up * jumpForce;   
                ChangePlayerLocation(PlayerLocation.InAir);
            }

            if(_RB.velocity.y < 0)
            {
                _Anim.Play("JumpFall");
            }

            Move(Input.GetAxis("Horizontal"));
        }

        public void Shoot()
        {
            Instantiate(Bullet, GunTransform.position, GunTransform.rotation);
        }

        private void CheckIfGrounded()
        {
            hitColliders = Physics2D.OverlapCircleAll(transform.position, overlapCircleRadius);
            int platformCount = 0;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].CompareTag("Platform") || hitColliders[i].CompareTag("Monster"))
                {
                    platformCount++;
                }
            }

            if(platformCount > 0)
                ChangePlayerLocation(PlayerLocation.Grounded);
            else
                ChangePlayerLocation(PlayerLocation.InAir);
        }

        private void PrintCurrentState()
        {
            stateText.transform.parent.position = transform.position + new Vector3(0, 4.25f, 0);
            stateText.text = playerState.ToString() + "\n" + playerLocation.ToString();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, overlapCircleRadius);
        }
    }
}