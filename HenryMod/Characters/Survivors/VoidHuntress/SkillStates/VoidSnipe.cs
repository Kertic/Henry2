﻿using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.LunarWisp;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidSnipe : BaseSkillState
    {

        public static float damageCoefficient = VoidHuntressStaticValues.primaryBowDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.72f;
        public static float firePercentTime = 1.0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        //        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("RoR2/Base/Huntress/TracerHuntressSnipe");
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;
        private float totalDuration;
        private float beginFireTime;
        private bool hasRang;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            totalDuration = baseDuration / attackSpeedStat;
            beginFireTime = firePercentTime * totalDuration;
            hasRang = false;
            characterBody.SetAimTimer(2f);
            Util.PlayAttackSpeedSound("Play_huntress_m1_ready", base.gameObject, base.attackSpeedStat);
            base.characterMotor.walkSpeedPenaltyCoefficient = VoidHuntressStaticValues.primaryBowMovementSlowPenalty;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            PlayAnimation("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", 0.1f);
            float currentChargeRatio = Math.Min( fixedAge / beginFireTime, 1.0f);
            Log.Warning("[VoidShot Charge Update]");
            Log.Message(fixedAge);
            Log.Message(beginFireTime);
            Log.Message(currentChargeRatio);
            characterBody.SetSpreadBloom(1 - (currentChargeRatio), false);
            if (fixedAge >= beginFireTime)
            {
                RingBell();
            }


            if ((!base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
            {
                if (fixedAge < beginFireTime)
                {
                    Util.PlayAttackSpeedSound("Play_huntress_m1_unready", base.gameObject, base.attackSpeedStat);
                }

                outer.SetNextStateToMain();
                return;
            }

        }
        public override void OnExit()
        {
            characterMotor.walkSpeedPenaltyCoefficient = 1f;

            if ((fixedAge >= beginFireTime) || characterBody.HasBuff(VoidHuntressBuffs.quickShot))
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
                Util.PlaySound("Play_item_goldgat_fire", base.gameObject);
            }
        }

        private void Fire()
        {
            Ray aimRay = GetAimRay();
            Util.PlaySound("Play_huntress_m2_impact", gameObject);
            PlayAnimation("Gesture, Override", "FireSeekingArrow");
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
            
            if(characterBody.HasBuff(VoidHuntressBuffs.quickShot))
            {
                characterBody.RemoveBuff(VoidHuntressBuffs.quickShot);
                    foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
                    {
                        characterBody.skillLocator.GetSkill(item)?.RunRecharge(VoidHuntressStaticValues.primaryAttackCDRInSeconds);
                    }

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
                    Log.Info("[Meter gain]: " + VoidHuntressStaticValues.primaryBowLunarInsightStacks);
                }

            }

            return defaultHitCallback.Invoke(bulletAttackRef, ref hitInfo);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }
}
