using System;
using Abel_The_Last_Son;
using Abel_The_Last_Son.Core.Enums;
using Abel_The_Last_Son.World.Doors;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;
/*
 * keynotes
 * positions
 * 0 = up
 * 1 = right
 * 2 = down
 * 3 = left
 */


public class Door : Sprite, ICollidable
{
    public int position { get; private set; }
    public bool open {get; private set;}
    private bool locked;
    private Direction direction;
    
    public Door(Direction direction , string doorSpriteName, bool open ) : base(doorSpriteName)
    {
        this.open = open;
        this.direction = direction;

        float scale = 10f;
        transform.scale = new Vector2(scale, scale);

        if (doorSpriteName == "DoorTwoLocked") transform.rotation -= MathHelper.ToRadians(90);
        
        HandleRotation();
    }

    public override void Start()
    {
        base.Start();
        
    }
    
    public void Open()
    {
        open = true;
    }

    public void Close()
    {
        open = false;
    }
    
    void HandleRotation()
    {
        // Start with a much smaller number. Tweak this up or down by 10s until it perfectly fits the wall.
       
        switch (direction)
        {
            case Direction.Up:
            {
                break;
            }
            case Direction.Right:
            {
                transform.rotation += MathHelper.ToRadians(90);
                break;
            }
            case Direction.Down:
            { 
                transform.rotation += MathHelper.ToRadians(180);
                break;
            }
            case Direction.Left:
            { 
                transform.rotation += MathHelper.ToRadians(270);
                break;
            }
        }
    }


    public Rectangle Collider
    {
        get
        {
            if (texture == null) return Rectangle.Empty;

            float width = texture.Width * transform.scale.X;
            float height = texture.Height * transform.scale.Y;

            // Swap dimensions if door is rotated sideways (90 or 270 deg)
            int degrees = (int)Math.Abs(MathHelper.ToDegrees(transform.rotation)) % 360;
            if (degrees == 90 || degrees == 270)
            {
                (width, height) = (height, width);
            }

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