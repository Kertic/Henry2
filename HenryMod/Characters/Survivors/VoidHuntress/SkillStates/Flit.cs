using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Henry2Mod.Survivors.VoidHuntress.SkillStates
{
    public class Flit : BaseSkillState
    {
        public static float duration = 0.5f;
        public static float minJumpCancelRatio = 0.5f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 5f;

        public static string dodgeSoundString = "Play_huntress_shift_mini_blink";
        public static string cancelSoundString = "Play_huntress_shift_end";

        private Vector3 blinkDirection;
        private Vector3 blinkVector;
        private Transform modelTransform;
        private float minCancelTime;

        public override void OnEnter()
        {
            base.OnEnter();
            minCancelTime = duration * minJumpCancelRatio;

            if (isAuthority && inputBank && characterDirection)
            {
                blinkVector = GetBlinkVector();
            }

            if (NetworkServer.active)
            {
                characterBody.AddBuff(VoidHuntressBuffs.quickShot);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f * duration);
            }

            Util.PlaySound(dodgeSoundString, gameObject);

            CreateBlinkEffect(Util.GetCorePosition(gameObject));
        }

        protected virtual Vector3 GetBlinkVector()
        {
            blinkDirection = inputBank.aimDirection;

            return blinkDirection;
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(EntityStates.Huntress.BlinkState.blinkPrefab, effectData, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * initialSpeedCoefficient * Time.fixedDeltaTime);

            if (isAuthority)
            {
                if (fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                    return;
                }

                if (inputBank.jump.down && fixedAge >= minCancelTime)
                {
                    outer.SetNextStateToMain();
                    return;
                }

            }

        }


        public override void OnExit()
        {
            modelTransform = GetModelTransform();
            if (this.modelTransform)
            {
                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1.6f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidBlinkBodyOverlay.mat").WaitForCompletion();
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
            base.OnExit();

            characterMotor.disableAirControlUntilCollision = false;

            if (inputBank.jump.down)
            {
                blinkVector = GetBlinkVector();
                characterMotor.velocity = blinkVector * moveSpeedStat * finalSpeedCoefficient;
                Util.PlaySound(cancelSoundString, gameObject);
            }
            else
            {
                Util.PlaySound(dodgeSoundString, gameObject);
            }

        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(blinkDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            blinkDirection = reader.ReadVector3();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}