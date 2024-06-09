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

        public static void Init()
        {
            voidSicknessBuff = Modules.Content.CreateAndAddBuff(
                "VoidSicknessBuff",
                Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion().iconSprite,
                Color.white,
                false,
                false);

            quickShot = Modules.Content.CreateAndAddBuff(
                "VoidHuntressQuickShot",
                 Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/CritOnUse/bdFullCrit.asset").WaitForCompletion().iconSprite,
                 Color.green,
                 true,
                 false);

        }
    }
}
