﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowGame
{
    public class GameConsts
    {
        #region Resources file paths

        public const string AbilityConfigPath = "Data/AbilityConfig";

        #endregion

        #region Layers

        public const int InvincibilityLayer = 9;
        public const int PlayerLayer = 10;

        #endregion

        #region Tags

        public const string BULLET_TAG = "Bullet";
        public const string MONSTER_TAG = "Monster";
        public const string PLATFORM_TAG = "Platform";
        public const string PLATFORM_BOTTOM_TAG = "PlatformBottom";
        public const string PLAYER_TAG = "Player";
        public const string BLOCK_TAG = "Block";

        #endregion

        #region Input keys

        public const string HORIZONTAL_CODE = "Horizontal";
        public const string VERTICAL_CODE = "Vertical";
        public const string FIRE_CODE = "Fire";
        public const string JUMP_CODE = "Jump";
        public const string ABILITY_ACTIVATE_CODE = "AbilityActivate";
        public const string ABILITY_SWITCH_CODE = "AbilitySwitch";

        #endregion
    }
}