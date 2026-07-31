using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class Sprite : IUpdateable, IDrawable
{
    public Transform transform = new Transform();
    public Texture2D texture;
    public SpriteSheet spriteSheet;
    public Color color =  Color.White;
    public int sortingOrder = 0;
    public SpriteEffects spriteEffects = SpriteEffects.None;

    protected Rectangle? sourceRectangle = null;
    protected Rectangle destinationRectangle;
    
    private Vector2 origin;

    public Sprite(string spriteName)
    {
        spriteSheet = SpriteManager.GetSprite(spriteName);
    }
    
    public virtual void Start()
    {
        texture = spriteSheet.texture;
        
        SetFrame(0,0);
    }
    

    public virtual void Update(GameTime gameTime)
    {
    }

    private Rectangle GetDestinationRectangle(Rectangle? sourceRectangle)
    {
        if  (sourceRectangle == null) return new Rectangle();
        
        int width = (int)(sourceRectangle.Value.Width * transform.scale.X);
        int height = (int)(sourceRectangle.Value.Height * transform.scale.Y);
        
        int postion_x = (int)(transform.position.X - origin.X * transform.scale.X);
        int postion_y = (int)(transform.position.Y - origin.Y * transform.scale.Y);
        
        return new Rectangle(
            postion_x,
            postion_y,
            width,
            height);
    }

    public void DrawSprite(SpriteBatch spriteBatch)
    {
        
        destinationRectangle = GetDestinationRectangle(sourceRectangle);
       
        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            color,
            MathHelper.ToRadians(transform.rotation),
            Vector2.Zero,
            spriteEffects,
            0f
            );
    }

    protected void SetFrame(int column, int row)
    {
        sourceRectangle = spriteSheet[column, row];

        // centering pivot based on the small frame, not the whole sheet.

        origin = new Vector2(
            sourceRectangle.Value.Width * 0.5f,
            sourceRectangle.Value.Height * 0.5f
        );

    }
}