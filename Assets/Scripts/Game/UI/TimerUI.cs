using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using ArrowGame.InGameEvents;
using System;

namespace ArrowGame
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] Text timerText;

        bool timerStarted = false;
        DateTime startTime;

        private void Start()
        {
            GameEventManager.Instance.AddListener<GameStartEvent>(OnGameStarted);
            GameEventManager.Instance.AddListener<RoundCompletedEvent>(OnRoundCompleted);
        }

        private void OnDestroy()
        {
            GameEventManager.Instance.RemoveListener<GameStartEvent>(OnGameStarted);
            GameEventManager.Instance.RemoveListener<RoundCompletedEvent>(OnRoundCompleted);
        }

        private void Update()
        {
            if (timerStarted)
            {
                SetText(100 - (float)(DateTime.Now - startTime).TotalMilliseconds / 1000);
            }
        }

        //TODO: Later create a game rules info config
        private void OnGameStarted(GameStartEvent e)
        {
            timerText.gameObject.SetActive(true);
            timerStarted = true;
            SetText(100);
            startTime = DateTime.Now;
        }

        private void OnRoundCompleted(RoundCompletedEvent e)
        {
            timerStarted = false;
            timerText.gameObject.SetActive(false);
        }

        void SetText(float timeStamp)
        {
            timerText.text = timeStamp.ToString();
        }
    }
}