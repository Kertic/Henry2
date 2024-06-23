using EntityStates;
using Henry2Mod.Characters.Survivors.VoidHuntress.Components;
using Henry2Mod.Survivors.VoidHuntress;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{

    public class EnterVoidFormSkillDef : SkillDef
    {
        public class InstanceData : BaseSkillInstanceData
        {
            public VoidHuntressVoidState voidState;
        }
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData { voidState = skillSlot.characterBody.GetComponent<VoidHuntressVoidState>() };
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return base.CanExecute(skillSlot) && ((InstanceData)skillSlot.skillInstanceData).voidState.CanTransform();
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && ((InstanceData)skillSlot.skillInstanceData).voidState.CanTransform();
        }

        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            base.OnExecute(skillSlot);
            ((InstanceData)skillSlot.skillInstanceData).voidState.TransitionToVoidState();
        }
    }
    public class EnterVoidForm : BaseSkillState
    {
        public static float baseDuration = 3f;
        public static float vanishPoint = 0.3f;
        public static float reappearPoint = 0.7f;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private TemporaryOverlay continuallyOverlay;
        private float vanishTime;
        private float reappearTime;

        private bool hasVanished;
        private bool hasReappeared;

        public override void OnEnter()
        {
            base.OnEnter();
            hasVanished = hasReappeared = false;

            vanishTime = baseDuration * vanishPoint;
            reappearTime = baseDuration * reappearPoint;
            characterMotor.disableAirControlUntilCollision = true;
            modelTransform = GetModelTransform();

            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                PhaseOut();
            }

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, baseDuration);
                characterBody.AddTimedBuff(RoR2Content.Buffs.TonicBuff, baseDuration);
            }

        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterMotor.velocity = Vector3.zero;

            if (fixedAge >= vanishTime && !hasVanished)
            {
                characterModel.invisibilityCount++;

                hasVanished = true;
            }

            if (fixedAge >= reappearTime && !hasReappeared)
            {
                characterModel.invisibilityCount--;
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

        private void ToggleActiveAura(bool isActive)
        {
            continuallyOverlay.RemoveFromCharacterModel();
            if (isActive)
            {
                continuallyOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                continuallyOverlay.duration = 600f;
                continuallyOverlay.animateShaderAlpha = true;
                continuallyOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
                continuallyOverlay.destroyComponentOnEnd = true;
                continuallyOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidBlinkBodyOverlayCorrupted.mat").WaitForCompletion();
                continuallyOverlay.AddToCharacerModel(characterModel);
            }
        }
    }
}
