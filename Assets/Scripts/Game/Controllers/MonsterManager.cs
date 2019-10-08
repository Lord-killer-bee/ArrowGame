using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using ArrowGame.InGameEvents;
using System;

namespace ArrowGame {
    /// <summary>
    /// Responsible for handling the monsters, their life time and setting them up
    /// </summary>
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private GameObject _MonsterPrefab;

        private void OnEnable()
        {
            GameEventManager.Instance.AddListener<CreateMonsterEvent>(CreateMonsterCallback);
        }

        private void OnDisable()
        {
            GameEventManager.Instance.RemoveListener<CreateMonsterEvent>(CreateMonsterCallback);
        }

        /// <summary>
        /// Creates a monster when the player misses a bullet
        /// </summary>
        /// <param name="eve"></param>
        private void CreateMonsterCallback(CreateMonsterEvent eve)
        {
            Instantiate(_MonsterPrefab, eve.spawnLocation, Quaternion.identity);
        }
    }
}