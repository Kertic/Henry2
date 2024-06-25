using System;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressStatics
    {
        public const float swordDamageCoefficient = 2.8f;

        public const float lunarBowDmgCoeff = 8f;
        public const float lunarBowAttackCDRInSeconds = 1.0f;
        public const float lunarBowMovementSlowPenalty = 0.7f;
        public const float lunarBowVoidMeterGain = 12.0f;

        public const float voidPrimaryAttackCDRInSeconds = 1.0f;
        public const float specialBowVoidMeterGain = 25.0f;

        public const float multiShotBowDamageCoefficient = lunarBowDmgCoeff / 3.0f;
        public const float bombDamageCoefficient = 16f;

        public const int arrowFlurryStacks = 6;
    }
}