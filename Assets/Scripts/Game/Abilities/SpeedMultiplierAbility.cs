using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class SpeedMultiplierAbility : PlayerAbility
    {
        public override void InitializeAbility(AbilityConfig abilityConfig)
        {
            this.abilityConfig = abilityConfig;

            abilityType = PlayerAbilityType.SpeedMultiplier;
            abilityActivationMode = abilityConfig.SpeedMultiplierActivationType;

            abilityDuration = abilityConfig.SpeedMultiplierDuration;
            
            base.InitializeAbility(abilityConfig);
        }

        public override void ActivateAbility()
        {
            player.MultiplyMoveSpeed(abilityConfig.SpeedMultiplier);
            base.ActivateAbility();
        }

        public override void DeactivateAbility()
        {
            player.MultiplyMoveSpeed(1);
            base.DeactivateAbility();
        }
    }
}