using Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates;
using Henry2Mod.Survivors.VoidHuntress;
using Henry2Mod.Survivors.VoidHuntress.SkillStates;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.Components
{
    public class VoidHuntressVoidState : MonoBehaviour
    {
        private static float maxVoidMeter = 100.0f;

        public float voidMeter;
        public float voidStateStartTime;
        public bool isCorruptModeActive;
        private CharacterBody m_body;
        private SkillDef[] m_overrides;

        public void Init(CharacterBody body, SkillDef[] overrides)
        {
            voidMeter = 0.0f;
            isCorruptModeActive = false;
            m_body = body;
            m_overrides = overrides;
        }

        private void Update()
        {
            CalculateVoidDecay();
            if (isCorruptModeActive)
            {

            }
        }

        private void Awake()
        {
            CharacterBody body = gameObject.GetComponent<CharacterBody>();
        }

        private void CalculateVoidDecay()
        {
            if (isCorruptModeActive)
            {
                int decayRatePerSecond = (int)(Time.time - voidStateStartTime);
                m_body.SetBuffCount(VoidHuntressBuffs.voidSicknessBuff.buffIndex, decayRatePerSecond);

                voidMeter -= decayRatePerSecond * Time.deltaTime;

                if (voidMeter <= 0.0f)
                {
                    TransitionToLunarState();
                }
            }
        }

        public void TransitionToVoidState()
        {
            if (isCorruptModeActive) return;

            isCorruptModeActive = true;
            voidMeter = maxVoidMeter;
            voidStateStartTime = Time.time;

            Log.Warning("[Void Transition]");
            Log.Warning(m_body.skillLocator.utility);

            m_body.SetBuffCount(VoidHuntressBuffs.quickShot.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.quickShot);


            m_body.skillLocator.primary.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Primary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.utility.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Utility], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.secondary.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Secondary], GenericSkill.SkillOverridePriority.Upgrade);

            foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
            {
                m_body.skillLocator.GetSkill(item).stock = m_body.skillLocator.GetSkill(item).maxStock;
            }



        }

        public void TransitionToLunarState()
        {
            if (!isCorruptModeActive) return;

            isCorruptModeActive = false;
            voidMeter = 0;

            m_body.SetBuffCount(VoidHuntressBuffs.voidSicknessBuff.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.voidSicknessBuff);
            m_body.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.voidShot);


            m_body.skillLocator.primary.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Primary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.utility.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Utility], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.secondary.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Secondary], GenericSkill.SkillOverridePriority.Upgrade);

            foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
            {
                m_body.skillLocator.GetSkill(item).stock = m_body.skillLocator.GetSkill(item).maxStock;
            }


        }

        public void SetVoidMeter(float value)
        {
            voidMeter = Mathf.Clamp(value, 0, maxVoidMeter);
        }

        public void AddVoidMeter(float value)
        {
            SetVoidMeter(voidMeter + value);
        }

        public float GetFillPercent()
        {
            return voidMeter / maxVoidMeter;
        }

        public bool CanTransform()
        {
            return (!isCorruptModeActive) && 0.1f >= (maxVoidMeter - voidMeter);
        }

    }
}
