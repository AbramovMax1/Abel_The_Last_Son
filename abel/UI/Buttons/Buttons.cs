using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Abel_The_Last_Son;

public class Buttons
{
    private Rectangle bounds;
    
    // ========== Texture ===========
    private Texture2D texture;
    private Texture2D colorTexture;

    // ========== Buttons color ============
    private Color normalColor;
    private Color hoverColor;
    
    // ========== Text ================
    private string text;
    private SpriteFont font;
    private Color textColor;
    private float textScale;
    
    // ========== Mouse ==============
    private MouseState currentMouse;
    private MouseState previousMouse;
    public Action OnClick;

    public Buttons(GraphicsDevice graphicsDevice, Rectangle bounds)
    {
        this.bounds = bounds;
        
        normalColor = Color.White; 
        hoverColor = Color.LightGray; // hover color 

        text = "";
        textColor = Color.Black; // text color 
        textScale = 1f;
        
        colorTexture = new Texture2D(graphicsDevice, 1, 1);
        colorTexture.SetData(new[] { Color.White });
    }
    
    public void SetTexture(Texture2D texture)
    {
        this.texture = texture;
    }

    public void SetColors(Color normalColor, Color hoverColor)
    {
        this.normalColor = normalColor;
        this.hoverColor = hoverColor;
    }

    public void SetText(string text, SpriteFont font, Color textColor, float textScale)
    {
        this.text = text;
        this.font = font;
        this.textColor = textColor;
        this.textScale = textScale;
    }

    public void Update()
    {
        previousMouse = currentMouse;
        currentMouse = Mouse.GetState();

        bool isHovering = bounds.Contains(currentMouse.Position);

        bool clicked =
            isHovering &&
            currentMouse.LeftButton == ButtonState.Pressed &&
            previousMouse.LeftButton == ButtonState.Released;

        if (clicked)
        {
            OnClick?.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        bool isHovering = bounds.Contains(Mouse.GetState().Position);

        Color drawColor = isHovering ? hoverColor : normalColor;

        if (texture != null)
        {
            // Draw image button.
            spriteBatch.Draw(texture, bounds, drawColor);
        }
        else
        {
            // Draw solid color button.
            spriteBatch.Draw(colorTexture, bounds, drawColor);
        }

        if (font != null && text != "")
        {
            Vector2 textSize = font.MeasureString(text) * textScale;

            Vector2 textPosition = new Vector2(
                bounds.Center.X - textSize.X * 0.5f,
                bounds.Center.Y - textSize.Y * 0.5f
            );

            spriteBatch.DrawString(
                font,
                text,
                textPosition,
                textColor,
                0f,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}