using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame.InGameEvents
{
    /// <summary>
    /// Event called by the bullet if it misses
    /// </summary>
    public class CreateMonsterEvent: GameEvent
    {
        public Vector3 spawnLocation;
        public MonsterType monsterType;

        public CreateMonsterEvent(Vector3 spawnLocation, MonsterType monsterType)
        {
            this.spawnLocation = spawnLocation;
            this.monsterType = monsterType;
        }
    }

    public class GrantAbilityEvent : GameEvent
    {
        public PlayerAbilityType abilityType;

        public GrantAbilityEvent(PlayerAbilityType abilityType)
        {
            this.abilityType = abilityType;
        }
    }

    public class ActivationPointInRangeEvent : GameEvent
    {
        public ActivationPoint activationPoint;
        public bool inRange;

        public ActivationPointInRangeEvent(ActivationPoint activationPoint, bool inRange)
        {
            this.activationPoint = activationPoint;
            this.inRange = inRange;
        }
    }

    public class PlayerKilledEvent : GameEvent
    {
        public PlayerKilledEvent() { }
    }
}