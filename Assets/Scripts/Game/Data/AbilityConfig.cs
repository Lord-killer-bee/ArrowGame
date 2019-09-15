using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// Responsible to hold all the ability parameters.
    /// Loaded when the player is created
    /// </summary>
    [CreateAssetMenu(menuName = "Ability configuration")]
    public class AbilityConfig : ScriptableObject
    {
        #region private attributes

        [Header("Double jump ability")]
        [SerializeField] private AbilityActivationMode doubleJumpActivationType = AbilityActivationMode.None;
        [SerializeField] private int jumpCount = 2;
        [SerializeField] private float jumpAbilityDuration = 2;

        [Header("Speed multiplier ability")]
        [SerializeField] private AbilityActivationMode speedMultiplierActivationType = AbilityActivationMode.None;
        [SerializeField] private float speedMultiplier = 2;
        [SerializeField] private float speedMultiplierDuration = 2;

        [Header("Invincibility ability")]
        [SerializeField] private AbilityActivationMode invincibilityActivationType = AbilityActivationMode.None;
        [SerializeField] private float invincibilityDuration = 2;

        #endregion

        #region public getters

        public AbilityActivationMode DoubleJumpActivationType
        {
            get { return doubleJumpActivationType; }
            private set { }
        }

        public int JumpCount
        {
            get { return jumpCount; }
            private set { }
        }

        public float JumpAbilityDuration
        {
            get { return jumpAbilityDuration; }
            private set { }
        }

        public AbilityActivationMode SpeedMultiplierActivationType
        {
            get { return speedMultiplierActivationType; }
            private set { }
        }

        public float SpeedMultiplier
        {
            get { return speedMultiplier; }
            private set { }
        }

        public float SpeedMultiplierDuration
        {
            get { return speedMultiplierDuration; }
            private set { }
        }

        public AbilityActivationMode InvincibilityActivationType
        {
            get { return invincibilityActivationType; }
            private set { }
        }

        public float InvincibilityDuration
        {
            get { return invincibilityDuration; }
            private set { }
        }

        #endregion
    }
}