using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.LunarWisp;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using static RoR2.BulletAttack;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidVolleySkillDef : SkillDef
    {
        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return base.CanExecute(skillSlot) && skillSlot.characterBody.inputBank.skill1.down;
        }
        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && skillSlot.characterBody.inputBank.skill1.down;
        }

    }
    public class VoidVolley : BaseSkillState
    {

        public const string skillName = "VoidVolley";
        public static float damageCoefficient = VoidHuntressStatics.lunarBowDmgCoeff;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.5f;
        public static float firePercentTime = 0.0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static SkillDef skillDef;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;
        public static GameObject tracerEffectPrefab2 = EntityStates.LaserTurbine.FireMainBeamState.forwardBeamTracerEffect;
        public static GameObject tracerEffectPrefab3 = EntityStates.Sniper.SniperWeapon.FireRifle.tracerEffectPrefab;


        private float totalDuration;
        private float beginFireTime;
        private string muzzleString;
        private bool hasFired;
        private VoidHuntressVoidState m_voidState;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            totalDuration = baseDuration / attackSpeedStat;
            beginFireTime = firePercentTime * totalDuration;
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();

            characterBody.SetAimTimer(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();


            if (base.isAuthority && fixedAge >= beginFireTime && !hasFired)
            {
                Util.PlayAttackSpeedSound("Play_huntress_m1_unready", base.gameObject, base.attackSpeedStat);
                Fire();
                hasFired = true;
            }

            if (fixedAge >= totalDuration && isAuthority)
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
            Ray aimRay = GetAimRay();
            Util.PlaySound("Play_huntress_m1_shoot", gameObject);
            var bulletAtk = new BulletAttack
            {
                bulletCount = 1,
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
                tracerEffectPrefab = tracerEffectPrefab2,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = FirePistol2.hitEffectPrefab,
                hitCallback = HitCallback,
                
            };

            bulletAtk.Fire();

        }

        private bool HitCallback(BulletAttack bulletAttackRef, ref BulletHit hitInfo)
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
