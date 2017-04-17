using System;
using System.Collections.Generic;
using System.Linq;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class SqueezeSnakeBot : SnakeBot
    {
        public SqueezeSnakeBot() : base("DotNetSqueeze")
        {
        }

        private MapCoordinate _destination = new MapCoordinate(0, 0);

        private IEnumerable<Direction> GetFreeDirections(Map map)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var coordinate = map.MySnake.HeadPosition.GetDestination(direction);
                if (!map.IsSnake(coordinate) && !map.IsObstace(coordinate) && map.IsCoordinateInsideMap(coordinate))
                {
                    yield return direction;
                }
            }
        }

        private Direction GetFreeDirectionToCoordinate(Map map, MapCoordinate coordinate)
        {
            var headPosition = map.MySnake.HeadPosition;
            return GetFreeDirections(map).OrderBy(direction => headPosition.GetDestination(direction).GetManhattanDistanceTo(coordinate))
                .DefaultIfEmpty(Direction.Down)
                .First();
        }

        private Direction GetDirectionToCoordinate(Map map, MapCoordinate coordinate)
        {
            var headPosition = map.MySnake.HeadPosition;
            return Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .OrderBy(direction => headPosition.GetDestination(direction).GetManhattanDistanceTo(coordinate))
                .DefaultIfEmpty(Direction.Down)
                .First();
        }

        public override Direction GetNextMove(Map map)
        {
            if (map.MySnake.HeadPosition.Equals(_destination))
            {
                _destination = GetNextDestination(map);
            }

            var straightDirection = GetDirectionToCoordinate(map, _destination);
            var freeDirection = GetFreeDirectionToCoordinate(map, _destination);

            // If direction is inaccesible, go to next destination instead, if that doesn't work,
            // it will just take next free directory.
            if (freeDirection != straightDirection)
            {
                _destination = GetNextDestination(map);
            }

            return GetFreeDirectionToCoordinate(map, _destination);
        }

        private MapCoordinate GetNextDestination(Map map)
        {
            // Increment x with wrap around
            var newX = _destination.X + 1 % map.Width;

            // Toggle Y up/down
            int newY = map.Height - 2 - _destination.Y;

            return new MapCoordinate(newX, newY);
        }
    }
}