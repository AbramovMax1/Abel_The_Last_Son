using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.World.Doors;

public class LockedDoor : Sprite
{
    public LockedDoor() : base("RightDoorLocked")
    {
    }

    public override void Start()
    {
        base.Start();
        transform.position = new Vector2(1825f, 540f);
        transform.scale = new Vector2(13f, 13f);
        sortingOrder = 3;
    }
}