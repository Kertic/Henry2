using Henry2Mod.Characters.Survivors.VoidHuntress.UI;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.Components
{
    public class VoidHuntressState : MonoBehaviour
    {
        private static float maxVoidMeter = 100.0f;

        public float voidMeter;
        public bool isCorruptModeActive;
        private VoidMeter m_meter;
        private CharacterBody m_body;

        private void Awake()
        {
            voidMeter = 0.0f;
            isCorruptModeActive = false;
            m_body = gameObject.GetComponent<CharacterBody>();
        }

        private void Update()
        {
            m_meter.SetFillPercent(voidMeter / maxVoidMeter);

        }

        private void CalculateVoidDecay()
        {
            if(isCorruptModeActive)
            {
                var buffCount = 
            }
        }

    }
}
