﻿namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class MysticSnake : Projectile
    {
        #region Constructors and Destructors

        public MysticSnake(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}