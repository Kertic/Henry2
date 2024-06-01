using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;
using static RoR2.BulletAttack;


namespace Henry2Mod.Survivors.Henry.SkillStates
{
    public class HenryPrimaryShoot : BaseSkillState
    {

        public static float damageCoefficient = Henry2StaticValues.primaryGunDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.6f;
        public static float firePercentTime = 0.0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float totalDuration;
        private float beginFireTime;
        private bool hasFired;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            totalDuration = baseDuration / attackSpeedStat;
            beginFireTime = firePercentTime * totalDuration;
            characterBody.SetAimTimer(2f);
            muzzleString = "Muzzle";

            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= beginFireTime)
            {
                Fire();
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
            if (hasFired) return;
            if (!isAuthority) return;

            hasFired = true;
            Ray aimRay = GetAimRay();
            AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
            Util.PlaySound(FireBarrage.fireBarrageSoundString, gameObject);
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
            };

            bulletAtk.Fire();
        }

        private bool HenryHitCallback(BulletAttack bulletAttackRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && bulletAttackRef.owner != null)
            {
                CharacterBody attackerBody = bulletAttackRef.owner.GetComponent<CharacterBody>();

                if (attackerBody != null)
                {
                    attackerBody.skillLocator.GetSkill(SkillSlot.Secondary).rechargeStopwatch += Henry2StaticValues.primaryAttackCDRInSeconds;
                    attackerBody.skillLocator.GetSkill(SkillSlot.Utility).rechargeStopwatch += Henry2StaticValues.primaryAttackCDRInSeconds;
                    attackerBody.skillLocator.GetSkill(SkillSlot.Special).rechargeStopwatch += Henry2StaticValues.primaryAttackCDRInSeconds;
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
