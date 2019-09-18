using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class DoubleJumpAbility : PlayerAbility
    {
        public float currentJumps { get; private set; }

        public override void InitializeAbility(AbilityConfig abilityConfig)
        {
            this.abilityConfig = abilityConfig;

            abilityType = PlayerAbilityType.DoubleJump;
            abilityActivationMode = abilityConfig.DoubleJumpActivationType;

            abilityDuration = abilityConfig.JumpAbilityDuration;
            base.InitializeAbility(abilityConfig);
        }

        public override void ActivateAbility()
        {
            currentJumps = abilityConfig.JumpCount;
            base.ActivateAbility();
        }

        public void RegisterJump()
        {
            currentJumps--;
        }

        public void RestoreJumps()
        {
            currentJumps = abilityConfig.JumpCount;
        }
    }
}