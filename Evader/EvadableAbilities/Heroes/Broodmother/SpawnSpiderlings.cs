﻿namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class SpawnSpiderlings : Projectile
    {
        #region Constructors and Destructors

        public SpawnSpiderlings(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}