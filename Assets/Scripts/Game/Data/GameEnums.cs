﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationState
{
    None = 0,
    Idle,
    Run,
    Jump,
    Attack,
    Death
}

public enum MonsterState
{
    None = 0,
    Run,
    Attack,
    Dizzy
}

public enum PlayerLocation
{
    None,
    Grounded,
    InAir
}

public enum MonsterType
{
    None = 0,
    SimpleMonster,
    MovingMonster,
    BlockingMonster,
    RammingMonster,
    InvincibleMonster,
    InvincibleMinion
}

public enum PlayerAbilityType
{
    None = 0,
    SpeedMultiplier,
    DoubleJump,
    Invincibility
}

public enum AbilityActivationMode
{
    None,
    AutoActivation,
    ManualActivation
}

public enum ObjectType
{
    None,
    Player
}