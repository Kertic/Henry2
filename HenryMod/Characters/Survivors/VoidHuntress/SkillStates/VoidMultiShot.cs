using EntityStates;
using EntityStates.LunarWisp;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using UnityEngine;
using static RoR2.BulletAttack;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidMultiShot : BaseSkillState
    {
        public const string skillName = "VoidMultiShot";
        public static float damageCoefficient = VoidHuntressStatics.voidMultishotDmgCoeff;
        public static float procCoefficient = 0.3f;
        public static float baseDuration = 0.6f;
        public static float firePercentTime = 0.0f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = FireLunarGuns.bulletTracerEffectPrefab;

        private float totalDuration;
        private float beginFireTime;
        private int totalShots;
        private int totalShotsFired;
        private float timePerShot;
        private string muzzleString;
        private VoidHuntressVoidState m_voidState;
        private HuntressTracker m_tracker;
        private HurtBox initialTarget;

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
            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();

            m_tracker = GetComponent<VoidHuntressTracker>();
            initialTarget = m_tracker.GetTrackingTarget();

        }

        public override void OnExit()
        {
            characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, Mathf.Min(VoidHuntressBuffs.voidShotMaxStacks, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) + 1));
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
                Util.PlaySound("Play_huntress_R_snipe_shoot", gameObject);

                if (isAuthority)
                {
                    Ray aimRay = GetAimRay();

                    if (initialTarget != null)
                    {
                        aimRay = new Ray(gameObject.transform.position, initialTarget.transform.position - gameObject.transform.position);
                    }

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
                        hitCallback = VoidSnipeHitCallback
                    }.Fire();


                    if (totalShotsFired == totalShots)
                    {
                        AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);
                    }
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
                    m_voidState?.AddVoidMeter(VoidHuntressStatics.voidBowVoidMeterGain);
                }

            }

            return defaultHitCallback.Invoke(bulletAttackRef, ref hitInfo);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}