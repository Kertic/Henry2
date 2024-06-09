using EntityStates;
using EntityStates.Huntress;
using RoR2;
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

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = global::EntityStates.Commando.DodgeState.dodgeFOV;

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
                forwardDirection = -Vector3.ProjectOnPlane(inputBank.aimDirection, Vector3.up);
                characterDirection.moveVector = -forwardDirection;
                characterDirection.moveVector.y = jumpForce;
            }

            PlayAnimation("FullBody, Override", "Backflip", "Backflip.playbackRate", duration);

            Util.PlaySound(dodgeSoundString, gameObject);

            if (NetworkServer.active)
            {
                characterBody.AddBuff(VoidHuntressBuffs.lunarInsight);
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
            stopwatch += Time.fixedDeltaTime;
            if (cameraTargetParams)
            {
                cameraTargetParams.fovOverride = Mathf.Lerp(dodgeFOV, 60f, stopwatch / duration);
            }

            if (characterMotor && characterDirection)
            {
                Vector3 velocity = characterMotor.velocity;
                Vector3 velocity2 = forwardDirection * rollSpeed;
                characterMotor.velocity = velocity2;
                characterMotor.velocity.y = velocity.y;
                characterMotor.moveDirection = forwardDirection;
            }
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (cameraTargetParams) cameraTargetParams.fovOverride = -1f;
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