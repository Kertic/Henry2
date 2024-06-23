using EntityStates;
using EntityStates.LunarWisp;
using Henry2Mod.Survivors.VoidHuntress;
using IL.RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class StormArrow : BaseSkillState
    {

        public static float damageCoefficient = VoidHuntressStaticValues.primaryBowDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.5f;
        public static string activateSound = "Play_railgunner_m2_scope_in";
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
                characterBody.AddBuff(VoidHuntressBuffs.quickShot);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }


    }
}
