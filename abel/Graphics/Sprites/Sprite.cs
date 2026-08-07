using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class Sprite : IUpdateable, IDrawable
{
    // Every sprite has a position, rotation, and scale.
    public Transform transform = new Transform();

    // The image currently being drawn.
    public Texture2D texture;

    // Information about the texture's columns, rows, and frames.
    public SpriteSheet spriteSheet;

    // White displays the texture's original colors.
    public Color color = Color.White;

    // Lower numbers are drawn first.
    public int sortingOrder = 0;

    // Controls whether the sprite is flipped.
    public SpriteEffects spriteEffects = SpriteEffects.None;

    // The small section of the sprite sheet currently displayed.
    protected Rectangle? sourceRectangle = null;

    // The sprite's position and size on the game screen.
    protected Rectangle destinationRectangle;

    // The sprite frame's center point.
    private Vector2 origin;

    // Every child sprite supplies the registered sprite name.
    public Sprite(string spriteName)
    {
        // Find the requested sprite sheet.
        spriteSheet = SpriteManager.GetSprite(spriteName);
    }

    // Start prepares the sprite's texture and first frame.
    public virtual void Start()
    {
        // Get the texture from the sprite sheet.
        texture = spriteSheet.texture;

        // Begin with the first frame.
        SetFrame(0, 0);
    }

    // Child classes can replace this method with their own update behavior.
    public virtual void Update(GameTime gameTime)
    {
    }

    // Calculate where and how large the sprite should be on the screen.
    private Rectangle GetDestinationRectangle(
        Rectangle? sourceRectangle)
    {
        // Return an empty rectangle if no animation frame was selected.
        if (sourceRectangle == null)
        {
            return new Rectangle();
        }

        // Multiply the frame width by the sprite's horizontal scale.
        int width =
            (int)(sourceRectangle.Value.Width * transform.scale.X);

        // Multiply the frame height by the sprite's vertical scale.
        int height =
            (int)(sourceRectangle.Value.Height * transform.scale.Y);

        // Position the sprite using its centered origin.
        int positionX =
            (int)(transform.position.X - origin.X * transform.scale.X);

        // Position the sprite using its centered origin.
        int positionY =
            (int)(transform.position.Y - origin.Y * transform.scale.Y);

        // Return the completed screen rectangle.
        return new Rectangle(
            positionX,
            positionY,
            width,
            height);
    }

    // Draw this sprite using MonoGame's SpriteBatch.
    public virtual void DrawSprite(SpriteBatch spriteBatch)
    {
        // Recalculate the destination rectangle.
        destinationRectangle =
            GetDestinationRectangle(sourceRectangle);

        // Draw the selected sprite-sheet frame.
        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            color,
            MathHelper.ToRadians(transform.rotation),
            Vector2.Zero,
            spriteEffects,
            0f);
    }

    // Select one frame from the sprite sheet.
    protected void SetFrame(int column, int row)
    {
        // Find the requested frame rectangle.
        sourceRectangle = spriteSheet[column, row];

        // Calculate the center point of this individual frame.
        origin = new Vector2(
            sourceRectangle.Value.Width * 0.5f,
            sourceRectangle.Value.Height * 0.5f);
    }
}