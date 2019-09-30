using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class InvincibleMinion : Monster
    {
        #region parameters

        [SerializeField] private float moveSpeed;
        [SerializeField] private float moveDistance;

        #endregion

        private int moveDirection = 1;// 1 means right, -1 means left
        private Vector3 InitialLocation;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.InvincibleMinion;

            InitialLocation = transform.position;
        }

        /// <summary>
        /// Destroy stuff here like particle effects or any residual bullet objects
        /// </summary>
        protected override void OnMonsterDestroyed()
        {
            //Broadcast an event to the player to activate an effect
        }

        private void FixedUpdate()
        {
            if (!monsterInitialized)
                return;

            if(Vector3.Distance(transform.position, InitialLocation) >= moveDistance)
            {
                FlipTheMonster();
            }
           
            var vel = new Vector2(moveDirection, 0) * moveSpeed * Time.fixedDeltaTime;
            vel.y = rigidBody.velocity.y;
            rigidBody.velocity = vel;

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
        public float Raylength = 1;
        private void OnPlatformCheck()
        {
            Vector3 Offset = new Vector3(3,0,0);
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
            if(other.collider.tag == "Player" || other.collider.tag == "Platform")
            {

            }
            else
            {
                FlipTheMonster();
            }
        }
    }
}