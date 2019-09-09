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
        [SerializeField] private float moveDistance;

        #endregion

        private int moveDirection = 1;// 1 means right, -1 means left
        private Vector3 InitialLocation;

        protected override void InitializeMonster()
        {
            monsterType = MonsterType.SimpleMonster;

            InitialLocation = transform.position;
        }

        /// <summary>
        /// Destroy stuff here like particle effects or any residual bullet objects
        /// </summary>
        protected override void OnMonsterDestroyed()
        {
            //Broadcast an event to the player to activate an effect
        }

        public float Raylength = 5f;
        public Transform RayStart;
        public Transform RayEnd;
        private float DoubleSpeed = 1;

        private void FixedUpdate()
        {
            if (!monsterInitialized)
                return;

            if(Vector3.Distance(transform.position, InitialLocation) >= moveDistance)
            {
                moveDirection *= -1;
                Vector3 MonsterScale = transform.localScale;
                MonsterScale.x *= -1;
                transform.localScale = MonsterScale;

                InitialLocation = transform.position;
            }
           
            var vel = new Vector2(moveDirection, 0) * moveSpeed * DoubleSpeed * Time.fixedDeltaTime;
            rigidBody.velocity = vel;

            RaycastHit2D hit = Physics2D.Raycast(RayStart.position, RayEnd.position - RayStart.position, Vector3.Distance(RayEnd.position, RayStart.position));
            Debug.DrawLine(RayStart.position, RayEnd.position, Color.red);
            
            if(hit.collider.tag == "Player")
            {
                DoubleSpeed = 3;
            }
            else
            {
                DoubleSpeed = 1;
            }
        }
    }
}