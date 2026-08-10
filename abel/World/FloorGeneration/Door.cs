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
    public bool locked {get; private set;}
    private Direction direction;
    
    private string closedSprite;
    private string openSprite;
    private string lockedSprite;
    
    public Door(Direction direction, string closedSprite, string openSprite, string lockedSprite, bool open, bool locked = false) 
        : base(locked ? lockedSprite : (open ? openSprite : closedSprite))
    {
        this.direction = direction;
        this.open = open;
        this.locked = locked;
        this.closedSprite = closedSprite;
        this.openSprite = openSprite;
        this.lockedSprite = lockedSprite;

        UpdateTexture();

        float scale = 10f;
        transform.scale = new Vector2(scale, scale);

        if (closedSprite == "DoorTwoLocked" || lockedSprite == "DoorTwoLocked") 
        {
            transform.rotation -= MathHelper.ToRadians(90);
        }
    
        HandleRotation();
    }

    public override void Start()
    {
        base.Start();
        
    }

    public void Open()
    {
        if (!locked)
        {
            open = true;
            UpdateTexture();
        }
    }

    public void Close()
    {
        if (!locked)
        {
            open = false;
            UpdateTexture();
        }
    }
    public void Lock()
    {
        locked = true;
        open = false;
        UpdateTexture();
    }

    public void Unlock()
    {
        locked = false;
        open = true;
        UpdateTexture();
    }
    
    public void ConvertToLocked(string newLockedSprite)
    {
        this.lockedSprite = newLockedSprite;
        Lock();
    }
    private void UpdateTexture()
    {
        string targetSprite = locked ? lockedSprite : (open ? openSprite : closedSprite);
        var spriteSheet = SpriteManager.GetSprite(targetSprite);
        if (spriteSheet != null)
        {
            this.texture = spriteSheet.texture;
        }
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