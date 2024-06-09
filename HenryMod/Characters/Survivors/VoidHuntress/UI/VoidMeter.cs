using Henry2Mod.Survivors.VoidHuntress;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.UI
{
    public class VoidMeter : MonoBehaviour
    {
        private GameObject voidHudRoot;
        private GameObject voidHudForeground;
        private Image fillImage;
        private Image backgroundImage;
        public float m_meterPercent { get; protected set; }
        private void Awake()
        {
            voidHudRoot = GenerateMeterCircleObject(gameObject);
            backgroundImage = voidHudRoot.GetComponent<Image>();
            backgroundImage.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

            voidHudForeground = new GameObject("VoidHuntressHUDBackground");
            voidHudForeground.transform.SetParent(voidHudRoot.transform);
            fillImage = GenerateMeterCircleObject(voidHudForeground).GetComponent<Image>();

            SetFillPercent(0.5f);
        }

        static private GameObject GenerateMeterCircleObject(GameObject rootObject)
        {
            if (!rootObject)
            {
                rootObject = new GameObject("VoidHuntressMeterCircle");
            }

            var rectTransform = rootObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(150, 150);
            rectTransform.anchoredPosition = Vector2.zero;


            var newfillImage = rootObject.AddComponent<Image>();
            newfillImage.sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/VoidSurvivor/texVoidSurvivorCorruptionGaugeOutlined.png").WaitForCompletion();
            newfillImage.name = "VoidHuntressHudFillCircle";
            newfillImage.type = Image.Type.Filled;
            newfillImage.fillMethod = Image.FillMethod.Radial360;
            newfillImage.color = VoidHuntressTokens.VOID_COLOR;

            return rootObject;
        }

        public void SetFillPercent(float percent)
        {
            Log.Warning("[VoidMeterFill] : " + percent);
            m_meterPercent = percent;
            fillImage.fillAmount = m_meterPercent;
        }
    }
}
