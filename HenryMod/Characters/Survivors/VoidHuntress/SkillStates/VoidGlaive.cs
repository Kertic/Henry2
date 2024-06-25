using EntityStates;
using EntityStates.Huntress.HuntressWeapon;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidGlaive : BaseState
    {

        public static float baseDuration = 2f;
        public static float damageCoefficient = 1.2f;
        public static float force = 20f;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            Transform modelTransform = GetModelTransform();
            this.duration = FireGlaive.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture", "FireGlaive", "FireGlaive.playbackRate", this.duration);
            Vector3 position = aimRay.origin;
            Quaternion rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            if (modelTransform)
            {
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild("RightHand");
                    if (transform)
                    {
                        position = transform.position;
                    }
                }
            }
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(FireGlaive.projectilePrefab, position, rotation, base.gameObject, this.damageStat * FireGlaive.damageCoefficient, FireGlaive.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
