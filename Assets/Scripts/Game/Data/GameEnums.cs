using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Run,
    Jump,
    Dash
}

public enum PlayerLocation
{
    Grounded,
    InAir
}

public enum MonsterType
{
    None = 0,
    SimpleMonster
}