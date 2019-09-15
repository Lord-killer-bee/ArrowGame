using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class DoubleJumpAbility : PlayerAbility
    {
        public float currentJumps { get; private set; }
        private float abilityDuration;

        private DateTime abilityTimeTracker;

        public override void InitializeAbility(AbilityConfig abilityConfig)
        {
            this.abilityConfig = abilityConfig;

            abilityType = PlayerAbilityType.DoubleJump;
            abilityActivationMode = abilityConfig.DoubleJumpActivationType;

            currentJumps = abilityConfig.JumpCount;
            abilityDuration = abilityConfig.JumpAbilityDuration;
            abilityTimeTracker = DateTime.Now;
            base.InitializeAbility(abilityConfig);
        }

        public void RegisterJump()
        {
            currentJumps--;
        }

        public void RestoreJumps()
        {
            currentJumps = abilityConfig.JumpCount;
        }

        private void Update()
        {
            if((DateTime.Now - abilityTimeTracker).TotalMilliseconds >= abilityDuration * 1000)
            {
                DeactivateAbility();
            }
        }
    }
}