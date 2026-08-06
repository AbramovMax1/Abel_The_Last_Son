using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son.Core.Collider;

public class Collider : Sprite
{
    public bool IsTrigger = false; // trigger toggle (only one needs to be a trigger to get a trigger collision)
    public int thickness; // the thickness fo the outline draw (when debugging)

    // Event delegates triggered when an overlap occurs
    private Action<Collider, Collider> _OnTrigger; 
    private Action<Collider, Collider> _OnCollision;
    public Sprite Parent { get; set; } // The parent sprite that this collider is tracking and bound to

    // Initializes a new instance of the Collider class, using a default "Pixel" texture for debug drawing.
    public Collider() : base("Pixel")
    {
    }

    // Checks if this collider's destination rectangle intersects with another collider's rectangle
    public bool IsInterset(Collider other)
    {
        return Parent.destinationRectangle.Intersects(other.Parent.destinationRectangle);
    }

    

    // Notifies the colliders of an overlap, firing either the Trigger event or Collision event 
    // depending on whether either object is designated as a trigger.
    public void Notify(Collider other)
    {
        if (IsTrigger || other.IsTrigger)
            _OnTrigger?.Invoke(this, other); // Fires if at least one object is a trigger zone
        else
            _OnCollision?.Invoke(this, other); // Fires if both objects are solid
    }

    // Draws a visual green outline around the parent sprite's bounding box when running in DEBUG mode.
    public override void DrawSprite(SpriteBatch _spriteBatch)
    {
#if DEBUG
        // draw outline bounds
        
        color = Color.Green;
        thickness = 5;
        
        _spriteBatch.Draw(
            SpriteManager.WhiteTexture,
            new Rectangle(Parent.destinationRectangle.X, Parent.destinationRectangle.Y,
                Parent.destinationRectangle.Width, thickness), // top
            color);

        _spriteBatch.Draw(
            SpriteManager.WhiteTexture,
            new Rectangle(Parent.destinationRectangle.X, Parent.destinationRectangle.Y, thickness,
                Parent.destinationRectangle.Height), // left
            color);

        _spriteBatch.Draw(
            SpriteManager.WhiteTexture,
            new Rectangle(Parent.destinationRectangle.X + Parent.destinationRectangle.Width - thickness,
                Parent.destinationRectangle.Y, thickness, Parent.destinationRectangle.Height), // right
            color);

        _spriteBatch.Draw(
            SpriteManager.WhiteTexture,
            new Rectangle(Parent.destinationRectangle.X,
                Parent.destinationRectangle.Y + Parent.destinationRectangle.Height - thickness, 
                Parent.destinationRectangle.Width, thickness), // bottom
            color);
        
#endif
    }

    // Subscribes a method to the OnTrigger event action.
    public void RegisterOnTrigger(Action<Collider, Collider> action)
    {
        _OnTrigger += action;
    }

    // Subscribes a method to the OnCollision event action.
    public void RegisterOnCollision(Action<Collider, Collider> action)
    {
        _OnCollision += action;
    }
    
    // Unsubscribes a method from the OnTrigger event action.
    public void UnregisterOnTrigger(Action<Collider, Collider> action)
    {
        _OnTrigger -= action;
    }

    // Unsubscribes a method from the OnCollision event action.
    public void UnregisterOnCollision(Action<Collider, Collider> action)
    {
        _OnCollision -= action;
    }
}