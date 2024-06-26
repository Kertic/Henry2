using EntityStates;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class ConsumeVoidForm : BaseSkillState
    {
public const string skillName = "ConsumeVoidForm";



        public static float baseDuration = VoidHuntressStatics.transitionDuration;
        public static float vanishPoint = 0.3f;
        public static float reappearPoint = 0.7f;

        private VoidHuntressVoidState m_voidState;
        private Transform modelTransform;
        private CharacterModel characterModel;
        private float vanishTime;
        private float reappearTime;

        private bool hasVanished;
        private bool hasReappeared;

        public override void OnEnter()
        {
            base.OnEnter();


            m_voidState = characterBody.GetComponent<VoidHuntressVoidState>();
            hasVanished = hasReappeared = false;

            vanishTime = baseDuration * vanishPoint;
            reappearTime = baseDuration * reappearPoint;
            characterMotor.disableAirControlUntilCollision = true;
            modelTransform = GetModelTransform();

            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                PhaseOut();

                ProcChainMask procChainMask = default(ProcChainMask);
                healthComponent.Heal(VoidHuntressStatics.healthPerVoidMeter * m_voidState.currentVoidMeter, procChainMask);
                m_voidState.AddVoidMeter(-m_voidState.currentVoidMeter);

            }

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, baseDuration);
            }

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterMotor.velocity = Vector3.zero;

            if (fixedAge >= vanishTime && !hasVanished)
            {
                hasVanished = true;
            }

            if (fixedAge >= reappearTime && !hasReappeared)
            {
                PhaseOut(true);
                hasReappeared = true;
            }

            if (fixedAge >= baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            characterMotor.disableAirControlUntilCollision = false;
            m_voidState.TransitionToLunarState();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        private void PhaseOut(bool reversed = false)
        {
            Util.PlaySound(reversed ? "Play_voidman_transform_return" : "Play_voidman_transform", gameObject);
            Util.PlaySound("Play_voidman_R_activate", gameObject);
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(reversed ? Vector3.up : Vector3.down);
            effectData.origin = Util.GetCorePosition(gameObject);
            EffectManager.SpawnEffect(EntityStates.Huntress.BlinkState.blinkPrefab, effectData, false);

            TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.duration = reversed ? (baseDuration - reappearTime) : vanishTime;
            temporaryOverlay.animateShaderAlpha = true;
            temporaryOverlay.alphaCurve =
                reversed ?
                AnimationCurve.EaseInOut(0f, 1f, 1f, 0f) :
                AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            temporaryOverlay.destroyComponentOnEnd = true;
            temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidBlinkBodyOverlayCorrupted.mat").WaitForCompletion();
            temporaryOverlay.AddToCharacerModel(characterModel);
        }

    }
}
