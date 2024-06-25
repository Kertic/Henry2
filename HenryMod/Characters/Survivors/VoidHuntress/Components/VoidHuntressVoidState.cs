using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.Components
{
    public class VoidHuntressVoidState : MonoBehaviour
    {
        private static float maxVoidMeter = 100.0f;

        public float currentVoidMeter;
        public float voidStateStartTime;
        public bool isCorruptModeActive;
        private CharacterBody m_body;
        private VoidHuntressTracker m_tracker;
        private SkillDef[] m_overrides;

        public void Init(CharacterBody body, SkillDef[] overrides, VoidHuntressTracker tracker)
        {
            currentVoidMeter = 0.0f;
            isCorruptModeActive = false;
            m_body = body;
            m_overrides = overrides;
            m_tracker = tracker;
        }

        private void Update()
        {
            CalculateVoidDecay();
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

                currentVoidMeter -= decayRatePerSecond * Time.deltaTime;

                if (currentVoidMeter <= 0.0f)
                {
                    m_body.skillLocator.special.ExecuteIfReady();
                }
            }
        }

        public void TransitionToVoidState()
        {
            if (isCorruptModeActive) return;

            isCorruptModeActive = true;
            currentVoidMeter = maxVoidMeter;
            voidStateStartTime = Time.time;


            m_body.SetBuffCount(VoidHuntressBuffs.quickShot.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.quickShot);


            m_body.skillLocator.primary.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Primary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.secondary.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Secondary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.utility.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Utility], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.special.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Special], GenericSkill.SkillOverridePriority.Upgrade);


            foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
            {
                if (item == SkillSlot.None) continue;

                m_body.skillLocator.GetSkill(item).stock = m_body.skillLocator.GetSkill(item).maxStock;
            }
        }

        public void TransitionToLunarState()
        {
            if (!isCorruptModeActive) return;

            isCorruptModeActive = false;
            currentVoidMeter = 0;

            m_body.SetBuffCount(VoidHuntressBuffs.voidSicknessBuff.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.voidSicknessBuff);
            m_body.SetBuffCount(VoidHuntressBuffs.voidShot.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.voidShot);


            m_body.skillLocator.primary.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Primary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.secondary.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Secondary], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.utility.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Utility], GenericSkill.SkillOverridePriority.Upgrade);
            m_body.skillLocator.special.UnsetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Special], GenericSkill.SkillOverridePriority.Upgrade);


            foreach (SkillSlot item in Enum.GetValues(typeof(SkillSlot)))
            {
                if (item == SkillSlot.None) continue;

                m_body.skillLocator.GetSkill(item).stock = m_body.skillLocator.GetSkill(item).maxStock;
            }


        }

        public void SetVoidMeter(float value)
        {
            currentVoidMeter = Mathf.Clamp(value, 0, maxVoidMeter);
        }

        public void AddVoidMeter(float value)
        {
            SetVoidMeter(currentVoidMeter + value);
        }

        public float GetFillPercent()
        {
            return currentVoidMeter / maxVoidMeter;
        }

        public bool CanTransform()
        {
            if (isCorruptModeActive)
            {
                return true;
            }
            return 0.1f >= (maxVoidMeter - currentVoidMeter);
        }

    }
}
