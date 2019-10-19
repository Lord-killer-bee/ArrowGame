using ArrowGame.InGameEvents;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// Base class for all abilities
    /// </summary>
    public class PlayerAbility : MonoBehaviour
    {
        public PlayerAbilityType abilityType { get; protected set; }
        public AbilityActivationMode abilityActivationMode { get; protected set; }

        public bool isActive { get; private set; }

        protected PlayerController player;
        protected AbilityConfig abilityConfig;

        protected float abilityDuration;
        private DateTime abilityTimeTracker;

        public virtual void InitializeAbility(AbilityConfig abilityConfig)
        {
            player = GetComponent<PlayerController>();

            if (abilityActivationMode == AbilityActivationMode.AutoActivation)
                ActivateAbility();
        }

        public virtual void ActivateAbility()
        {
            isActive = true;
            abilityTimeTracker = DateTime.Now;
            GameEventManager.Instance.TriggerSyncEvent(new AbilityActivatedEvent(abilityType, abilityDuration));
        }

        private void Update()
        {
            if ((DateTime.Now - abilityTimeTracker).TotalMilliseconds >= abilityDuration * 1000)
            {
                DeactivateAbility();
            }
        }

        public virtual void DeactivateAbility()
        {
            isActive = false;
            Destroy(this);
        }

        public void RestoreTheAbilityDuration()
        {
            abilityTimeTracker = DateTime.Now;
            GameEventManager.Instance.TriggerSyncEvent(new AbilityActivatedEvent(abilityType, abilityDuration));
        }
    }
}