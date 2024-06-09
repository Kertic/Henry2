using Henry2Mod.Survivors.Henry.Achievements;
using RoR2;
using UnityEngine;

namespace Henry2Mod.Survivors.Henry
{
    public static class Henry2Unlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                Henry2MasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(Henry2MasteryAchievement.identifier),
                Henry2Survivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
