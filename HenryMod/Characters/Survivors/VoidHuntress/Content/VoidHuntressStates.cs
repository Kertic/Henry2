﻿using Henry2Mod.Survivors.Henry.SkillStates;

namespace Henry2Mod.Survivors.VoidHuntress
{
    public static class VoidHuntressStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(HenryPrimaryShoot));

            Modules.Content.AddEntityState(typeof(MultiShot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}