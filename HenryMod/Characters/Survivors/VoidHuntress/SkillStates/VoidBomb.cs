using EntityStates;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.Henry;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidBomb : GenericProjectileBaseState
    {
        public const string skillName = "VoidBomb";


        public static float BaseDuration = 0.1f;
        //delays for projectiles feel absolute ass so only do this if you know what you're doing, otherwise it's best to keep it at 0
        public static float strikePointPercent = 0f;

        public static float DamageCoefficient = 16f;


        private VoidHuntressVoidState m_voidState;

        public override void OnEnter()
        {

            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            projectilePrefab = Henry2Assets.voidBombProjectilePrefab;
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"
            attackSoundString = "HenryBombThrow";
            baseDuration = BaseDuration;
            baseDelayBeforeFiringProjectile = baseDuration * strikePointPercent;
            damageCoefficient = DamageCoefficient;

            //base.projectilePitchBonus = 0;
            //base.minSpread = 0;
            //base.maxSpread = 0;
            force = 120f;
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
                PlayAnimation("Gesture, Override", "ThrowBomb", "ThrowBomb.playbackRate", duration);
            }

            base.FireProjectile();

            m_voidState?.AddVoidMeter(VoidHuntressStatics.bombVoidMeterGain);
        }

    }
}