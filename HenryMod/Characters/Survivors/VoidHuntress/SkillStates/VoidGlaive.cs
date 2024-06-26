using EntityStates;
using EntityStates.Huntress.HuntressWeapon;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidGlaive : BaseState
    {
        public const string skillName = "VoidGlaive";
        public static float baseDuration = 1f;
        public static float smallHopStrength;
        public static float antigravityStrength;
        public static float damageCoefficient = VoidHuntressStatics.voidGlaiveDmgCoeff;
        public static float damageCoefficientPerBounce = 1f;
        public static float glaiveProcCoefficient = 1.0f;
        public static float glaiveTravelSpeed = 80.0f;
        public static float glaiveBounceRange = 20f;
        public static string attackSoundString;
        public static int maxBounceCount = 4;
        public static int glaiveThrownCount = VoidHuntressStatics.voidGlaiveThrownCount;

        private float duration;
        private float stopwatch;
        private Animator animator;
        private GameObject chargeEffect;
        private Transform modelTransform;
        private HuntressTracker huntressTracker;
        private ChildLocator childLocator;
        private bool hasTriedToThrowGlaive;
        private bool hasSuccessfullyThrownGlaive;
        private HurtBox initialOrbTarget;
        private VoidHuntressVoidState m_voidState;


        public override void OnEnter()
        {
            base.OnEnter();
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            this.stopwatch = 0f;
            this.duration = baseDuration / this.attackSpeedStat;
            this.modelTransform = base.GetModelTransform();
            this.animator = base.GetModelAnimator();
            this.huntressTracker = base.GetComponent<HuntressTracker>();
            Util.PlayAttackSpeedSound(attackSoundString, base.gameObject, this.attackSpeedStat);
            if (this.huntressTracker && base.isAuthority)
            {
                this.initialOrbTarget = this.huntressTracker.GetTrackingTarget();
            }
            if (base.characterMotor && ThrowGlaive.smallHopStrength != 0f)
            {
                base.characterMotor.velocity.y = ThrowGlaive.smallHopStrength;
            }
            base.PlayAnimation("FullBody, Override", "ThrowGlaive", "ThrowGlaive.playbackRate", this.duration);
            if (this.modelTransform)
            {
                this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    Transform transform = this.childLocator.FindChild("HandR");
                    if (transform && ThrowGlaive.chargePrefab)
                    {
                        this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ThrowGlaive.chargePrefab, transform.position, transform.rotation);
                        this.chargeEffect.transform.parent = transform;
                    }
                }
            }
            if (base.characterBody)
            {
                characterBody.SetAimTimer(this.duration);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.chargeEffect)
            {
                EntityState.Destroy(this.chargeEffect);
            }
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 1.5f);
                this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }
            if (!this.hasTriedToThrowGlaive)
            {
                this.FireOrbGlaive();
            }
            if (!this.hasSuccessfullyThrownGlaive && NetworkServer.active)
            {
                base.skillLocator.secondary.AddOneStock();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (!this.hasTriedToThrowGlaive && this.animator.GetFloat("ThrowGlaive.fire") > 0f)
            {
                if (this.chargeEffect)
                {
                    EntityState.Destroy(this.chargeEffect);
                }
                this.FireOrbGlaive();
            }
            CharacterMotor characterMotor = base.characterMotor;
            characterMotor.velocity.y = characterMotor.velocity.y + ThrowGlaive.antigravityStrength * Time.fixedDeltaTime * (1f - this.stopwatch / this.duration);
            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void FireOrbGlaive()
        {
            if (!NetworkServer.active || this.hasTriedToThrowGlaive)
            {
                return;
            }

            hasTriedToThrowGlaive = true;

            for (int i = 0; i < glaiveThrownCount; i++)
            {
                LightningOrb lightningOrb = new LightningOrb();
                lightningOrb.lightningType = LightningOrb.LightningType.HuntressGlaive;
                lightningOrb.damageValue = characterBody.damage * damageCoefficient;
                lightningOrb.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
                lightningOrb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
                lightningOrb.attacker = gameObject;
                lightningOrb.procCoefficient = glaiveProcCoefficient;
                lightningOrb.bouncesRemaining = maxBounceCount;
                lightningOrb.speed = glaiveTravelSpeed * (i + 1);
                lightningOrb.bouncedObjects = new List<HealthComponent>();
                lightningOrb.range = glaiveBounceRange;
                lightningOrb.damageCoefficientPerBounce = damageCoefficientPerBounce;
                HurtBox hurtBox = initialOrbTarget;

                if (hurtBox)
                {
                    m_voidState?.AddVoidMeter(VoidHuntressStatics.voidGlaiveVoidMeterGain);
                    GrantBuff();
                    hasSuccessfullyThrownGlaive = true;
                    Transform transform = childLocator.FindChild("HandR");
                    EffectManager.SimpleMuzzleFlash(ThrowGlaive.muzzleFlashPrefab, gameObject, "HandR", true);
                    lightningOrb.origin = transform.position;
                    lightningOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(lightningOrb);
                }

            }

        }
        private void GrantBuff()
        {
            characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, Mathf.Min(VoidHuntressBuffs.voidShotMaxStacks, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) + 1));
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(this.initialOrbTarget));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }

    }
}
