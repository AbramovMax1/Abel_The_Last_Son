using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class Camera
{
    public Matrix Transform { get; private set; }
    private Viewport _viewport;
    private Vector2 _position;

    // By using a property, we automatically update the camera's math 
    // every single time you change its position!
    public Vector2 Position
    {
        get { return _position; }
        set
        {
            _position = value;
            UpdateTransform();
        }
    }

    public Camera(Viewport viewport)
    {
        _viewport = viewport;
    }

    private void UpdateTransform()
    {
        // Moves the world in the opposite direction of the target,
        // and offsets it by half the screen dimensions to keep the target centered.
        Transform = Matrix.CreateTranslation(
            -_position.X + (_viewport.Width * 0.5f),
            -_position.Y + (_viewport.Height * 0.5f),
            0);
    }
}