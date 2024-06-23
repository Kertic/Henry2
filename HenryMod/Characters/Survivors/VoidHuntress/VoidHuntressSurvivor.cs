using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates;
using Henry2Mod.Characters.Survivors.VoidHuntress.UI;
using Henry2Mod.Modules;
using Henry2Mod.Modules.Characters;
using Henry2Mod.Survivors.Henry;
using Henry2Mod.Survivors.Henry.SkillStates;
using Henry2Mod.Survivors.VoidHuntress.SkillStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

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
        public override string modelPrefabName => "mdlHuntress";
        public override string displayPrefabName => "HuntressDisplay";

        public const string VOIDHUNTRESS_PREFIX = Henry2Plugin.DEVELOPER_PREFIX + "_VOIDHUNTRESS_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => VOIDHUNTRESS_PREFIX;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = VOIDHUNTRESS_PREFIX + "NAME",
            subtitleNameToken = VOIDHUNTRESS_PREFIX + "SUBTITLE",

            characterPortrait = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Huntress/texHuntressIcon.png").WaitForCompletion(),

            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Assets.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1,
        };

        public override UnlockableDef characterUnlockableDef => VoidHuntressUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => null;// new VoidHuntressItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }
        public VoidMeter voidHudMeterRef { get; protected set; }
        public VoidHuntressVoidState voidHuntressVoidStateRef { get; protected set; }
        public float voidAmount { get; protected set; }
        public float maxVoidAmount { get; protected set; }

        public enum SkillOverrideTypes
        {
            Primary,
            Secondary,
            Utility,
            Special,
            NUM_OF_TYPES
        }
        private SkillDef[] skillDefOverrides;
        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;
            skillDefOverrides = new SkillDef[(int)SkillOverrideTypes.NUM_OF_TYPES];

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
            VoidHuntressBuffs.Init();


            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        protected override void InitializeCharacterBodyPrefab()
        {
            GameObject defaultBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBody.prefab").WaitForCompletion();

            characterModelObject = defaultBodyPrefab.transform.Find("ModelBase").Find("mdlHuntress").gameObject;
            bodyPrefab = Prefabs.CreateBodyPrefab(characterModelObject, bodyInfo);
            prefabCharacterBody = bodyPrefab.GetComponent<CharacterBody>();

            Log.Warning("[InitCharBodyPrefab]");
            prefabCharacterModel = Prefabs.SetupCharacterModel(bodyPrefab, customRendererInfos);

        }

        protected override void InitializeDisplayPrefab()
        {
            displayPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressDisplay.prefab").WaitForCompletion();
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
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon3");

        }

        void AdditionalBodySetup()
        {
            // This would be where we add things like the void bow
            voidHuntressVoidStateRef = bodyPrefab.GetComponent<VoidHuntressVoidState>();

            voidHuntressVoidStateRef.Init(prefabCharacterBody, skillDefOverrides);
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
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/ThrowSmokebomb.asset").WaitForCompletion().icon,
            });

            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);
        }

        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //here is a basic skill def with all fields accounted for
            var m_primarySkill = Skills.CreateSkillDef(new SkillDefInfo
                (
                "LunarBow",
                VOIDHUNTRESS_PREFIX + "LUNAR_BOW_NAME",
                VOIDHUNTRESS_PREFIX + "LUNAR_BOW_DESCRIPTION",
                Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/AimArrowSnipe.asset").WaitForCompletion().icon,
                new EntityStates.SerializableEntityStateType(typeof(LunarSnipe)))
            );
            m_primarySkill.canceledFromSprinting = true;
            m_primarySkill.cancelSprintingOnActivation = true;

            var overridePrimarySkill = skillDefOverrides[(int)SkillOverrideTypes.Primary] = Skills.CreateSkillDef(new SkillDefInfo
            (
                "VoidBow",
                VOIDHUNTRESS_PREFIX + "VOID_BOW_NAME",
                VOIDHUNTRESS_PREFIX + "VOID_BOW_DESCRIPTION",
                Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/FireArrowSnipe.asset").WaitForCompletion().icon,
                new EntityStates.SerializableEntityStateType(typeof(VoidSnipe)))
            );
            m_primarySkill.canceledFromSprinting = true;
            m_primarySkill.cancelSprintingOnActivation = true;


            Skills.AddPrimarySkills(bodyPrefab, m_primarySkill);

        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            //here is a basic skill def with all fields accounted for
            var flitSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Flit",
                skillNameToken = VOIDHUNTRESS_PREFIX + "FLIT_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "FLIT_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyBlink.asset").WaitForCompletion().icon,

                activationState = new EntityStates.SerializableEntityStateType(typeof(Flit)),

                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 4f,
                baseMaxStock = 2,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            var voidFlitSkillDef = skillDefOverrides[(int)SkillOverrideTypes.Secondary] = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "VoidFlit",
                skillNameToken = VOIDHUNTRESS_PREFIX + "VOID_FLIT_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "VOID_FLIT_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyMiniBlink.asset").WaitForCompletion().icon,

                activationState = new EntityStates.SerializableEntityStateType(typeof(VoidFlit)),

                activationStateMachineName = "Weapon3",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 6f,
                baseMaxStock = 3,

                rechargeStock = 3,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });


            Skills.AddSecondarySkills(bodyPrefab, flitSkillDef);

        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            var stormArrow = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "ArrowFlurry",
                skillNameToken = VOIDHUNTRESS_PREFIX + "ARROW_FLURRY_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "ARROW_FLURRY_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFireFMJ.asset").WaitForCompletion().icon,

                activationState = new EntityStates.SerializableEntityStateType(typeof(StormArrow)),

                //setting this to the "Weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 7f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            var voidStormArrow = skillDefOverrides[(int)SkillOverrideTypes.Utility] = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "VoidArrowFlurry",
                skillNameToken = VOIDHUNTRESS_PREFIX + "VOID_ARROW_FLURRY_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "VOID_ARROW_FLURRY_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyFocusedAssault.asset").WaitForCompletion().icon,

                activationState = new EntityStates.SerializableEntityStateType(typeof(VoidMultiShot)),

                //setting this to the "Weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 1f,
                baseMaxStock = 8,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });




            Skills.AddUtilitySkills(bodyPrefab, stormArrow);

        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            //a basic skill. some fields are omitted and will just have default values
            SkillDef specialSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryBomb",
                skillNameToken = VOIDHUNTRESS_PREFIX + "SPECIAL_BOMB_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "SPECIAL_BOMB_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(VoidBomb)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 10f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                isCombatSkill = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,
            });

            EnterVoidFormSkillDef specialSkillDef2 = Skills.CreateSkillDef<EnterVoidFormSkillDef>(new SkillDefInfo
            {
                skillName = "Invert",
                skillNameToken = VOIDHUNTRESS_PREFIX + "INVERT_NAME",
                skillDescriptionToken = VOIDHUNTRESS_PREFIX + "INVERT_DESCRIPTION",
                skillIcon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Bandit2/ThrowSmokebomb.asset").WaitForCompletion().icon,

                activationState = new EntityStates.SerializableEntityStateType(typeof(EnterVoidForm)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                isCombatSkill = true,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,
            });

            Skills.AddSpecialSkills(bodyPrefab, specialSkillDef2);
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.GetComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();
            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Achievements/texMercClearGameMonsoonIcon.png").WaitForCompletion(),
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
            On.RoR2.UI.HUD.Awake += HUD_Awake;
            RoR2.UI.HUD.onHudTargetChangedGlobal += HUD_onHudTargetChangedGlobal;
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            CreateResourceGauge(self);
            orig(self);
        }

        private void CreateResourceGauge(RoR2.UI.HUD hud)
        {
            if (voidHudMeterRef) return;

            Log.Warning("[ResourceGaugeMake]");
            Log.Warning("[VoidStateRef]: " + voidHuntressVoidStateRef);
            var voidHudRoot = new GameObject("VoidHuntressHud");
            voidHudRoot.transform.SetParent(hud.mainContainer.transform);
            voidHudMeterRef = voidHudRoot.AddComponent<VoidMeter>();
            voidHudMeterRef.Init(voidHuntressVoidStateRef);

        }
        private void HUD_onHudTargetChangedGlobal(RoR2.UI.HUD obj)
        {
            if (obj && obj.targetBodyObject && voidHudMeterRef)
            {
                VoidHuntressVoidState voidState = obj.targetBodyObject.GetComponent<VoidHuntressVoidState>();
                voidState.Init(obj.targetBodyObject.GetComponent<CharacterBody>(), skillDefOverrides);
                if (voidState)
                {
                    voidHudMeterRef.Init(voidState);
                }
            }
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

            if (sender.HasBuff(VoidHuntressBuffs.voidSicknessBuff))
            {
                args.baseAttackSpeedAdd += 0.07f * sender.GetBuffCount(VoidHuntressBuffs.voidSicknessBuff);
            }
        }


    }
}
