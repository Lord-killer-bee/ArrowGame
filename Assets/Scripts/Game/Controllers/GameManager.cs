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
        [SerializeField] private float roundTime = 100;

        DateTime roundStartTime;

        bool isRoundActive = false;

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
            GameEventManager.Instance.AddListener<GameStartEvent>(OnGameStarted);
        }

        private void RemoveListeners()
        {
            GameEventManager.Instance.RemoveListener<PlayerKilledEvent>(OnPlayerDeath);
            GameEventManager.Instance.RemoveListener<GameStartEvent>(OnGameStarted);
        }

        private void OnPlayerDeath(PlayerKilledEvent e)
        {
            Invoke("ReloadLevel", 1f);
        }

        void ReloadLevel()
        {
            SceneManager.LoadScene(0);
        }

        private void OnGameStarted(GameStartEvent e)
        {
            roundStartTime = DateTime.Now;
            isRoundActive = true;
        }

        private void Update()
        {
            if((DateTime.Now - roundStartTime).TotalMilliseconds >= roundTime * 1000)
            {
                isRoundActive = false;
                GameEventManager.Instance.TriggerSyncEvent(new RoundCompletedEvent());
            }
        }
    }
}