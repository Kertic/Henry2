using EntityStates;
using Henry2Mod.Survivors.VoidHuntress;
using UnityEngine;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class ArrowFlurry : BaseSkillState
    {

        public static float damageCoefficient = VoidHuntressStatics.lunarBowDmgCoeff;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.01f;
        public static string activateSound = "Play_railgunner_m2_scope_in";
        public static int stacksGainedOnUse = 6;

        protected int buffsToAdd;
        public override void OnEnter()
        {
            int maxBuffs = 6;
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
