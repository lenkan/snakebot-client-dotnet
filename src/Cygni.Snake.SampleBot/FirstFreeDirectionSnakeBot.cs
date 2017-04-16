using System;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class FirstFreeDirectionSnakeBot : SnakeBot
    {
        public FirstFreeDirectionSnakeBot() : base("DotNetFree")
        {
        }

        public override Direction GetNextMove(Map map)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var newHeadPosition = map.MySnake.HeadPosition.GetDestination(direction);
                if (!map.IsObstace(newHeadPosition) && !map.IsSnake(newHeadPosition) && newHeadPosition.IsInsideMap(map.Width, map.Height))
                {
                    return direction;
                }
            }

            // Fall back value
            return Direction.Down;
        }
    }
}