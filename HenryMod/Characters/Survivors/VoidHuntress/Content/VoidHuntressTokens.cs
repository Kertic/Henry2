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
            Log.Warning("[VoidHuntress Tokens]");
            Log.Warning(VOID_COLOR);
            Log.Warning(VOID_TEXT);
            Log.Warning(ColorUtility.ToHtmlStringRGB(VOID_COLOR));
            string prefix = VoidHuntressSurvivor.VOIDHUNTRESS_PREFIX;

            string desc = "The Void Huntress is an archer that relies on maintaining it's Void state as long as it can before eventually returning to it's mundane state <color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Maintain your connection to the void as long as you can while under it's influence, but eventually the " + VOID_SICKNESS + " will overwelm you and push you back into normality" + Environment.NewLine + Environment.NewLine
             + "< ! > Use your Bow to gain Corruption while enabling your Bow's Secondary" + Environment.NewLine + Environment.NewLine
             + "< ! > Your Secondary can help you gain more Corruption or maintain it while in Void Form." + Environment.NewLine + Environment.NewLine
             + "< ! > Your Special will commit to your Void Form, where your abilities will change." + Environment.NewLine + Environment.NewLine;

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
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "You gain Corruption as you fire your Bow, which enables you to enter Void Form. You can maintain it indefinitely, but the " + VOID_SICKNESS + " will eventually cause you to revert as it increases the speed of decay");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_1_NAME", "Bow shot");
            Language.Add(prefix + "PRIMARY_1_DESCRIPTION", $"Fire your bow, dealing <style=cIsDamage>{100f * VoidHuntressStaticValues.primaryBowDamageCoefficient}% damage and generating {VoidHuntressStaticValues.primaryBowLunarInsightStacks} lunar insight stacks</style>. If you have Quick Shot, you charge up instantly and reduce your cooldowns by 1 second.");
            #endregion

            #region Primary2
            Language.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            Language.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Tokens.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * VoidHuntressStaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_1_NAME", "");
            Language.Add(prefix + "SECONDARY_1_DESCRIPTION", Tokens.agilePrefix + $"Dash quickly to evade attacks, and gain 1 Quick Shot");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * VoidHuntressStaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(Henry2MasteryAchievement.identifier), "Henry: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(Henry2MasteryAchievement.identifier), "As Henry, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
