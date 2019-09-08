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

        [SerializeField] private Transform GunTransform;
        [SerializeField] private GameObject Bullet;
 
        private Rigidbody2D _RB;
        private Collider2D _collider;
        private Animator _Anim;

        public bool isRotated = false;

        private PlayerState playerState = PlayerState.None;
        private PlayerLocation playerLocation = PlayerLocation.InAir;

        private RaycastHit2D[] groundedHits;
        private RaycastHit2D[] almostGroundedHits;

        private float jumpBufferTime = 0.75f;
        private DateTime jumpHitTime;
        private bool jumpBufferActive = false;

        private float groundedCheckLength = 0;
        private float almostGroundedCheckLength = 0;
        private bool almostGrounded = false;
        private bool jumpStored = false;

        private bool phasing = false;

        [SerializeField] private Text stateText;

        protected void Awake()
        {
            _RB = GetComponent<Rigidbody2D>();
            _Anim = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();

            groundedCheckLength = _collider.bounds.size.y / 2 + 0.1f;
            almostGroundedCheckLength = _collider.bounds.size.y / 2 + 7f;
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
            CheckIfAlmostGrounded();

            if (Input.GetButtonDown("Jump") && (playerLocation == PlayerLocation.Grounded || almostGrounded))
            {
                ChangeState(PlayerState.Jump);
            }
            else if (Input.GetAxis("Horizontal") != 0 && playerLocation == PlayerLocation.Grounded)
            {
                ChangeState(PlayerState.Run);
            }
            else if(Input.GetAxis("Horizontal") == 0 && playerLocation == PlayerLocation.Grounded && !jumpBufferActive && !jumpStored)
            {
                ChangeState(PlayerState.Idle);
            }

            if(Input.GetButtonDown("Fire"))
            {
                Shoot();
            }

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
                jumpHitTime = DateTime.Now;
                jumpBufferActive = true;
                jumpStored = false;
                ChangePlayerLocation(PlayerLocation.InAir);
            }

            if(almostGrounded && Input.GetButtonDown("Jump"))
            {
                jumpStored = true;
            }

            if (_RB.velocity.y < 0)
            {
                _Anim.Play("JumpFall");
            }
            else if(_RB.velocity.y > 0)
            {
                _Anim.Play("JumpStart");
            }

            Move(Input.GetAxis("Horizontal"));
        }

        public void Shoot()
        {
            Instantiate(Bullet, GunTransform.position, GunTransform.rotation);
        }

        private void CheckIfGrounded()
        {
            groundedHits = Physics2D.RaycastAll(transform.position, Vector2.down, groundedCheckLength);

            int platformCount = 0;

            for (int i = 0; i < groundedHits.Length; i++)
            {
                if (groundedHits[i].collider.CompareTag("Platform") || groundedHits[i].collider.CompareTag("Monster"))
                {
                    platformCount++;
                }
            }

            if (!jumpBufferActive)
            {
                if (platformCount > 0)
                    ChangePlayerLocation(PlayerLocation.Grounded);
                else
                    ChangePlayerLocation(PlayerLocation.InAir);
            }

            if(jumpBufferActive && (DateTime.Now - jumpHitTime).TotalMilliseconds >= jumpBufferTime * 1000)
            {
                jumpBufferActive = false;
            }
        }

        private void CheckIfAlmostGrounded()
        {
            if (playerState == PlayerState.Jump && !jumpBufferActive)
            {
                almostGroundedHits = Physics2D.RaycastAll(transform.position, Vector2.down, almostGroundedCheckLength);

                int platformCount = 0;

                for (int i = 0; i < almostGroundedHits.Length; i++)
                {
                    if (almostGroundedHits[i].collider.CompareTag("Platform"))
                    {
                        platformCount++;
                    }
                }

                if (platformCount > 0)
                {
                    almostGrounded = true;
                }
            }
            else
            {
                almostGrounded = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("PlatformBottom"))
            {
                _collider.isTrigger = true;
            }
            else if (collider.CompareTag("Platform"))
            {
                phasing = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Platform") && phasing)
            {
                _collider.isTrigger = false;
                phasing = false;
            }
            else if (collider.CompareTag("PlatformBottom") && !phasing)
            {
                _collider.isTrigger = false;
            }
        }

        private void PrintCurrentState()
        {
            stateText.transform.parent.position = transform.position + new Vector3(0, 4.25f, 0);
            stateText.text = playerState.ToString() + "\n" + playerLocation.ToString() + "\n" + "AG : " + almostGrounded;

            Debug.DrawLine(transform.position, transform.position - new Vector3(0, groundedCheckLength, 0), Color.red);
            Debug.DrawLine(transform.position + new Vector3(0.01f, 0, 0), transform.position - new Vector3(0.01f, almostGroundedCheckLength, 0), Color.blue);
        }
    }
}