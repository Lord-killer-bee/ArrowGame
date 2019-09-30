using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class RammingMonster : Monster
    {
        #region parameters

        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveDistance;

        public float Raylength = 5f;
        public Transform RayStart;
        public Transform RayEnd;
        private float SpeedMultiplier = 1;
        private float DistanceMultiplier = 1;

        #endregion

        private Animator _Anim;

        private int moveDirection = 1;// 1 means right, -1 means left
        private Vector3 InitialLocation;

        private MonsterState monsterState = MonsterState.Run;
        public bool CanDie = false;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.RammingMonster;

            InitialLocation = transform.position;
            //_RB = GetComponent<Rigidbody2D>();
            _Anim = GetComponent<Animator>();
            //_collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Destroy stuff here like particle effects or any residual bullet objects
        /// </summary>
        protected override void OnMonsterDestroyed()
        {
            //Broadcast an event to the player to activate an effect
        }

        void ChangeState(MonsterState state)
        {
            if (monsterState == state)
                return;

            OnExitState(state);

            switch (state)
            {
                case MonsterState.Run:
                    _Anim.Play("RammingMonster");
                    CanDie = false;
                    break;
                
                case MonsterState.Attack:
                    _Anim.Play("RammingMonster_Dashing");
                    CanDie = false;
                    break;

                case MonsterState.Dizzy:
                    _Anim.Play("RammingMonster_Dizzy");
                    CanDie = true;
                    break;
            }

            monsterState = state;
        }

        private void OnExitState(MonsterState state)
        {
            switch (state)
            {
                case MonsterState.Run:
                    //currentMoveSpeed = 0;
                    break;
            }
        }

        public float DizzyTime = 5f;
        private float DizzyCountDown = 0f;

        private void FixedUpdate()
        {
            if (!monsterInitialized)
                return;

            if(Vector3.Distance(transform.position, InitialLocation) >= moveDistance*DistanceMultiplier)
            {
                FlipTheMonster();
            }
           
            var vel = new Vector2(moveDirection, 0) * moveSpeed * SpeedMultiplier * Time.fixedDeltaTime;
            vel.y = rigidBody.velocity.y;
            rigidBody.velocity = vel;

            RaycastHit2D hit = Physics2D.Raycast(RayStart.position, RayEnd.position - RayStart.position, Vector3.Distance(RayEnd.position, RayStart.position));
            Debug.DrawLine(RayStart.position, RayEnd.position, Color.red);
            
            if(hit.collider != null)
            {
                if(hit.collider.tag == "Player")
                {
                    SpeedMultiplier = 3;
                    DistanceMultiplier = 10;
                    ChangeState(MonsterState.Attack);
                }
            }
            else
            {
                //ChangeState(MonsterState.Run);
                SpeedMultiplier = 1;
                DistanceMultiplier = 1;
            } 

            if(monsterState == MonsterState.Dizzy)
            {
                DizzyCountDown += Time.deltaTime;
                if(DizzyCountDown >= DizzyTime)
                {
                    ChangeState(MonsterState.Run);
                    FlipTheMonster();
                    DizzyCountDown = 0;
                }
            }

            OnPlatformCheck();
        }

        private void FlipTheMonster()
        {
            moveDirection *= -1;
            Vector3 MonsterScale = transform.localScale;
            MonsterScale.x *= -1;
            transform.localScale = MonsterScale;

            InitialLocation = transform.position;
        }

        private bool Flipped = false;
        private void OnPlatformCheck()
        {
            Vector3 Offset = new Vector3(2,0,0);
            RaycastHit2D HitLeft = Physics2D.Raycast(transform.position + Offset, Vector2.down, Raylength);
            RaycastHit2D HitRight = Physics2D.Raycast(transform.position - Offset, Vector2.down, Raylength);
            Debug.DrawRay(transform.position + Offset, Vector2.down * Raylength, Color.red);
            Debug.DrawRay(transform.position - Offset, Vector2.down * Raylength, Color.red);

            if((HitLeft.collider == null || HitRight.collider == null) && !Flipped)
            {
                FlipTheMonster();
                Flipped = true;
            }
            if(HitLeft.collider != null && HitRight.collider != null)
            {
                Flipped = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D other) 
        {
            if(other.collider.tag == "Platform" || other.collider.tag == "Bullet" || other.collider.tag == "Player")
            {

            }

            else
            {
                if(monsterState == MonsterState.Attack)
                ChangeState(MonsterState.Dizzy);

                else
                {
                    FlipTheMonster();
                }
            }
            
        }
    }
}