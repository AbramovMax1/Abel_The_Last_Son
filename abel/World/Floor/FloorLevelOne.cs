using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.World.Floor;

public class FloorLevelOne : Sprite
{
    public FloorLevelOne() : base("FloorLevelOne")
    {
    }

    public override void Start()
    {
        base.Start();
        transform.position = Game1._screenCenter; // center the sprite 
        transform.scale = new Vector2(13f, 13f); // scale for the floor
        sortingOrder = 1;
    }
}