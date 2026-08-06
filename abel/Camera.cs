using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class Camera
{
    public Matrix Transform { get; private set; }
    private Viewport _viewport;

    public Camera(Viewport viewport)
    {
        _viewport = viewport;
    }

    public void Follow(Vector2 targetPosition)
    {
        // Moves the world in the opposite direction of the target,
        // and offsets it by half the screen dimensions to keep the target centered.
        Transform = Matrix.CreateTranslation(
            -targetPosition.X + (_viewport.Width * 0.5f),
            -targetPosition.Y + (_viewport.Height * 0.5f),
            0);
    }

    public void MoveRoomUp(Vector2 targetRoomPosition)
    {
        // Transform = Matrix.CreateTranslation(
        //   )
    }
}