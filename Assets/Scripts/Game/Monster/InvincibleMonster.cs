using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class InvincibleMonster : Monster
    {
        #region parameters


        #endregion

        //public GameObject[] Minions;
        public List<Transform> Targets;

        private Animator _Anim;
        public ParticleSystem Effect;

        public bool CanDie = false;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.InvincibleMonster;
            _Anim = GetComponent<Animator>();
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

            for (int i = 0; i < Targets.Count; i++)
            {
                if(Targets[i] == null)
                {
                    Targets.Remove(Targets[i]);
                }
            }

            if(Targets.Count == 0)
            {
                CanDie = true;
            }
            else
            {
                CanDie = false;
            }

        }

        private void OnTriggerStay2D(Collider2D other) 
        {
            // Shooting Code
            if(other.gameObject.CompareTag("Player"))
            {
                Effect.Play();
            } 

            else
            {
                Effect.Stop();
            } 
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Effect.Stop();
            } 
        }

        private void OnCollisionEnter2D(Collision2D other) 
        {
            if(other.collider.tag == "Platform" || other.collider.tag == "Bullet" || other.collider.tag == "Player")
            {

            }

            else
            {
              

            }
            
        }
    }
}