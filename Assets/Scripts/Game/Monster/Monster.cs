using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// Base class defining common properties for a monster
    /// </summary>
    public abstract class Monster : MonoBehaviour
    {
        public MonsterType monsterType { get; protected set; }

        private void Awake()
        {
            InitializeMonster();
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