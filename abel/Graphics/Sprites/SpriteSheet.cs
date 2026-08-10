using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class SpriteSheet
{
    public int columns { get; set; }
    public int rows {get; set;}
    public Texture2D texture{ get; set;}

    public Rectangle this[int x, int y]
    {
        get
        {
            
            int width = texture.Width / columns; // Each small frame width.
            int height = texture.Height / rows; // Each small frame height.

            int postion_x = width * x;
            int postion_y = height * y;

            return new Rectangle(
                postion_x,
                postion_y,
                width,
                height);
        }
            
    }
}