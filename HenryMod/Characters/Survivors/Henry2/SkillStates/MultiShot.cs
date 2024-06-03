using EntityStates;
using RoR2;
using UnityEngine;

namespace Henry2Mod.Survivors.Henry.SkillStates
{
    public class MultiShot : BaseSkillState
    {
        public static float damageCoefficient = Henry2StaticValues.multiShotGunDamageCoefficient;
        public static float procCoefficient = 0.2f;
        public static float baseDuration = 0.6f;
        //delay on firing is usually ass-feeling. only set this if you know what you're doing
        public static float firePercentTime = 0.0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float totalDuration;
        private float beginFireTime;
        private int totalShots;
        private int totalShotsFired;
        private float timePerShot;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            totalShotsFired = 0;
            totalShots = activatorSkillSlot.stock;
            totalDuration = baseDuration / attackSpeedStat;
            beginFireTime = firePercentTime * totalDuration;
            timePerShot = totalDuration / totalShots;
            characterBody.SetAimTimer(2f);
            muzzleString = "Muzzle";

            PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= beginFireTime)
            {
                if (fixedAge >= (timePerShot * totalShotsFired))
                {
                    Fire();
                }
            }

            if (fixedAge >= totalDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void Fire()
        {
            if (totalShotsFired < totalShots)
            {
                totalShotsFired++;
                activatorSkillSlot.stock--;

                characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, gameObject, muzzleString, false);
                Util.PlaySound("HenryShootPistol", gameObject);

                if (isAuthority)
                {
                    Ray aimRay = GetAimRay();
                    new BulletAttack
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
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                    }.Fire();


                    if (totalShotsFired == totalShots)
                    {
                        AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}