using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.LunarWisp;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using System;
using UnityEngine;
using static RoR2.BulletAttack;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidBow : BaseSkillState
    {
        public static float damageCoefficient = VoidHuntressStatics.lunarBowDmgCoeff;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.72f;
        public static float firePercentTime = 0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static float maxChargeFOV = 120f;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;

        private float totalDuration;
        private float beginFireTime;
        private bool hasFired;
        private string muzzleString;
        private VoidHuntressVoidState m_voidState;
        private CameraTargetParams.CameraParamsOverrideHandle handle;

        public override void OnEnter()
        {
            base.OnEnter();
            totalDuration = baseDuration / attackSpeedStat;
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            beginFireTime = firePercentTime * totalDuration;

            hasFired = false;
            characterBody.SetAimTimer(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            PlayAnimation("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", 0.1f);
            PlayAnimation("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", 0.1f);

            if ((fixedAge >= beginFireTime))
            {
                Fire();
            }

            if (isAuthority && inputBank && fixedAge >= totalDuration)
            {
                outer.SetNextStateToMain();
                return;
            }

        }
        public override void OnExit()
        {


            base.OnExit();
        }

        private void Fire()
        {
            if (hasFired) return;

            hasFired = true;
            Ray aimRay = GetAimRay();
            Util.PlaySound("Play_huntress_m2_impact", gameObject);
            PlayAnimation("Gesture, Override", "FireSeekingArrow");
            PlayAnimation("Gesture, Additive", "FireSeekingArrow");
            var bulletAtk = new BulletAttack
            {
                bulletCount = (uint)(characterBody.HasBuff(VoidHuntressBuffs.voidShot) ? 2 : 1),
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = damageCoefficient * damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Generic,
                falloffModel = BulletAttack.FalloffModel.None,
                maxDistance = range,
                force = force,
                hitMask = LayerIndex.CommonMasks.bullet,
                minSpread = 0f,
                maxSpread = 0f,
                isCrit = RollCrit(),
                owner = gameObject,
                muzzleName = muzzleString,
                smartCollision = true,
                procChainMask = default,
                procCoefficient = procCoefficient,
                radius = 0.75f,
                sniper = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                weapon = null,
                tracerEffectPrefab = tracerEffectPrefab,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = FirePistol2.hitEffectPrefab,
                hitCallback = VoidSnipeHitCallback,
            };

            bulletAtk.Fire();

            if (characterBody.HasBuff(VoidHuntressBuffs.voidShot))
            {
                characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) - 1);
                foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
                {
                    characterBody.skillLocator.GetSkill(item)?.RunRecharge(VoidHuntressStatics.voidPrimaryAttackCDRInSeconds);
                }

            }
        }

        private bool VoidSnipeHitCallback(BulletAttack bulletAttackRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && hitInfo.hitHurtBox != null && bulletAttackRef.owner != null)
            {
                CharacterBody attackerBody = bulletAttackRef.owner.GetComponent<CharacterBody>();

                if (attackerBody != null)
                {
                    m_voidState?.AddVoidMeter(VoidHuntressStatics.lunarBowVoidMeterGain);
                }
            }

            return defaultHitCallback.Invoke(bulletAttackRef, ref hitInfo);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
