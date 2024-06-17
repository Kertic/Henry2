using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressBuffs
    {
        // armor buff gained during roll
        public static BuffDef voidSicknessBuff;
        public static BuffDef lunarInsight;

        public static void Init()
        {
            voidSicknessBuff = Modules.Content.CreateAndAddBuff(
                "VoidSicknessBuff",
                Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion().iconSprite,
                Color.magenta,
                true,
                true);

            lunarInsight = Modules.Content.CreateAndAddBuff(
                "VoidHuntressLunarInsight",
                 Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite,
                 Color.blue,
                 true,
                 false);

        }
    }
}
