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

        public CreateMonsterEvent(Vector3 spawnLocation)
        {
            this.spawnLocation = spawnLocation;
        }
    }
}