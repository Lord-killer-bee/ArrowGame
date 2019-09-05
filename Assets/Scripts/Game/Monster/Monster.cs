using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// Base class defining common properties for a monster
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Monster : MonoBehaviour
    {
        public MonsterType monsterType { get; protected set; }

        protected bool monsterInitialized = false;
        protected Rigidbody2D rigidBody;

        private void Awake()
        {
            try
            {
                InitializeMonster();
                rigidBody = GetComponent<Rigidbody2D>();
                monsterInitialized = true;
            }
            catch(Exception e)
            {
                Debug.LogError("Monster "+ name +" has not been initialized");
                monsterInitialized = false;
            }
        }

        private void OnDestroy()
        {
            OnMonsterDestroyed();
        }

        #region Abstract methods

        /// <summary>
        /// Initializes the monster when it is created
        /// </summary>
        protected abstract void InitializeMonster();

        /// <summary>
        /// Cleanes up any data used by this monster
        /// </summary>
        protected abstract void OnMonsterDestroyed();

        #endregion
    }
}