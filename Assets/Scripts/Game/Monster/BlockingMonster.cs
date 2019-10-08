using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class BlockingMonster : Monster
    {
        #region parameters

        #endregion


        protected override void InitializeMonster()
        {
            monsterType = MonsterType.BlockingMonster;
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
        }

        private void OnCollisionEnter2D(Collision2D other) 
        {
            if(other.collider.tag == GameConsts.BULLET_TAG)
            {
                //TODO : Capture this monster
                Destroy(gameObject);
            }
        }
    }
}