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
    public class LunarSnipe : BaseSkillState
    {
        public static float damageCoefficient = VoidHuntressStaticValues.primaryBowDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.72f;
        public static float firePercentTime = 1.0f;
        public static float quickShotFirePercentTime = 0.25f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static float maxChargeFOV = 120f;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;

        private float totalDuration;
        private float beginFireTime;
        private bool hasRang;
        private string muzzleString;
        private VoidHuntressVoidState m_voidState;

        public override void OnEnter()
        {
            base.OnEnter();
            totalDuration = baseDuration / attackSpeedStat;
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();

            if (characterBody.HasBuff(VoidHuntressBuffs.quickShot))
            {
                beginFireTime = quickShotFirePercentTime * totalDuration;
            }
            else
            {
                beginFireTime = firePercentTime * totalDuration;
                characterMotor.walkSpeedPenaltyCoefficient = VoidHuntressStaticValues.primaryBowMovementSlowPenalty;
            }

            hasRang = false;
            characterBody.SetAimTimer(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            PlayAnimation("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", 0.1f);
            PlayAnimation("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", 0.1f);

            float currentChargeRatio = Math.Min(fixedAge / beginFireTime, 1.0f);

            if (fixedAge >= beginFireTime)
            {
                characterBody.SetSpreadBloom(0.0f, false);
                RingBell();
            }
            else
            {
                var lerpResult = Mathf.Lerp(0.7f, 1.0f, 1 - (currentChargeRatio));
                characterBody.SetSpreadBloom(lerpResult, false);
            }


            if (isAuthority)
            {
                if (inputBank)
                {
                    if (!inputBank.skill1.down)
                    {
                        outer.SetNextStateToMain();
                        Util.PlaySound("Play_huntress_m1_shoot", gameObject);
                        return;
                    }

                    if (characterBody.HasBuff(VoidHuntressBuffs.quickShot))
                    {
                        if (fixedAge >= beginFireTime)
                        {
                            outer.SetNextStateToMain();
                            Util.PlaySound("Play_huntress_m1_shoot", gameObject);
                            Util.PlaySound("Play_huntress_m1_shoot", gameObject);
                            return;
                        }
                    }
                }
            }

        }
        public override void OnExit()
        {
            characterMotor.walkSpeedPenaltyCoefficient = 1f;

            if ((fixedAge >= beginFireTime))
            {
                Fire();
            }

            base.OnExit();
        }

        private void RingBell()
        {
            if (!hasRang)
            {
                hasRang = true;
                Util.PlaySound("Play_UI_cooldownRefresh", base.gameObject);
            }
        }

        private void Fire()
        {
            Ray aimRay = GetAimRay();
            PlayAnimation("Gesture, Override", "FireSeekingArrow");
            PlayAnimation("Gesture, Additive", "FireSeekingArrow");
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
                tracerEffectPrefab = tracerEffectPrefab,
                spreadPitchScale = 0f,
                spreadYawScale = 0f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = FirePistol2.hitEffectPrefab,
                hitCallback = VoidSnipeHitCallback,
            };

            bulletAtk.Fire();

            if (characterBody.HasBuff(VoidHuntressBuffs.quickShot))
            {
                characterBody.SetBuffCount(VoidHuntressBuffs.quickShot.buffIndex, characterBody.GetBuffCount(VoidHuntressBuffs.quickShot) - 1);
            }
        }

        private bool VoidSnipeHitCallback(BulletAttack bulletAttackRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && hitInfo.hitHurtBox != null && bulletAttackRef.owner != null)
            {
                MonoBehaviour.print("[VoidSnipe HitCallback]");
                MonoBehaviour.print(hitInfo.hitHurtBox);
                CharacterBody attackerBody = bulletAttackRef.owner.GetComponent<CharacterBody>();

                if (attackerBody != null)
                {
                    m_voidState?.AddVoidMeter(VoidHuntressStaticValues.primaryBowVoidMeterGain);
                    attackerBody.skillLocator.secondary.RunRecharge(VoidHuntressStaticValues.primaryAttackCDRInSeconds);
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
