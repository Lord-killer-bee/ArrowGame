using ArrowGame.InGameEvents;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class InGameUIManager : MonoBehaviour
    {
        #region UI Classes

        [SerializeField] AbilityTrayUI abilityTrayUI;

        #endregion

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            GameEventManager.Instance.AddListener<GrantAbilityEvent>(OnAbilityGranted);
            GameEventManager.Instance.AddListener<AbilityActivatedEvent>(OnAbilityActivated);
            GameEventManager.Instance.AddListener<AbilityDeactivatedEvent>(OnAbilityDeactivated);
        }

        private void RemoveListeners()
        {
            GameEventManager.Instance.RemoveListener<GrantAbilityEvent>(OnAbilityGranted);
            GameEventManager.Instance.AddListener<AbilityActivatedEvent>(OnAbilityActivated);
            GameEventManager.Instance.AddListener<AbilityDeactivatedEvent>(OnAbilityDeactivated);
        }

        #region Event Listeners

        /// <summary>
        /// Add the ability in the UI
        /// </summary>
        /// <param name="e"></param>
        private void OnAbilityGranted(GrantAbilityEvent e)
        {
            abilityTrayUI.AddNewAbility(e.abilityType);
        }

        /// <summary>
        /// Indicate tha the ability is currently active
        /// </summary>
        /// <param name="e"></param>
        private void OnAbilityActivated(AbilityActivatedEvent e)
        {
            abilityTrayUI.AbilityActivated(e.abilityType, e.duration);
        }

        /// <summary>
        /// Remove the ability
        /// </summary>
        /// <param name="e"></param>
        private void OnAbilityDeactivated(AbilityDeactivatedEvent e)
        {
            abilityTrayUI.RemoveAbility(e.abilityType);
        }

        #endregion

    }
}