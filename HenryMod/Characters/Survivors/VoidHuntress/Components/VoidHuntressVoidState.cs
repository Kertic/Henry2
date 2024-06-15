using Henry2Mod.Characters.Survivors.VoidHuntress.UI;
using Henry2Mod.Survivors.VoidHuntress;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
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

        private void Awake()
        {

            Log.Warning("[VoidStateAwake]");
        }

        private void Update()
        {
            CalculateVoidDecay();
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
            isCorruptModeActive = true;
            voidMeter = maxVoidMeter;
        }

        public void TransitionToLunarState()
        {
            isCorruptModeActive = false;
            voidMeter = 0;
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

    }
}
