using Core;
using ArrowGame.InGameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartGame()
        {
            GameEventManager.Instance.TriggerSyncEvent(new GameStartEvent());

            gameObject.SetActive(false);
        }
    }
}