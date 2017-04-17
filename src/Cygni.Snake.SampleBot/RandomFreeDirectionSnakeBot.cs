using System;
using System.Collections.Generic;
using System.Linq;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class RandomFreeDirectionSnakeBot : SnakeBot
    {
        private readonly Random _random = new Random();

        public RandomFreeDirectionSnakeBot() : base("DotNetRandom")
        {
        }

        private IEnumerable<Direction> GetFreeDirections(Map map)
        {
            var freeDirections = new List<Direction>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var newHeadPosition = map.MySnake.HeadPosition.GetDestination(direction);
                if (IsPositionSafe(map, newHeadPosition))
                {
                    freeDirections.Add(direction);
                }
            }
            return freeDirections;
        }

        public bool IsPositionSafe(Map map, MapCoordinate position)
        {
            if (!map.IsObstace(position) && !map.IsSnake(position) && position.IsInsideMap(map.Width, map.Height))
                return false;

            // Check if other snakes may occupy this position in the next tick:
            return map.SnakeHeads.Except(new[] {map.MySnake.HeadPosition}).Any(x => x.GetManhattanDistanceTo(position) == 1);
        }

        public override Direction GetNextMove(Map map)
        {
            var freeDirections = GetFreeDirections(map).ToList();

            if (!freeDirections.Any())
                return Direction.Down;

            return freeDirections[_random.Next(0, freeDirections.Count - 1)];
        }
    }
}