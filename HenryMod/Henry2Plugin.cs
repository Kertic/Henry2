using BepInEx;
using EntityStates;
using Henry2Mod.Modules;
using Henry2Mod.Survivors.Henry;
using Henry2Mod.Survivors.Henry.SkillStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace Henry2Mod
{
    //[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class Henry2Plugin : BaseUnityPlugin
    {
        // if you do not change this, you are giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.coolb.Henry2Mod";
        public const string MODNAME = "Henry2Mod";
        public const string MODVERSION = "1.0.2";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "COOLB";

        public static Henry2Plugin instance;

        void Awake()
        {
            instance = this;

            //easy to use logger
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            // character initialization
            new Henry2Survivor().Initialize();

            AddExtraSkill();

            // make a content pack and add it. this has to be last
            new Modules.ContentPacks().Initialize();
        }

        void AddExtraSkill()
        {
            // First we must load our survivor's Body prefab. For this tutorial, we are making a skill for Commando
            // If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            // https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            GameObject huntressBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBody.prefab").WaitForCompletion();
            var huntressIcon = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Huntress/texHuntressSkillIcons.png").WaitForCompletion();

            // We use LanguageAPI to add strings to the game, in the form of tokens
            // Please note that it is instead recommended that you use a language file.
            // More info in https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            LanguageAPI.Add("HUNTRESS_PRIMARY_SIMPLEBULLET_NAME", "Simple Bullet Attack");
            LanguageAPI.Add("HUNTRESS_PRIMARY_SIMPLEBULLET_DESCRIPTION", $"Fire a bullet from your right pistol for <style=cIsDamage>300% damage</style>.");

            // Now we must create a SkillDef
            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();

            //Check step 2 for the code of the CustomSkillsTutorial.MyEntityStates.SimpleBulletAttack class


            mySkillDef.activationState = new SerializableEntityStateType(typeof(ChargeArrow));


            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.cancelSprintingOnActivation = true;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            // For the skill icon, you will have to load a Sprite from your own AssetBundle
            mySkillDef.icon = huntressIcon;
            mySkillDef.skillDescriptionToken = "HUNTRESS_PRIMARY_SIMPLEBULLET_DESCRIPTION";
            mySkillDef.skillName = "HUNTRESS_PRIMARY_SIMPLEBULLET_NAME";
            mySkillDef.skillNameToken = "HUNTRESS_PRIMARY_SIMPLEBULLET_NAME";

            // This adds our skilldef. If you don't do this, the skill will not work.
            ContentAddition.AddSkillDef(mySkillDef);

            // Now we add our skill to one of the survivor's skill families
            // You can change component.primary to component.secondary, component.utility and component.special
            SkillLocator skillLocator = huntressBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            // If this is an alternate skill, use this code.
            // Here, we add our skill as a variant to the existing Skill Family.

            Skills.AddPrimarySkills(huntressBodyPrefab, mySkillDef);

        }
    }
}
