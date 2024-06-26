using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.LunarWisp;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using RoR2.Orbs;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.BulletAttack;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidBow : BaseSkillState
    {
public const string skillName = "VoidBow";


        public static float damageCoefficient = VoidHuntressStatics.voidBowDmgCoeff;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.5f;
        public static float firePercentTime = 0f;
        public static float baseArrowReloadTimer = 0.1f;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;

        private static string muzzlePrefabString = "RoR2/Base/Huntress/MuzzleflashHuntress.prefab";
        private static GameObject muzzlePrefabOject = Addressables.LoadAssetAsync<GameObject>(muzzlePrefabString).WaitForCompletion();

        private float totalDuration;
        private float beginFireTime;
        private float arrowReloadTimer;
        private float lastArrowReloadTimer;
        private string muzzleString;
        private VoidHuntressVoidState m_voidState;
        private HuntressTracker m_tracker;
        private HurtBox initialOrbTarget;
        private int firedArrowCount;
        private int maxArrowCount;

        public override void OnEnter()
        {
            base.OnEnter();
            muzzleString = "Muzzle";

            arrowReloadTimer = baseArrowReloadTimer / attackSpeedStat;
            lastArrowReloadTimer = 0;

            maxArrowCount = characterBody.HasBuff(VoidHuntressBuffs.voidShot) ? 3 : 1;
            firedArrowCount = 0;

            totalDuration = baseDuration / attackSpeedStat;
            beginFireTime = firePercentTime * totalDuration;
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            characterBody.SetAimTimer(2f);
            m_tracker = GetComponent<VoidHuntressTracker>();

            if (m_tracker && isAuthority)
            {
                initialOrbTarget = m_tracker.GetTrackingTarget();
            }

            if (initialOrbTarget == null)
            {
                outer.SetNextStateToMain();
                return;
            }

            if (arrowReloadTimer * 3.0f >= totalDuration)
            {
                Log.Error($"[VoidBow Misconfiguration] Your reload time ({arrowReloadTimer}) is too long and can't be completed in the duration ({totalDuration}).");
            }

            PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", totalDuration, totalDuration * 0.2f / attackSpeedStat);
            PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", totalDuration, totalDuration * 0.2f / attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            lastArrowReloadTimer -= Time.fixedDeltaTime;

            if ((fixedAge >= beginFireTime))
            {
                FireOrbArrow();
            }

            if (isAuthority && inputBank && fixedAge >= totalDuration)
            {
                outer.SetNextStateToMain();
                return;
            }

        }

        private void ConsumeBuff()
        {
            if (characterBody.HasBuff(VoidHuntressBuffs.voidShot))
            {
                characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) - 1);
                foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
                {
                    characterBody.skillLocator.GetSkill(item)?.RunRecharge(VoidHuntressStatics.voidBowAttackCDRInSeconds);
                }

            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        private void FireOrbArrow()
        {
            if (firedArrowCount >= maxArrowCount || lastArrowReloadTimer > 0f || !NetworkServer.active)
            {
                return;
            }

            firedArrowCount++;
            lastArrowReloadTimer = arrowReloadTimer;
            GenericDamageOrb genericDamageOrb = CreateArrowOrb();
            genericDamageOrb.damageValue = characterBody.damage * damageCoefficient;
            genericDamageOrb.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            genericDamageOrb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            genericDamageOrb.attacker = gameObject;
            genericDamageOrb.procCoefficient = procCoefficient;
            HurtBox hurtBox = initialOrbTarget;

            if (hurtBox)
            {
                m_voidState?.AddVoidMeter(VoidHuntressStatics.voidBowVoidMeterGain);
                ConsumeBuff();
                EffectManager.SimpleMuzzleFlash(muzzlePrefabOject, gameObject, muzzleString, true);
                genericDamageOrb.origin = transform.position;
                genericDamageOrb.target = hurtBox;
                OrbManager.instance.AddOrb(genericDamageOrb);
            }

        }

        protected virtual GenericDamageOrb CreateArrowOrb()
        {
            return new HuntressArrowOrb();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
