using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowGame
{
    public class AbilityTrayItem : MonoBehaviour
    {
        AbilityTrayUI trayUI;
        Image timerImage;

        bool timerStart = false;
        float duration = 0;

        DateTime startTime;

        public void Initialize(AbilityTrayUI parentUI)
        {
            trayUI = parentUI;
        }

        public void StartTimer(float duration)
        {
            this.duration = duration;
            timerStart = true;
            startTime = DateTime.Now;
        }

        private void Update()
        {
            if (timerStart)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds >= duration * 1000)
                {
                    timerStart = false;
                }
                else
                {
                    timerImage.fillAmount = (float)(DateTime.Now - startTime).TotalMilliseconds / duration * 1000;
                }
            }
        }
    }
}