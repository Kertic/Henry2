using Henry2Mod.Modules;
using Henry2Mod.Modules.Characters;
using Henry2Mod.Survivors.Henry;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public class VoidHuntressSurvivor : SurvivorBase<VoidHuntressSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "myassetbundle2"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "HenryBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "HenryMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlHenry";
        public override string displayPrefabName => "HenryDisplay";

        public const string VOIDHUNTRESS_PREFIX = Henry2Plugin.DEVELOPER_PREFIX + "_VOIDHUNTRESS_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => VOIDHUNTRESS_PREFIX;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = VOIDHUNTRESS_PREFIX + "NAME",
            subtitleNameToken = VOIDHUNTRESS_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Assets.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = assetBundle.LoadMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => VoidHuntressUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new VoidHuntressItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }
        public GameObject voidHudObject { get; protected set; }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            VoidHuntressUnlockables.Init();

            base.InitializeCharacter();

            VoidHuntressConfig.Init();
            VoidHuntressStates.Init();
            VoidHuntressTokens.Init();

            VoidHuntressAssets.Init(assetBundle);
            VoidHuntressBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }
        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
        }

        void AdditionalBodySetup()
        {
            // This would be where we add things like the void bow
        }

        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkill()
        {
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");

            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "VoidTouched",
                skillNameToken = VOIDHUNTRESS_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "PASSIVE_DESCRIPTION",
                keywordTokens = new string[] {},
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            });

        }

        private void AddPrimarySkills()
        {
            throw new NotImplementedException();
        }

        private void AddSecondarySkills()
        {
            throw new NotImplementedException();
        }

        private void AddUtiitySkills()
        {
            throw new NotImplementedException();
        }

        private void AddSpecialSkills()
        {
            throw new NotImplementedException();
        }

        public override void InitializeSkins()
        {
            throw new NotImplementedException();
        }

        public override void InitializeCharacterMaster()
        {
            throw new NotImplementedException();
        }

        void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            //This is where I'd add in conditional stat changes based on the state of my character
            /*
            if (sender.HasBuff(Henry2Buffs.armorBuff))
            {
                args.armorAdd += 300;
            }
            */
        }


    }
}
