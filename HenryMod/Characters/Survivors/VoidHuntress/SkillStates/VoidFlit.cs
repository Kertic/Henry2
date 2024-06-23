using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Henry2Mod.Survivors.VoidHuntress.SkillStates
{
    public class VoidFlit : BaseSkillState
    {
        public static float totalDuration = 0.35f;
        public static float minJumpCancelRatio = 0.7f;
        public static float minSprintCancelRatio = 0.1f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 7.5f;

        public static string dodgeSoundString = "Play_huntress_shift_mini_blink";
        public static string cancelSoundString = "Play_huntress_shift_end";


        private Vector3 blinkDirection;
        private Vector3 blinkVector;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private float minJumpCancelTime;
        private float minSprintCancelTime;
        private bool hasFinishedBlinking;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFinishedBlinking = false;
            minJumpCancelTime = totalDuration * minJumpCancelRatio;
            minSprintCancelTime = totalDuration * minSprintCancelRatio;

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

            Util.PlaySound(dodgeSoundString, gameObject);

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
                if (fixedAge >= minSprintCancelTime && inputBank.sprint.down)
                {
                    ExitBlink();// After we can crouch cancel, we don't display exit blink unless we're exiting early
                    outer.SetNextStateToMain();
                    return;
                }

                if (fixedAge >= minJumpCancelTime)
                {
                    ExitBlink();// After we can jump cancel, we'll just briefly hover before falling down. So display the exit blink
                    if (inputBank.jump.down)
                    {
                        outer.SetNextStateToMain();
                        return;
                    }
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

            characterMotor.disableAirControlUntilCollision = false;
            characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, Mathf.Min(VoidHuntressBuffs.voidShotMaxStacks, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) + 1)); 

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

            Util.PlaySound(dodgeSoundString, gameObject);
            CreateBlinkEffect(Util.GetCorePosition(gameObject));
            hasFinishedBlinking = true;
        }
        protected virtual Vector3 GetBlinkVector()
        {
            blinkDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;

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