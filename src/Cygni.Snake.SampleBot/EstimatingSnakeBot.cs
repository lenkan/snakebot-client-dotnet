using System;
using System.Collections.Generic;
using System.Linq;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class EstimatingSnakeBot : SnakeBot
    {
        public EstimatingSnakeBot() : base((string) "Estimating")
        {
        }

        public IEnumerable<MapCoordinate> GetSurroundingCoordinates(MapCoordinate coordinate, int size)
        {
            int startX = coordinate.X - size / 2;
            int startY = coordinate.Y - size / 2;
            int stopX = coordinate.X + size / 2;
            int stopY = coordinate.Y + size / 2;

            for (int x = startX; x <= stopX; x++)
            {
                for (int y = startY; y <= stopY; y++)
                {
                    yield return new MapCoordinate(x, y);
                }
            }
        }

        public int GetNumberOfObstaclesAroundCoordinate(Map map, MapCoordinate coordinate)
        {
            int numberOfObstacles = 0;
            foreach (var surroundingCoordinate in GetSurroundingCoordinates(coordinate, 5))
            {
                if (!IsCoordinateSafe(map, surroundingCoordinate))
                {
                    numberOfObstacles = numberOfObstacles + 1;
                }
            }
            return numberOfObstacles;
        }

        private  bool IsCoordinateSafe(Map map, MapCoordinate coordinate)
        {
            return !map.IsObstacle(coordinate) && !map.IsSnake(coordinate) && map.IsCoordinateInsideMap(coordinate);
        }

        public int GetWeightForDirection(Map map, Direction direction)
        {
            var destination = map.MySnake.HeadPosition.GetDestination(direction);
            if (!IsCoordinateSafe(map, destination))
            {
                return 100;
            }
            return GetNumberOfObstaclesAroundCoordinate(map, destination);
        }

        public override Direction GetNextMove(Map map)
        {
            var directionPoints = Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .Select(dir => new {Points = GetWeightForDirection(map, dir), Direction = dir})
                .OrderBy(x => x.Points)
                .FirstOrDefault();
            return directionPoints.Direction;
        }
    }
}