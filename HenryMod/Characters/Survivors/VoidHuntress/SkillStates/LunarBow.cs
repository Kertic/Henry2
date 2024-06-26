using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.LunarWisp;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using Henry2Mod.Survivors.VoidHuntress.SkillStates;
using RoR2;
using System;
using UnityEngine;
using static RoR2.BulletAttack;
using static RoR2.CameraTargetParams;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class LunarBow : BaseSkillState
    {
        public const string skillName = "LunarBow";
        public static float baseDuration = 0.42f;
        public static float procCoefficient = 1f;
        public static float firePercentTime = 1.0f;
        public static float quickShotFirePercentTime = 0.25f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;

        public float damageCoefficient;
        private const float maxZoomFOV = 60f;

        private float totalDuration;
        private float beginFireTime;
        private bool hasRang;
        private string muzzleString;
        private VoidHuntressVoidState m_voidState;
        CameraParamsOverrideHandle handle;
        private HuntressTracker m_tracker;
        private HurtBox initialTarget;




        public override void OnEnter()
        {
            base.OnEnter();
            totalDuration = baseDuration / attackSpeedStat;
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            hasRang = false;

            m_tracker = GetComponent<VoidHuntressTracker>();
            initialTarget = m_tracker.GetTrackingTarget();

            if (characterBody.HasBuff(VoidHuntressBuffs.quickShot))
            {
                beginFireTime = quickShotFirePercentTime * totalDuration;
                hasRang = true;
                damageCoefficient = VoidHuntressStatics.lunarBowFlurryDmgCoeff;
            }
            else
            {
                beginFireTime = firePercentTime * totalDuration;
                characterMotor.walkSpeedPenaltyCoefficient = VoidHuntressStatics.lunarBowMovementSlowPenalty;
                damageCoefficient = VoidHuntressStatics.lunarBowDmgCoeff;

                CameraTargetParams ctp = base.cameraTargetParams;
                CharacterCameraParamsData characterCameraParamsData = ctp.currentCameraParamsData;
                characterCameraParamsData.idealLocalCameraPos = new Vector3(1.8f, -0.2f, -6f);
                characterCameraParamsData.isFirstPerson = false;
                characterCameraParamsData.fov = maxZoomFOV;
                UpdateCameraRequest(characterCameraParamsData, 0.0f, beginFireTime);
            }

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
                        return;
                    }

                    if (characterBody.HasBuff(VoidHuntressBuffs.quickShot))
                    {
                        if (fixedAge >= beginFireTime)
                        {
                            outer.SetNextStateToMain();
                            return;
                        }
                    }
                }
            }
        }

        public override void OnExit()
        {
            characterMotor.walkSpeedPenaltyCoefficient = 1f;
            cameraTargetParams.RemoveParamsOverride(handle, 0.4f);

            if ((fixedAge >= beginFireTime))
            {
                Fire();
            }

            base.OnExit();
        }

        private void UpdateCameraRequest(CharacterCameraParamsData data, float removeDuration = 0.2f, float addDuration = 0.2f)
        {
            return; // Remove zooms
            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = data,
                priority = 0,
            };

            cameraTargetParams.RemoveParamsOverride(handle, removeDuration);
            handle = cameraTargetParams.AddParamsOverride(request, addDuration);
        }

        private void RingBell()
        {
            if (!hasRang)
            {
                hasRang = true;
                Util.PlayAttackSpeedSound("Play_railgunner_m2_reload_pass", gameObject, attackSpeedStat);
            }
        }

        private void Fire()
        {
            Ray aimRay = GetAimRay();

            if (initialTarget != null)
            {
                aimRay = new Ray(gameObject.transform.position, initialTarget.transform.position - gameObject.transform.position);
            }


            Util.PlaySound("Play_huntress_m1_shoot", gameObject);
            var bulletAtk = new BulletAttack
            {
                bulletCount = 1,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = (characterBody.HasBuff(VoidHuntressBuffs.quickShot) ? VoidHuntressStatics.lunarBowFlurryDmgCoeff : damageCoefficient) * damageStat,
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
                CharacterBody attackerBody = bulletAttackRef.owner.GetComponent<CharacterBody>();

                if (attackerBody != null)
                {
                    m_voidState?.AddVoidMeter(VoidHuntressStatics.lunarBowVoidMeterGain);
                    attackerBody.skillLocator.utility.RunRecharge(VoidHuntressStatics.lunarBowAttackCDRInSeconds);
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
