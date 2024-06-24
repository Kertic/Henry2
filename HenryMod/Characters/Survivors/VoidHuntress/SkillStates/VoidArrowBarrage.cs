using EntityStates;
using Henry2Mod.Survivors.VoidHuntress;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidArrowBarrage : BaseSkillState
    {

        public static float baseDuration = 0.1f;
        public static string activateSound = "Play_voidman_m2_chargeUp";
        public static int stacksGainedOnUse = 1;

        protected int buffsToAdd;
        public override void OnEnter()
        {
            int maxBuffs = stacksGainedOnUse * skillLocator.secondary.maxStock;
            int currentBuffs = characterBody.GetBuffCount(VoidHuntressBuffs.quickShot);
            buffsToAdd = Mathf.Min(maxBuffs - currentBuffs, stacksGainedOnUse);

            RoR2.Util.PlaySound(activateSound, gameObject);
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }

        }

        public override void OnExit()
        {
            base.OnExit();

            for (int i = 0; i < buffsToAdd; i++)
            {
                characterBody.AddBuff(VoidHuntressBuffs.voidShot);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }


    }
}
