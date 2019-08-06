using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    /// <summary>
    /// A test monster class till we define real monster properties
    /// </summary>
    public class SimpleMonster : Monster
    {
        protected override void InitializeMonster()
        {
            monsterType = MonsterType.SimpleMonster;
        }

        protected override void OnMonsterDestroyed()
        {
            throw new System.NotImplementedException();
        }
    }
}