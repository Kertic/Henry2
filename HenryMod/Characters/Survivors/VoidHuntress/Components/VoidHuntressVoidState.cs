using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
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

        public void Init(CharacterBody body)
        {
            voidMeter = 0.0f;
            isCorruptModeActive = false;
            m_body = body;
            Log.Warning("[VoidStateInit]");
            Log.Warning(body);
        }

        private void Update()
        {
            CalculateVoidDecay();
        }

        private void Awake()
        {
            CharacterBody body = gameObject.GetComponent<CharacterBody>();
            Init(body);
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
            if (voidMeter >= maxVoidMeter)
            {
                TransitionToVoidState();
            }
        }

        public float GetFillPercent()
        {
            return voidMeter / maxVoidMeter;
        }

    }
}
