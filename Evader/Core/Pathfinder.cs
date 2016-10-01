﻿namespace Evader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Pathfinder
    {
        #region Fields

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly float turnRate;

        private readonly Dictionary<Unit, uint> units = new Dictionary<Unit, uint>();

        #endregion

        #region Constructors and Destructors

        public Pathfinder()
        {
            Pathfinding = new NavMeshPathfinding();
            Game.OnIngameUpdate += PositionUpdater;
            try
            {
                turnRate = (float)Hero.GetTurnRate();
            }
            catch (Exception)
            {
                // demo mode, lets set random value
                turnRate = 0.5f;
            }
        }

        #endregion

        #region Public Properties

        public NavMeshPathfinding Pathfinding { get; }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        #endregion

        #region Public Methods and Operators

        public uint? AddObstacle(Vector3 position, float radius, uint? obstacle = null)
        {
            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            if (radius > 5000)
            {
                return null;
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(position.SetZ(600), radius);
        }

        public uint? AddObstacle(Vector3 startPosition, Vector3 endPosition, float width, uint? obstacle = null)
        {
            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            if (startPosition.Distance2D(endPosition) > 5000)
            {
                return null;
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(startPosition.SetZ(600), endPosition.SetZ(600), width);
        }

        public uint? AddObstacle(
            Vector3 startPosition,
            Vector3 endPosition,
            float startWidth,
            float endWidth,
            uint? obstacle = null)
        {
            // todo: fix start => end widths
            // return Pathfinding.AddObstacle(startPosition, endPosition, startWidth, endWidth);

            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            if (startPosition.Distance2D(endPosition) > 5000)
            {
                return null;
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(
                startPosition.SetZ(600),
                endPosition.SetZ(600),
                Math.Max(startWidth, endWidth));
        }

        public IEnumerable<Vector3> CalculateDebugPathFromObstacle(float remainingTime)
        {
            bool success;
            return Pathfinding.CalculatePathFromObstacle(
                Hero.NetworkPosition,
                Hero.NetworkPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                turnRate,
                remainingTime * 1000,
                false,
                out success);
        }

        public IEnumerable<Vector3> CalculateLongDebugPath(Vector3 endPosition)
        {
            bool success;
            return Pathfinding.CalculateLongPath(Hero.Position, endPosition, 5000, false, out success);
        }

        public IEnumerable<Vector3> CalculateLongPath(Vector3 endPosition, out bool success)
        {
            return Pathfinding.CalculateLongPath(Hero.Position, endPosition, 5000, true, out success);
        }

        public IEnumerable<Vector3> CalculatePathFromObstacle(float remainingTime, out bool success)
        {
            return Pathfinding.CalculatePathFromObstacle(
                Hero.NetworkPosition,
                Hero.NetworkPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                turnRate,
                remainingTime * 1000,
                true,
                out success);
        }

        public IEnumerable<Vector3> CalculatePathFromObstacle(
            Vector3 startPosition,
            float remainingTime,
            out bool success)
        {
            return Pathfinding.CalculatePathFromObstacle(
                startPosition,
                Hero.NetworkPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                turnRate,
                remainingTime * 1000,
                true,
                out success);
        }

        public void Close()
        {
            Game.OnIngameUpdate -= PositionUpdater;
            units.Clear();
        }

        public IEnumerable<uint> GetIntersectingObstacles(Hero hero)
        {
            //////////<<<<<<<<<<<
            /// 
            var obstacles = Pathfinding.GetIntersectingObstacleIDs(hero.NetworkPosition, hero.HullRadius).ToList();
            if (hero.IsMoving)
            {
                obstacles.AddRange(Pathfinding.GetIntersectingObstacleIDs(hero.InFront(150), hero.HullRadius));
            }
            return obstacles;
        }

        public IEnumerable<uint> GetIntersectingObstacles(Vector3 position, float radius)
        {
            return Pathfinding.GetIntersectingObstacleIDs(position, radius);
        }

        public void RemoveObstacle(uint id)
        {
            Pathfinding.RemoveObstacle(id);
        }

        public void UpdateObstacle(uint id, Vector3 position, float radius)
        {
            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, position.SetZ(600), radius);
        }

        public void UpdateObstacle(uint id, Vector3 position, float startWidth, float endWidth)
        {
            // FAIL
            // Pathfinding.UpdateObstacle(id, position, startWidth, endWidth);

            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, position.SetZ(600), endWidth, endWidth);
        }

        public void UpdateObstacle(uint id, Vector3 startPosition, Vector3 endPosition)
        {
            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, startPosition.SetZ(600), endPosition.SetZ(600));
        }

        #endregion

        #region Methods

        private void PositionUpdater(EventArgs args)
        {
            if (!sleeper.Sleeping(units))
            {
                foreach (var unit in
                    ObjectManager.GetEntities<Unit>()
                        .Where(
                            x =>
                            x.IsValid && !units.ContainsKey(x) && (x is Creep || x is Hero || x is Building)
                            && x.IsSpawned && x.IsAlive && !x.Equals(Hero)))
                {
                    var obstacle = AddObstacle(unit.NetworkPosition, unit.HullRadius);
                    if (obstacle != null)
                    {
                        units.Add(unit, obstacle.Value);
                    }
                }

                sleeper.Sleep(1000, units);
            }

            if (!sleeper.Sleeping(Pathfinding))
            {
                var remove = new List<Unit>();
                foreach (var unitPair in units)
                {
                    var unit = unitPair.Key;
                    var id = unitPair.Value;

                    if (unit == null || !unit.IsValid || !unit.IsAlive)
                    {
                        remove.Add(unit);
                        RemoveObstacle(id);
                        continue;
                    }

                    UpdateObstacle(id, unit.NetworkPosition, unit.HullRadius);
                }

                foreach (var unit in remove)
                {
                    units.Remove(unit);
                }

                sleeper.Sleep(100, Pathfinding);
            }
        }

        #endregion
    }
}