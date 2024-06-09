using Henry2Mod.Modules;
using Henry2Mod.Modules.Characters;
using Henry2Mod.Survivors.Henry;
using Henry2Mod.Survivors.Henry.SkillStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public class VoidHuntressSurvivor : SurvivorBase<VoidHuntressSurvivor>
    {
        public override string assetBundleName => "myassetbundle2"; 
        // public override string assetBundleName => "voidhuntressassetbundle"; 

        public override string bodyName => "VoidHuntressBody"; 

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "VoidHuntressMonsterMaster"; 

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlVoidHuntress";
        public override string displayPrefabName => "VoidHuntressDisplay";

        public const string VOIDHUNTRESS_PREFIX = Henry2Plugin.DEVELOPER_PREFIX + "_VOIDHUNTRESS_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => VOIDHUNTRESS_PREFIX;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = VOIDHUNTRESS_PREFIX + "NAME",
            subtitleNameToken = VOIDHUNTRESS_PREFIX + "SUBTITLE",

            characterPortrait =  Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Huntress/texHuntressIcon.png").WaitForCompletion(),

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
        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();
        }

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
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //here is a basic skill def with all fields accounted for
            var m_primarySkill = Skills.CreateSkillDef(new SkillDefInfo
                (
                "HenryPrimaryGun",
                VOIDHUNTRESS_PREFIX + "PRIMARY_1_NAME",
                VOIDHUNTRESS_PREFIX + "PRIMARY_1_DESCRIPTION",
                assetBundle.LoadAsset<Sprite>("texPistolIcon"),
                new EntityStates.SerializableEntityStateType(typeof(HenryPrimaryShoot)))
            );

            Skills.AddPrimarySkills(bodyPrefab, m_primarySkill);

        }

        private void AddSecondarySkills()
        {
        }

        private void AddUtiitySkills()
        {
        }

        private void AddSpecialSkills()
        {
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin

            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);

            #endregion

            skinController.skins = skins.ToArray();
        }

        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            Henry2AI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);

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
