using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class SimpleMonster : Monster
    {
        #region parameters

        [SerializeField] private float moveSpeed;
        [SerializeField] private float oscillateTime;

        #endregion

        private int moveDirection = 1;// 1 means right, -1 means left
        private DateTime moveStartTime;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.SimpleMonster;

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
                moveStartTime = DateTime.Now;
            }

            var vel = new Vector2(moveDirection, 0) * moveSpeed * Time.fixedDeltaTime;
            rigidBody.velocity = vel;
        }
    }
}