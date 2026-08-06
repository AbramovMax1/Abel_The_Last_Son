using System;
using Abel_The_Last_Son;
using Abel_The_Last_Son.Core.Collider;
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


public class Door : Sprite
{
    public int position { get; private set; }
    private bool open;
    private bool locked;
    private Room.Direction direction;
    
    public Collider doorCollider;

    public Door(Room.Direction direction , string doorSpriteName, bool open ) : base(doorSpriteName)
    {
        this.position = position;
        this.open = open;
        this.direction = direction;

        if (doorSpriteName == "DoorFrameLockedFlootTwo") transform.rotation -= MathHelper.ToRadians(90);
        
        HandleRotation();
    }

    public override void Start()
    {
        base.Start();
        
        // Set up the trigger collider
        doorCollider = new Collider();
        doorCollider.Parent = this; 
        doorCollider.IsTrigger = true; // Make it a trigger to detect room transitions!
        // Register the transition logic
        doorCollider.RegisterOnTrigger(OnPlayerEnterDoor);
        
    }
    
    private void OnPlayerEnterDoor(Collider door, Collider other)
    {
        if (open)
        {
            // TODO: trigger room transition logic
        }

        if (locked)
        {
            //TODO: take key logic
            Open();
        }
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
        switch (direction)
        {
            case Room.Direction.Up:
            {
                break;
            }
            case Room.Direction.Right:
            {
                transform.rotation += MathHelper.ToRadians(90);
                break;
            }
            case Room.Direction.Down:
            {
                transform.rotation += MathHelper.ToRadians(180);
                break;
            }
            case Room.Direction.Left:
            { 
                transform.rotation += MathHelper.ToRadians(270);
                break;
            }
        }
    }
}