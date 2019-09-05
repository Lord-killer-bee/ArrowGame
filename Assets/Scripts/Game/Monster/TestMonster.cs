using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class TestMonster : Monster
    {
        protected override void InitializeMonster()
        {
            monsterType = MonsterType.None;
        }

        protected override void OnMonsterDestroyed()
        {
            throw new System.NotImplementedException();
        }

    }
}