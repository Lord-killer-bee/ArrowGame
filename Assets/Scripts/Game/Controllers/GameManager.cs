using ArrowGame.InGameEvents;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArrowGame
{
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            AddListeners();

            DontDestroyOnLoad(this);
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            GameEventManager.Instance.AddListener<PlayerKilledEvent>(OnPlayerDeath);
        }

        private void RemoveListeners()
        {
            GameEventManager.Instance.RemoveListener<PlayerKilledEvent>(OnPlayerDeath);
        }

        private void OnPlayerDeath(PlayerKilledEvent e)
        {
            Invoke("ReloadLevel", 1f);
        }

        void ReloadLevel()
        {
            SceneManager.LoadScene(0);
        }
    }
}