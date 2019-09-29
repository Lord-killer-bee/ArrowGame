using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class InvincibilityAbility : PlayerAbility
    {
        private Color playerColor;

        public override void InitializeAbility(AbilityConfig abilityConfig)
        {
            this.abilityConfig = abilityConfig;

            abilityType = PlayerAbilityType.Invincibility;
            abilityActivationMode = abilityConfig.InvincibilityActivationType;

            abilityDuration = abilityConfig.InvincibilityDuration;

            base.InitializeAbility(abilityConfig);
        }

        public override void ActivateAbility()
        {
            Physics2D.IgnoreLayerCollision(GameConsts.InvincibilityLayer, GameConsts.PlayerLayer, true);

            playerColor = player.GetComponent<SpriteRenderer>().color;
            playerColor.a = 0.5f;
            player.GetComponent<SpriteRenderer>().color = playerColor;

            base.ActivateAbility();
        }

        public override void DeactivateAbility()
        {
            Physics2D.IgnoreLayerCollision(GameConsts.InvincibilityLayer, GameConsts.PlayerLayer, false);

            playerColor = player.GetComponent<SpriteRenderer>().color;
            playerColor.a = 1f;
            player.GetComponent<SpriteRenderer>().color = playerColor;

            base.DeactivateAbility();
        }
    }
}