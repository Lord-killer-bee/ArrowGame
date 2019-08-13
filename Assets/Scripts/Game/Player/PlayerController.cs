﻿using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ArrowGame
{
    public class PlayerController : Unit
    {
        [Header("Dash")]
        [SerializeField] private float _DashMultiplier = 5f;
        [SerializeField] private float _DashKnockBackMultiplier = 5f;
        [SerializeField] private float _DashDuration = 0.1f;
        [SerializeField] private float _DashingCooldown = 1f;

        [Space(10)]
        [Header("Shooting")]
        [SerializeField] private GameObject _BulletPrefab;
        [SerializeField] private Transform _BulletSpawn;
        [SerializeField] private float _ShootingCooldown = 1f;
        [SerializeField] private float _PushBackForce = 1500f;

        private GameObject _SpawnnedBullet;

        private PlayerState playerState = PlayerState.None;
        private PlayerLocation playerLocation = PlayerLocation.None;

        private int _CurJumps = 0;
        private DateTime _DashTimer;
        private DateTime _ShootTimer;

        private bool _DashAvailable = true;
        private bool _ShootAvailable = true;

        private float _XInput = 0f;
        private float _RTrigger = 0f;
        public bool _CanMove = true;

        public float Raylength = 1f;

        private int _PushForceMult = 1;
        bool _ShootTrigger = false;

        private void Update()
        {
            GroundedTest();

            if (InputAble)
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    ChangeState(PlayerState.Run);
                }

                if (Input.GetButtonDown("Jump"))
                {
                    ChangeState(PlayerState.Jump);
                }

                if (Input.GetButtonUp("Fire"))
                {
                    ChangeState(PlayerState.Attack);
                }

                if (!_ShootAvailable)
                {
                    if ((DateTime.Now - _ShootTimer).Milliseconds >= _ShootingCooldown)
                    {
                        _ShootAvailable = true;
                    }
                }

            }

        }

        void ChangeState(PlayerState state)
        {
            if (playerState == state)
                return;

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

        void ChangePlayerLocation(PlayerLocation location)
        {
            if (playerLocation == location)
                return;

            switch (location)
            {
                case PlayerLocation.Grounded:
                    _Anim.Play("Idle");
                    _CurJumps = 0;
                    break;
                case PlayerLocation.InAir:
                    break;
            }

            playerLocation = location;
        }

        private void FixedUpdate()
        {
            if (_CanMove && playerState == PlayerState.Run)
            {
                _XInput = Input.GetAxis("Horizontal");

                Move(_XInput);
                if(_XInput != 0)
                {
                    _Anim.Play("Run");
                }
                else
                {
                    _Anim.Play("Idle");
                }
            }

            if(playerState == PlayerState.Jump)
            {
                Jump(1f);
            }
        }

        private void ResetCanMove()
        {
            _CanMove = true;
            //_Dashing = false;
        }



        private void ResetTimeScale()
        {
            Time.timeScale = 1;
        }

        private void GroundedTest()
        {
            RaycastHit hit;
            Vector3 physicsCentre = this.transform.position + this.GetComponent<BoxCollider2D>().bounds.center;
            Vector3 physicsLeft = physicsCentre - new Vector3(0.3f, 0, 0);
            Vector3 physicsRight = physicsCentre + new Vector3(0.3f, 0, 0);

            if ((Physics.Raycast(physicsLeft, Vector3.down, out hit, Raylength)) || (Physics.Raycast(physicsRight, Vector3.down, out hit, Raylength)))
            {
                if (hit.transform.gameObject.CompareTag("Platform") || hit.transform.gameObject.CompareTag("Pool"))
                {
                    ChangePlayerLocation(PlayerLocation.Grounded);
                }
                else
                {
                    ChangePlayerLocation(PlayerLocation.InAir);
                }
            }
            else
            {
                ChangePlayerLocation(PlayerLocation.InAir);
            }
        }


        #region Unused code
#if false
        
        /*public bool _Dashing = false;
        private void Dash()
        {
            if (_XInput == 0)
            {
                if (IsRotated)
                    _XInput = -1;
                else
                    _XInput = 1;
            }

            Move(_XInput, _DashMultiplier);
            _CanMove = false;
            _Dashing = true;
            Invoke("ResetCanMove", _DashDuration);

            _DashAvailable = false;
            _DashTimer = DateTime.Now;
        }*/

        /*private void Shoot(GameObject Bullet)
        {
            if (_ShootAvailable)
            {
                _SpawnnedBullet = Instantiate(Bullet, _BulletSpawn.position, _BulletSpawn.rotation);

                _ShootTimer = DateTime.Now;
                _ShootAvailable = false;
            }
        }*/

        /*private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Platform")
            {
                RayCastJumpTest();
            }
        }*/

        /*private void RayCastJumpTest()
        {
            RaycastHit hit;
            Vector3 physicsCentre = this.transform.position + this.GetComponent<BoxCollider2D>().bounds.center;
            Vector3 physicsLeft = physicsCentre - new Vector3(0.3f, 0, 0);
            Vector3 physicsRight = physicsCentre + new Vector3(0.3f, 0, 0);

            if ((Physics.Raycast(physicsLeft, Vector3.down, out hit, Raylength)) || (Physics.Raycast(physicsRight, Vector3.down, out hit, Raylength)))
            {
                if (hit.transform.gameObject.CompareTag("Platform") || hit.transform.gameObject.CompareTag("Pool") || hit.transform.gameObject.CompareTag("TuTPool"))
                {
                    ChangePlayerLocation(PlayerLocation.Grounded);
                }
                else
                {
                    ChangePlayerLocation(PlayerLocation.InAir);
                }
            }
            else
            {
                ChangePlayerLocation(PlayerLocation.InAir);
            }
        }*/
#endif
        #endregion

    }
}