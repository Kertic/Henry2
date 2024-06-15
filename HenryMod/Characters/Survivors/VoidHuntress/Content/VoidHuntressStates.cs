using Henry2Mod.Characters.Survivors.VoidHuntress.SkillStates;
using Henry2Mod.Survivors.Henry.SkillStates;
using Henry2Mod.Survivors.VoidHuntress.SkillStates;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(VoidSnipe));

            Modules.Content.AddEntityState(typeof(MultiShot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(Backflip));

            Modules.Content.AddEntityState(typeof(ThrowBomb));

            Modules.Content.AddEntityState(typeof(VoidBomb));

        }
    }
}
