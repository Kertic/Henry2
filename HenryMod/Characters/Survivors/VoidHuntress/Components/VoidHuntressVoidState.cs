using Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates;
using Henry2Mod.Survivors.VoidHuntress;
using Henry2Mod.Survivors.VoidHuntress.SkillStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;

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

            m_body.skillLocator.utility.SetSkillOverride(m_body, m_overrides[(int)VoidHuntressSurvivor.SkillOverrideTypes.Utility], GenericSkill.SkillOverridePriority.Upgrade);
        }

        public void TransitionToLunarState()
        {
            if (!isCorruptModeActive) return;

            isCorruptModeActive = false;
            voidMeter = 0;
            m_body.SetBuffCount(VoidHuntressBuffs.voidSicknessBuff.buffIndex, 1);
            m_body.RemoveBuff(VoidHuntressBuffs.voidSicknessBuff);
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
