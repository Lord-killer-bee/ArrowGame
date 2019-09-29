using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using Core;
using ArrowGame.InGameEvents;

namespace ArrowGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement parameters")]
        [SerializeField] private float maxMoveSpeed = 8f;
        [SerializeField] private float moveAcceleration = 8f;
        private float currentMoveSpeed = 0;
        private float moveSpeedMultiplier = 1;

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
        private float secondaryGroundedCheckOffset = 0;
        private float almostGroundedCheckLength = 0;
        private bool almostGrounded = false;
        private bool jumpStored = false;

        private bool phasing = false;

        private AbilityConfig abilityConfig;

        [SerializeField] private Text stateText;

        protected void Awake()
        {
            _RB = GetComponent<Rigidbody2D>();
            _Anim = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();

            groundedCheckLength = _collider.bounds.size.y / 2 + 0.1f;
            secondaryGroundedCheckOffset = _collider.bounds.size.x / 2 + 3f;
            almostGroundedCheckLength = _collider.bounds.size.y / 2 + 7f;

            abilityConfig = Resources.Load<AbilityConfig>(GameConsts.AbilityConfigPath);

            AddListeners();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _RB.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void AddListeners()
        {
            GameEventManager.Instance.AddListener<GrantAbilityEvent>(OnAbilityGranted);
            GameEventManager.Instance.AddListener<ActivationPointInRangeEvent>(OnActivationPointStatusRecieved);
        }

        void RemoveListeners()
        {
            GameEventManager.Instance.RemoveListener<GrantAbilityEvent>(OnAbilityGranted);
            GameEventManager.Instance.RemoveListener<ActivationPointInRangeEvent>(OnActivationPointStatusRecieved);
        }

        private void Update()
        {
            PrintCurrentState();
            CheckIfGrounded();
            CheckIfAlmostGrounded();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameEventManager.Instance.TriggerAsyncEvent(new GrantAbilityEvent(PlayerAbilityType.Invincibility));
            }

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
                    if (IsAbilityActive(PlayerAbilityType.DoubleJump))
                    {
                        GetComponent<DoubleJumpAbility>().RestoreJumps();
                    }
                    break;
                case PlayerLocation.InAir:
                    break;
            }

            playerLocation = location;
        }

        public void Move(float dir)
        {
            if (currentMoveSpeed < maxMoveSpeed * moveSpeedMultiplier)
            {
                currentMoveSpeed += moveAcceleration * moveSpeedMultiplier * 0.05f;
            }
            else if (currentMoveSpeed >= maxMoveSpeed * moveSpeedMultiplier)
            {
                currentMoveSpeed = maxMoveSpeed * moveSpeedMultiplier;
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

                if (IsAbilityActive(PlayerAbilityType.DoubleJump))
                {
                    GetComponent<DoubleJumpAbility>().RegisterJump();
                }
            }
            else if (playerLocation == PlayerLocation.InAir && Input.GetButtonDown("Jump"))
            {
                if (IsAbilityActive(PlayerAbilityType.DoubleJump) && GetComponent<DoubleJumpAbility>().currentJumps > 0)
                {
                    _RB.velocity = Vector2.up * jumpForce;
                    GetComponent<DoubleJumpAbility>().RegisterJump();
                }
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
            int platformCount = 0;

            //Primary grounded test
            groundedHits = Physics2D.RaycastAll(transform.position, Vector2.down, groundedCheckLength);

            for (int i = 0; i < groundedHits.Length; i++)
            {
                if (groundedHits[i].collider.CompareTag("Platform") || groundedHits[i].collider.CompareTag("Monster") || groundedHits[i].collider.CompareTag("Block"))
                {
                    platformCount++;
                }
            }

            //Secondary grounded test
            groundedHits = Physics2D.RaycastAll(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset, 0, 0), Vector2.down, groundedCheckLength);

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
                int platformCount = 0;

                almostGroundedHits = Physics2D.RaycastAll(transform.position, Vector2.down, almostGroundedCheckLength);

                for (int i = 0; i < almostGroundedHits.Length; i++)
                {
                    if (almostGroundedHits[i].collider.CompareTag("Platform"))
                    {
                        platformCount++;
                    }
                }

                almostGroundedHits = Physics2D.RaycastAll(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset, 0, 0), Vector2.down, almostGroundedCheckLength);

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

        #region Abilities related

        /// <summary>
        /// Adds an ability to the active ability list.
        /// If an ability is already active then that ability duration is restored
        /// </summary>
        /// <param name="eve"></param>
        private void OnAbilityGranted(GrantAbilityEvent eve)
        {
            PlayerAbility ability = null;

            if (IsAbilityGranted(eve.abilityType))
            {
                switch (eve.abilityType)
                {
                    case PlayerAbilityType.SpeedMultiplier:
                        ability = gameObject.GetComponent<SpeedMultiplierAbility>();
                        break;
                    case PlayerAbilityType.DoubleJump:
                        ability = gameObject.GetComponent<DoubleJumpAbility>();
                        break;
                    case PlayerAbilityType.Invincibility:
                        ability = gameObject.GetComponent<InvincibilityAbility>();
                        break;
                }

                ability.RestoreTheAbilityDuration();
            }
            else
            {
                switch (eve.abilityType)
                {
                    case PlayerAbilityType.SpeedMultiplier:
                        ability = gameObject.AddComponent<SpeedMultiplierAbility>();
                        break;
                    case PlayerAbilityType.DoubleJump:
                        ability = gameObject.AddComponent<DoubleJumpAbility>();
                        break;
                    case PlayerAbilityType.Invincibility:
                        ability = gameObject.AddComponent<InvincibilityAbility>();
                        break;
                }
                ability.InitializeAbility(abilityConfig);
            }
        }

        private bool IsAbilityActive(PlayerAbilityType abilityType)
        {
            switch (abilityType)
            {
                case PlayerAbilityType.SpeedMultiplier:
                    if (!GetComponent<SpeedMultiplierAbility>())
                    {
                        return false;
                    }
                    else
                    {
                        return GetComponent<SpeedMultiplierAbility>().isActive;
                    }
                case PlayerAbilityType.DoubleJump:
                    if (!GetComponent<DoubleJumpAbility>())
                    {
                        return false;
                    }
                    else
                    {
                        return GetComponent<DoubleJumpAbility>().isActive;
                    }
                case PlayerAbilityType.Invincibility:
                    if (!GetComponent<InvincibilityAbility>())
                    {
                        return false;
                    }
                    else
                    {
                        return GetComponent<InvincibilityAbility>().isActive;
                    }
            }

            return false;
        }

        private bool IsAbilityGranted(PlayerAbilityType abilityType)
        {
            switch (abilityType)
            {
                case PlayerAbilityType.SpeedMultiplier:
                    return (GetComponent<SpeedMultiplierAbility>() != null);
                case PlayerAbilityType.DoubleJump:
                    return (GetComponent<DoubleJumpAbility>() != null);
                case PlayerAbilityType.Invincibility:
                    return (GetComponent<InvincibilityAbility>() != null);
            }

            return false;
        }

        public void MultiplyMoveSpeed(float mult)
        {
            moveSpeedMultiplier = mult;
        }

        #endregion

        #region Level related

        private void OnActivationPointStatusRecieved(ActivationPointInRangeEvent e)
        {
            //TODO:: Activation point status is recieved, assign an input button to trigger it from this script
        }

        #endregion

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
            Debug.DrawLine(transform.position - new Vector3(0.05f, 0, 0), transform.position - new Vector3(0.05f, almostGroundedCheckLength, 0), Color.blue);

            Debug.DrawLine(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset, 0, 0), transform.position - new Vector3((isRotated ? -1 : 1) * secondaryGroundedCheckOffset, groundedCheckLength, 0), Color.red);
            Debug.DrawLine(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset - 0.05f, 0, 0), transform.position - new Vector3((isRotated ? -1 : 1) * (secondaryGroundedCheckOffset - 0.05f), almostGroundedCheckLength, 0), Color.blue);
        }
    }
}