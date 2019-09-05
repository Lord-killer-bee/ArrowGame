using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class MovingMonster : Monster
    {
        #region parameters

        [SerializeField] private float moveSpeed;
        [SerializeField] private float oscillateTime;

        [SerializeField] private bool Horizontal = true;
        [SerializeField] private bool Vertical = false;

        #endregion

        private int moveDirection = 1;// 1 means right, -1 means left
        private DateTime moveStartTime;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.MovingMonster;

            moveStartTime = DateTime.Now;
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

            if((DateTime.Now - moveStartTime).TotalMilliseconds >= oscillateTime * 1000)
            {
                moveDirection *= -1;
                Vector3 MonsterScale = transform.localScale;
                MonsterScale.x *= -1;
                transform.localScale = MonsterScale;
                moveStartTime = DateTime.Now;
            }

            Vector2 vel;
            if(Horizontal)
            {
                vel = new Vector2(moveDirection, 0) * moveSpeed * Time.fixedDeltaTime;
                rigidBody.velocity = vel;
            }

            if(Vertical)
            {
                vel = new Vector2(0, moveDirection) * moveSpeed * Time.fixedDeltaTime;
                rigidBody.velocity = vel;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            print("collided");
            if(other.transform.CompareTag("Player"))
            {
                if(other.contacts[0].normal == Vector3.down)
                other.transform.SetParent(transform);	
            }	
        }

        private void OnCollisionExit(Collision other)
        {
            if(other.transform.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }	
        }
    }
}