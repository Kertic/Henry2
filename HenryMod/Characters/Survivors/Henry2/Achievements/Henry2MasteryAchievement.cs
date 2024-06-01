using RoR2;
using Henry2Mod.Modules.Achievements;

namespace Henry2Mod.Survivors.Henry.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class Henry2MasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = Henry2Survivor.HENRY_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = Henry2Survivor.HENRY_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => Henry2Survivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}