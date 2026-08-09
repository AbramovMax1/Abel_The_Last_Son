using System;
using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.World.Trash;

public class Rock : Sprite, ICollidable
{
    public Rock() : base("Rock")
    {
    }
    public override void Start()
    {
        transform.position = new Vector2(1650f, 360f);
        transform.scale = new Vector2(2f, 2f);
        sortingOrder = 2;
        base.Start();
    }

    public Rectangle Collider
    {
        get
        {
            if (texture == null) return Rectangle.Empty;

            float width = texture.Width * transform.scale.X;
            float height = texture.Height * transform.scale.Y;

            // Center rectangle around transform.position
            return new Rectangle(
                (int)(transform.position.X - (width * 0.5f)),
                (int)(transform.position.Y - (height * 0.5f)),
                (int)width,
                (int)height
            );
        }
    }
}