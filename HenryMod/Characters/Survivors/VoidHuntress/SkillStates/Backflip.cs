using EntityStates;
using EntityStates.Huntress;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Henry2Mod.Survivors.VoidHuntress.SkillStates
{
    public class Backflip : BaseSkillState
    {
        public static float duration = 0.5f;
        public static float initialSpeedCoefficient = 10f;
        public static float finalSpeedCoefficient = 2.5f;
        public static float jumpForce = 10.5f;
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;

        public static SkillDef skillDef;

        public static string dodgeSoundString = "HenryRoll";

        private float rollSpeed;
        private float stopwatch;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();

            if (isAuthority && inputBank)
            {
                forwardDirection = -Vector3.ProjectOnPlane(inputBank.aimDirection, Vector3.up).normalized;
                characterDirection.moveVector = -forwardDirection;

                Log.Warning("[InsideBackflip]");
            }

            characterMotor.disableAirControlUntilCollision = true;

            PlayAnimation("FullBody, Override", "Backflip", "Backflip.playbackRate", duration);

            Util.PlaySound(dodgeSoundString, gameObject);

            if (NetworkServer.active)
            {
                characterBody.AddBuff(VoidHuntressBuffs.quickShot);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * duration);
            }
        }

        private void RecalculateRollSpeed()
        {
            rollSpeed = moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, fixedAge / duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            RecalculateRollSpeed();
            
            if (characterMotor && characterDirection)
            {
                Vector3 velocity = characterMotor.velocity;
                Vector3 velocity2 = forwardDirection * rollSpeed;
                characterMotor.velocity = velocity2;
                characterMotor.velocity.y = velocity.y;
                characterMotor.moveDirection = forwardDirection;
                Log.Warning("[Vectors] : " + forwardDirection);
                Log.Warning("[forwardDirection]" + forwardDirection);
                Log.Warning("[characterMotor.velocity]" + characterMotor.velocity);
                Log.Warning("[characterMotor.moveDirection]" + characterMotor.moveDirection);
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            characterMotor.disableAirControlUntilCollision = false;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}