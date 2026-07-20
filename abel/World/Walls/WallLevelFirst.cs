using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.World.Walls;

public class WallLevelFirst : Sprite
{
    public WallLevelFirst() : base("WallsLevelOne")
    {
    }

    public override void Start()
    {
        base.Start();
        transform.position = Game1._screenCenter; // center the sprite 
        transform.scale = new Vector2(15f, 17f); // scale for the walls
        sortingOrder = 2;
    }
}