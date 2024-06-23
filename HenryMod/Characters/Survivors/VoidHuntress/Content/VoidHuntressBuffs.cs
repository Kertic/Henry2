using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressBuffs
    {
        // armor buff gained during roll
        public static BuffDef voidSicknessBuff;
        public static BuffDef quickShot;
        public static BuffDef adsUptime;

        public static int quickShotMaxStacksPerSecondaryCharge = 3;

        public static void Init()
        {
            voidSicknessBuff = Modules.Content.CreateAndAddBuff(
                "VoidSicknessBuff",
                Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion().iconSprite,
                Color.magenta,
                true,
                true);

            quickShot = Modules.Content.CreateAndAddBuff(
                "VoidHuntressLunarInsight",
                 Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite,
                 Color.blue,
                 true,
                 false);

            adsUptime = Modules.Content.CreateAndAddBuff(
                "VoidHuntressAdsUptime",
                 Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite,
                 Color.blue,
                 true,
                 false);

        }
    }
}
