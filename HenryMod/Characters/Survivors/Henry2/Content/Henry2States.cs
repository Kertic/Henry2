using Henry2Mod.Survivors.Henry.SkillStates;

namespace Henry2Mod.Survivors.Henry
{
    public static class Henry2States
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(MultiShot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
