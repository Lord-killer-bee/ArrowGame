﻿using System;
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

        private PlayerAnimationState playerState = PlayerAnimationState.None;
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

        private bool activationPointInRange = false;
        private ActivationPoint inRangeActivationPoint;

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

            if (Input.GetButtonDown(GameConsts.JUMP_CODE) && (playerLocation == PlayerLocation.Grounded || almostGrounded))
            {
                ChangeState(PlayerAnimationState.Jump);
            }
            else if (Input.GetAxis(GameConsts.HORIZONTAL_CODE) != 0 && playerLocation == PlayerLocation.Grounded)
            {
                ChangeState(PlayerAnimationState.Run);
            }
            else if(Input.GetAxis(GameConsts.HORIZONTAL_CODE) == 0 && playerLocation == PlayerLocation.Grounded && !jumpBufferActive && !jumpStored)
            {
                ChangeState(PlayerAnimationState.Idle);
            }

            if(Input.GetButtonDown(GameConsts.FIRE_CODE))
            {
                Shoot();
            }

            if (Input.GetButtonDown(GameConsts.ABILITY_ACTIVATE_CODE))
            {
                inRangeActivationPoint.ActivatePoint();
            }

            switch (playerState)
            {
                case PlayerAnimationState.Run:
                    Move(Input.GetAxis(GameConsts.HORIZONTAL_CODE));
                    break;
                case PlayerAnimationState.Jump:
                    Jump();
                    break;

            }

        }

        void ChangeState(PlayerAnimationState state)
        {
            if (playerState == state)
                return;

            OnExitState(state);

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

            playerState = state;
        }

        private void OnExitState(PlayerAnimationState state)
        {
            switch (state)
            {
                case PlayerAnimationState.Run:
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
            else if (playerLocation == PlayerLocation.InAir && Input.GetButtonDown(GameConsts.JUMP_CODE))
            {
                if (IsAbilityActive(PlayerAbilityType.DoubleJump) && GetComponent<DoubleJumpAbility>().currentJumps > 0)
                {
                    _RB.velocity = Vector2.up * jumpForce;
                    GetComponent<DoubleJumpAbility>().RegisterJump();
                }
            }

            if(almostGrounded && Input.GetButtonDown(GameConsts.JUMP_CODE))
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

            Move(Input.GetAxis(GameConsts.HORIZONTAL_CODE));
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
                if (groundedHits[i].collider.CompareTag(GameConsts.PLATFORM_TAG) || groundedHits[i].collider.CompareTag(GameConsts.MONSTER_TAG) || groundedHits[i].collider.CompareTag(GameConsts.BLOCK_TAG))
                {
                    platformCount++;
                }
            }

            //Secondary grounded test
            groundedHits = Physics2D.RaycastAll(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset, 0, 0), Vector2.down, groundedCheckLength);

            for (int i = 0; i < groundedHits.Length; i++)
            {
                if (groundedHits[i].collider.CompareTag(GameConsts.PLATFORM_TAG) || groundedHits[i].collider.CompareTag(GameConsts.MONSTER_TAG))
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
            if (playerState == PlayerAnimationState.Jump && !jumpBufferActive)
            {
                int platformCount = 0;

                almostGroundedHits = Physics2D.RaycastAll(transform.position, Vector2.down, almostGroundedCheckLength);

                for (int i = 0; i < almostGroundedHits.Length; i++)
                {
                    if (almostGroundedHits[i].collider.CompareTag(GameConsts.PLATFORM_TAG))
                    {
                        platformCount++;
                    }
                }

                almostGroundedHits = Physics2D.RaycastAll(transform.position - (isRotated ? -1 : 1) * new Vector3(secondaryGroundedCheckOffset, 0, 0), Vector2.down, almostGroundedCheckLength);

                for (int i = 0; i < almostGroundedHits.Length; i++)
                {
                    if (almostGroundedHits[i].collider.CompareTag(GameConsts.PLATFORM_TAG))
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
            activationPointInRange = e.inRange;

            if (e.inRange)
            {
                inRangeActivationPoint = e.activationPoint;
            }
            else
            {
                inRangeActivationPoint = null;
            }
        }

        #endregion

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(GameConsts.PLATFORM_BOTTOM_TAG))
            {
                _collider.isTrigger = true;
            }
            else if (collider.CompareTag(GameConsts.PLATFORM_TAG))
            {
                phasing = true;
            }

            if(collider.tag == GameConsts.MONSTER_TAG)
            {
                GameEventManager.Instance.TriggerSyncEvent(new PlayerKilledEvent());
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag(GameConsts.PLATFORM_TAG) && phasing)
            {
                _collider.isTrigger = false;
                phasing = false;
            }
            else if (collider.CompareTag(GameConsts.PLATFORM_BOTTOM_TAG) && !phasing)
            {
                _collider.isTrigger = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == GameConsts.MONSTER_TAG)
            {
                GameEventManager.Instance.TriggerSyncEvent(new PlayerKilledEvent());
                Destroy(gameObject);
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