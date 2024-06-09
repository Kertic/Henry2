using EntityStates;
using Henry2Mod.Survivors.Henry;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Henry2Mod.Survivors.Henry.SkillStates
{
    public class ThrowBomb : GenericProjectileBaseState
    {
        public static float BaseDuration = 2f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float strikePointPercent = 1f;

        public static float DamageCoefficient = 16f;

        public override void OnEnter()
        {
            projectilePrefab = Henry2Assets.bombProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            attackSoundString = "HenryBombThrow";

            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = baseDuration * strikePointPercent;

            damageCoefficient = DamageCoefficient;
            //proc coefficient is set on the components of the projectile prefab
            force = 80f;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            recoilAmplitude = 0.1f;
            bloom = 10;

            if (baseDelayBeforeFiringProjectile > 0.1f)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.Slow80, baseDelayBeforeFiringProjectile);
            }

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void FireProjectile()
        {
            if (GetModelAnimator())
            {
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", this.duration);
            }

            base.FireProjectile();
        }

    }
}