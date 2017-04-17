using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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

        public int GetDangerForCoordinate(Map map, MapCoordinate coordinate)
        {
            int dangerRating = IsCoordinateSafe(map, coordinate) ? 0 : 100;
            foreach (var surroundingCoordinate in GetSurroundingCoordinates(coordinate, 5).Except(new []{coordinate}))
            {
                int distance = coordinate.GetManhattanDistanceTo(surroundingCoordinate);
                dangerRating += 5 * (map.IsSnake(surroundingCoordinate) ? 1 : 0) / distance;
                dangerRating += 5 * (map.SnakeHeads.Except(new []{map.MySnake.HeadPosition}).Contains(surroundingCoordinate) ? 1 : 0) / distance;
                dangerRating += 2 * (map.IsObstacle(surroundingCoordinate) ? 1 : 0) / distance;
                dangerRating += 2 * (!map.IsCoordinateInsideMap(surroundingCoordinate) ? 1 : 0) / distance;
            }
            return dangerRating;
        }

        private  bool IsCoordinateSafe(Map map, MapCoordinate coordinate)
        {
            return !map.IsObstacle(coordinate) && !map.IsSnake(coordinate) && map.IsCoordinateInsideMap(coordinate);
        }

        public int GetDangerForDirection(Map map, Direction direction)
        {
            var destination = map.MySnake.HeadPosition.GetDestination(direction);
            return GetDangerForCoordinate(map, destination);
        }

        public override Direction GetNextMove(Map map)
        {
            var directionPoints = Enum.GetValues(typeof(Direction)).Cast<Direction>()
                .Select(dir => new {Points = GetDangerForDirection(map, dir), Direction = dir})
                .OrderBy(x => x.Points)
                .FirstOrDefault();
            return directionPoints.Direction;
        }
    }
}