﻿namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class ScreamOfPain : Projectile
    {
        #region Constructors and Destructors

        public ScreamOfPain(Ability ability)
            : base(ability)
        {
            //todo add multitarget
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}