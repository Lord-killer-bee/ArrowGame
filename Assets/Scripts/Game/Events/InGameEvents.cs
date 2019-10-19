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

    /// <summary>
    /// Sent when an ability is colected by the player
    /// </summary>
    public class GrantAbilityEvent : GameEvent
    {
        public PlayerAbilityType abilityType;

        public GrantAbilityEvent(PlayerAbilityType abilityType)
        {
            this.abilityType = abilityType;
        }
    }

    /// <summary>
    /// Sent when an ability is activated by the player
    /// </summary>
    public class AbilityActivatedEvent : GameEvent
    {
        public PlayerAbilityType abilityType;
        public float duration;

        public AbilityActivatedEvent(PlayerAbilityType abilityType, float duration)
        {
            this.abilityType = abilityType;
            this.duration = duration;
        }
    }

    /// <summary>
    /// Sent when an ability duration is completed
    /// </summary>
    public class AbilityDeactivatedEvent : GameEvent
    {
        public PlayerAbilityType abilityType;

        public AbilityDeactivatedEvent(PlayerAbilityType abilityType)
        {
            this.abilityType = abilityType;
        }
    }

    /// <summary>
    /// Sent when an activation point went in or out of player's range
    /// </summary>
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