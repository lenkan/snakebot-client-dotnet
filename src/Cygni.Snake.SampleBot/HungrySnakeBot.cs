using System;
using System.Collections.Generic;
using System.Linq;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class HungrySnakeBot : SnakeBot
    {
        public HungrySnakeBot() : base("DotNetHungry")
        {
        }

        private IEnumerable<MapCoordinate> GetFoodByDistance(Map map)
        {
            var headPosition = map.MySnake.HeadPosition;
            return map.FoodPositions.OrderBy(x => x.GetManhattanDistanceTo(headPosition));
        }

        private IEnumerable<Direction> GetFreeDirections(Map map)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var coordinate = map.MySnake.HeadPosition.GetDestination(direction);
                if (!map.IsSnake(coordinate) && !map.IsObstacle(coordinate) && map.IsCoordinateInsideMap(coordinate))
                {
                    yield return direction;
                }
            }
        }

        private Direction GetDirectionToCoordinate(Map map, MapCoordinate coordinate)
        {
            var headPosition = map.MySnake.HeadPosition;
            return GetFreeDirections(map).OrderBy(direction => headPosition.GetDestination(direction).GetManhattanDistanceTo(coordinate))
                .DefaultIfEmpty(Direction.Down)
                .First();
        }

        public override Direction GetNextMove(Map map)
        {
            var closestFood = GetFoodByDistance(map).FirstOrDefault() ?? new MapCoordinate(0, 0);
            return GetDirectionToCoordinate(map, closestFood);
        }
    }
}