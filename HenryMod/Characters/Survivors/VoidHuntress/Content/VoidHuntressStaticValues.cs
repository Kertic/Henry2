﻿using System;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressStaticValues
    {
        public const float swordDamageCoefficient = 2.8f;

        public const float primaryBowDamageCoefficient = 4.2f;
        public const float primaryAttackCDRInSeconds = 1.0f;
        public const float voidPrimaryAttackCDRInSeconds = 1.0f;
        public const float primaryBowMovementSlowPenalty = 0.7f;
        public const float primaryBowVoidMeterGain = 15.0f;

        public const float multiShotBowDamageCoefficient = primaryBowDamageCoefficient / 3.0f;
        public const float specialBowVoidMeterGain = 25.0f;
        public const float bombDamageCoefficient = 16f;


        public const int barrageStacks = 6;
    }
}