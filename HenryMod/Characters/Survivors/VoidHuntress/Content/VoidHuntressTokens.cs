using System;
using Henry2Mod.Modules;
using Henry2Mod.Survivors.Henry.Achievements;
using UnityEngine;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressTokens
    {

        public static Color VOID_COLOR = new Color(
                r: 141.0f / 256.0f,
                g: 100.0f / 256.0f,
                b: 222.0f / 256.0f);

        public static string VOID_TEXT = "<color=#" + ColorUtility.ToHtmlStringRGB(VOID_COLOR) + ">";
        public static string GREY_TEXT = "<color=#CCD3E0>";
        public static string WHITE_TEXT = "<color=#FFFFFF>";
        public static string VOID_SICKNESS = VOID_TEXT + "Void Sickness" + GREY_TEXT;
        public static void Init()
        {
            AddVoidHuntressTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddVoidHuntressTokens()
        {
            string prefix = VoidHuntressSurvivor.VOIDHUNTRESS_PREFIX;

            string desc = "The Void Huntress is an archer that relies on maintaining it's Void state as long as it can before eventually returning to it's mundane state <color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Maintain your connection to the void as long as you can while under it's influence, but eventually the " + VOID_SICKNESS + " will overwelm you and push you back into normality" + Environment.NewLine + Environment.NewLine
             + "< ! > Use your Bow to gain Corruption." + Environment.NewLine + Environment.NewLine
             + "< ! > Your Secondary allows you to reposition quickly and take a better vantage point." + Environment.NewLine + Environment.NewLine
             + "< ! > Your Utility increases your attack speed and allows you to fire in rapid successsion." + Environment.NewLine + Environment.NewLine
             + "< ! > Your Special will commit to your Void Form, where your abilities will change." + Environment.NewLine + Environment.NewLine

             + $"{VOID_TEXT}< ! > Void Form" + Environment.NewLine + Environment.NewLine
             + "< ! > Your Bow tracks targets and fires rapidly, and using an ability will enhance your bow to reduce your cooldowns" + Environment.NewLine + Environment.NewLine
             + "< ! > Your Secondary is much quicker and allows you to dodge enemies while staying in the fray." + Environment.NewLine + Environment.NewLine
             + "< ! > Your Utility will fire arrows rapidly, increasing it's potency over time." + Environment.NewLine + Environment.NewLine
             + $"< ! > Your Special will succumb to {VOID_SICKNESS} and convert remaining Corruption into some health. </color>" + Environment.NewLine + Environment.NewLine;

            string outro = "..and so it left, searching for a new identity.";
            string outroFailure = "..and so it vanished, forever a memory only few will keep.";

            Language.Add(prefix + "NAME", "Void Huntress");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Tainted");
            Language.Add(prefix + "LORE", "The Huntress touched the beyond, and it reached back");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Void Huntress Passive");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "You gain Corruption as you fire your Bow, which enables you to enter Void Form. You can maintain it indefinitely, gaining increasing attack speed, but the " + VOID_SICKNESS + " will eventually cause you to revert as it increases the speed of decay");
            #endregion

            #region Primary
            Language.Add(prefix + "LUNAR_BOW_NAME", "Lunar bow");
            Language.Add(prefix + "LUNAR_BOW_DESCRIPTION", $"Fire your bow, dealing <style=cIsDamage>{100f * VoidHuntressStatics.lunarBowDmgCoeff}% damage and generating {VoidHuntressStatics.lunarBowVoidMeterGain} Corruption</style>. If you have Quick Shot, you charge up instantly.");

            Language.Add(prefix + "VOID_BOW_NAME", "Void Bow");
            Language.Add(prefix + "VOID_BOW_DESCRIPTION", $"Fire your bow, dealing <style=cIsDamage>{100f * VoidHuntressStatics.voidBowDmgCoeff}% damage and generating {VoidHuntressStatics.voidBowVoidMeterGain} Corruption</style>. If you have Void Shot, you reduce your cooldowns.");
            #endregion

            #region Secondary
            Language.Add(prefix + "FLIT_NAME", "Flit");
            Language.Add(prefix + "FLIT_DESCRIPTION", Tokens.agilePrefix + $" Flit in a direction, hovering at the end briefly. You can hit JUMP to leap while hovering. You can hit SECONDARY while in motion to hover whenever you wish. Hitting attacks reduces this cooldown by <style=cIsUtility>{VoidHuntressStatics.lunarBowAttackCDRInSeconds} seconds.</style>" );

            Language.Add(prefix + "VOID_FLIT_NAME", "Void Fade");
            Language.Add(prefix + "VOID_FLIT_DESCRIPTION", Tokens.agilePrefix + $" Dash quickly to evade attacks. Hold JUMP to dash upwards. Recharges all charges at once." );
            #endregion

            #region Utility
            Language.Add(prefix + "ARROW_FLURRY_NAME", "Barrage");
            Language.Add(prefix + "ARROW_FLURRY_DESCRIPTION", $"Activate to allow yourself to rapidly fire <style=cIsUtility>{VoidHuntressStatics.arrowFlurryStacks} arrows, each dealing</style> <style=cIsDamage>{100f * VoidHuntressStatics.lunarBowFlurryDmgCoeff}% damage</style>.");

            Language.Add(prefix + "VOID_ARROW_FLURRY_NAME", "Void Barrage");
            Language.Add(prefix + "VOID_ARROW_FLURRY_DESCRIPTION", $"Activate to rapidly fire all stacks for <style=cIsUtility>{100f * VoidHuntressStatics.voidMultishotDmgCoeff}% damage</style>. Activiating this skill only gives <style=cIsUtility>1 Void Shot</style>");
            #endregion

            #region Special
            Language.Add(prefix + "INVERT_NAME", "Invert");
            Language.Add(prefix + "INVERT_DESCRIPTION", $"Enter Void Form, gaining <style=cIsDamage> 0.75 Attack Speed for each second in Void Form</style>. However, this benefit also increases the rate at which void form fades.");

            Language.Add(prefix + "REFORM_NAME", "Reform");
            Language.Add(prefix + "REFORM_DESCRIPTION", $"Exit Void Form early, gaining <style=cIsHealth> up to {VoidHuntressStatics.healthPerVoidMeter * 100f} health depending on remaining corruption</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(Henry2MasteryAchievement.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(Henry2MasteryAchievement.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
