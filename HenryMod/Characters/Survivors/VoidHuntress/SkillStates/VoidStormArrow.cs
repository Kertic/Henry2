namespace Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates
{
    public class VoidStormArrow : StormArrow
    {
        public override void OnEnter()
        {
            activateSound = "Play_voidman_m2_chargeUp";
            stacksGainedOnUse = 3;
            base.OnEnter();
        }

    }
}
