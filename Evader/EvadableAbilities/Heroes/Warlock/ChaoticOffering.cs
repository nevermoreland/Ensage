﻿namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class ChaoticOffering : LinearAOE
    {
        #region Constructors and Destructors

        public ChaoticOffering(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
        }

        #endregion
    }
}