﻿namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using Utils;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class SacredArrow : LinearProjectile
    {
        #region Fields

        private Unit arrow;

        private bool fowCast;

        #endregion

        #region Constructors and Destructors

        public SacredArrow(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(Invis);
        }

        #endregion

        #region Public Methods and Operators

        public void AddUnit(Unit unit)
        {
            if (Owner.IsVisible)
            {
                return;
            }

            arrow = unit;
            StartCast = Game.RawGameTime;
            EndCast = Game.RawGameTime + GetCastRange() / GetProjectileSpeed();
            StartPosition = unit.Position;
            fowCast = true;
        }

        public override bool CanBeStopped()
        {
            return !IsValidArrow() && base.CanBeStopped();
        }

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase;

            if (phase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if ((phase && Obstacle == null && (int)Owner.RotationDifference == 0) || (fowCast && Obstacle == null))
            {
                if (fowCast && (!IsValidArrow() || !arrow.IsVisible))
                {
                    return;
                }

                if (fowCast)
                {
                    EndPosition = StartPosition.Extend(arrow.Position, GetCastRange());
                }
                else
                {
                    StartPosition = Owner.NetworkPosition;
                    EndPosition = Owner.InFront(GetCastRange() + Radius / 2);
                }

                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
            }
            else if ((StartCast > 0 && time > EndCast) || (fowCast && !IsValidArrow()))
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(fowCast), Radius);
            }
        }

        public override void Draw()
        {
            if (Obstacle == null || (!IsValidArrow() && fowCast))
            {
                return;
            }

            if (Particle == null)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", StartPosition);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }

            Utils.DrawRectangle(StartPosition, EndPosition, Radius);
            Vector2 textPosition;
            Drawing.WorldToScreen(StartPosition, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);

            Particle?.SetControlPoint(0, GetProjectilePosition(fowCast));
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            if (!IsValidArrow())
            {
                return base.GetRemainingTime();
            }

            return (hero.NetworkPosition.Distance2D(arrow) - Radius) / GetProjectileSpeed();
        }

        public override bool IsStopped()
        {
            var check = !IsInPhase && CanBeStopped() && !fowCast;

            if (check)
            {
                End();
            }

            return check;
        }

        #endregion

        #region Methods

        private bool IsValidArrow()
        {
            return arrow != null && arrow.IsValid;
        }

        #endregion
    }
}