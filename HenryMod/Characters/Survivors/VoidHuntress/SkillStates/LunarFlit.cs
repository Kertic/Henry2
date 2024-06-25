using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Henry2Mod.Survivors.VoidHuntress.SkillStates
{
    public class LunarFlit : BaseSkillState
    {
        public static float totalDuration = 1.5f;
        public static float minJumpCancelThresh = 0.6f;
        public static float minSprintCancelThresh = 0.1f;
        public static float abilityFalloffThresh = 0.1f;
        public static float initialSpeedCoefficient = 6f;
        public static float finalSpeedCoefficient = 6.8f;

        public static string dodgeSoundString = "Play_huntress_shift_mini_blink";
        public static string dodgeStartSoundString = "Play_voidman_m2_chargeUp";
        public static string cancelSoundString = "Play_huntress_shift_end";


        private Vector3 blinkDirection;
        private Vector3 blinkVector;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private float minJumpCancelTime;
        private float minManualCancelTime;
        private bool hasFinishedBlinking;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFinishedBlinking = false;
            minJumpCancelTime = totalDuration * minJumpCancelThresh;
            minManualCancelTime = totalDuration * minSprintCancelThresh;

            if (isAuthority && inputBank && characterDirection)
            {
                blinkVector = GetBlinkVector();
                modelTransform = GetModelTransform();
                if (modelTransform)
                {
                    characterModel = modelTransform.GetComponent<CharacterModel>();
                    characterModel.invisibilityCount++;
                }
            }

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, totalDuration);
            }

            Util.PlaySound(dodgeStartSoundString, gameObject);

            CreateBlinkEffect(Util.GetCorePosition(gameObject));
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();


            base.characterMotor.velocity = Vector3.zero;
            if (!hasFinishedBlinking)
            {
                base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * initialSpeedCoefficient * Time.fixedDeltaTime);
            }

            if (isAuthority)
            {
                if (fixedAge >= minManualCancelTime && inputBank.skill2.justPressed)
                {
                    ExitBlink();// After we can manually cancel, we can hover at any point
                }

                if (fixedAge >= minJumpCancelTime)
                {
                    ExitBlink();// After we can jump cancel, we'll just briefly hover before falling down. So display the exit blink
                }

                if (inputBank.jump.down && hasFinishedBlinking)
                {
                    outer.SetNextStateToMain();
                    return;
                }

                if (fixedAge >= totalDuration)
                {
                    ExitBlink();// Someday we just get yeeted back to the floor
                    outer.SetNextStateToMain();
                    return;
                }

            }

        }
        public override void OnExit()
        {
            base.OnExit();
            characterBody.SetAimTimer(totalDuration - minJumpCancelTime);
            ExitBlink();

            if (inputBank.jump.down)
            {
                blinkVector = GetBlinkVector();
                characterMotor.velocity = blinkVector * moveSpeedStat * finalSpeedCoefficient;
                Util.PlaySound(cancelSoundString, gameObject);
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


        private void ExitBlink()
        {
            if (hasFinishedBlinking) return;

            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel.invisibilityCount--;

                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.6f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidBlinkBodyOverlay.mat").WaitForCompletion();
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }

            characterMotor.disableAirControlUntilCollision = false;
            Util.PlaySound(dodgeSoundString, gameObject);
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            hasFinishedBlinking = true;
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



    }
}