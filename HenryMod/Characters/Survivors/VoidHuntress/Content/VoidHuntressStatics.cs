using System;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressStatics
    {
        public const float transitionDuration = 1.5f;

        public const float lunarBowDmgCoeff = 8f;
        public const float lunarBowFlurryDmgCoeff = lunarBowDmgCoeff * 0.25f;
        public const float lunarBowAttackCDRInSeconds = 1.0f;
        public const float lunarBowMovementSlowPenalty = 0.7f;
        public const float lunarBowVoidMeterGain = 12.0f;
        public const int arrowFlurryStacks = 6;

        public const float voidBowDmgCoeff = 1.5f;
        public const float voidBowVoidMeterGain = 1.0f;
        public const float voidBowAttackCDRInSeconds = 1.0f;
        public const float voidMultishotDmgCoeff = voidBowDmgCoeff * 0.8f;

        public const float bombVoidMeterGain = 25.0f;
        public const float bombDamageCoefficient = 16f;

        public const float healthPerVoidMeter = 1.0f;
    }
}