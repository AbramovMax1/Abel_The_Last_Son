using Abel_The_Last_Son.Core.Enums;
using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.Core.Helpers;

public static class DirectionHelper
{
    public static Vector2 ToVector(Direction direction)
    {
        return direction switch
        {
            Direction.Right => new Vector2(1f, 0f),
            Direction.Left => new Vector2(-1f, 0f),
            Direction.Down => new Vector2(0f, 1f),
            Direction.Up => new Vector2(0f, -1f),

            _ => Vector2.Zero
        };
    }
}