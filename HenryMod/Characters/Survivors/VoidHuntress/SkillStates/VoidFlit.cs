using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Henry2Mod.Survivors.VoidHuntress.SkillStates
{
    public class VoidFlit : BaseSkillState
    {
        public const string skillName = "VoidFlit";

        public static float totalDuration = 0.35f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 7.5f;

        public static string dodgeSoundString = "Play_huntress_shift_mini_blink";
        public static string cancelSoundString = "Play_huntress_shift_end";

        private Vector3 blinkDirection;
        private Vector3 blinkVector;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private bool hasFinishedBlinking;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFinishedBlinking = false;

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

            characterMotor.velocity = Vector3.zero;
            if (!hasFinishedBlinking)
            {
                characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * initialSpeedCoefficient * Time.fixedDeltaTime);
            }

            if (isAuthority)
            {

                if (fixedAge >= totalDuration)
                {
                    ExitBlink();
                    outer.SetNextStateToMain();
                    return;
                }


            }

        }
        public override void OnExit()
        {
            base.OnExit();
            ExitBlink();

            characterMotor.disableAirControlUntilCollision = false;
            characterBody.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, Mathf.Min(VoidHuntressBuffs.voidShotMaxStacks, characterBody.GetBuffCount(VoidHuntressBuffs.voidShot) + 1));
            Util.PlaySound(cancelSoundString, gameObject);
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

            if (inputBank.jump.down)
            {
                blinkDirection = Vector3.up;
            }
            else if (inputBank.moveVector != Vector3.zero)
            {
                blinkDirection = (inputBank.moveVector).normalized;
            }
            else
            {
                blinkDirection = (characterDirection.forward).normalized;
            }

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