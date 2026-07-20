using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.World.Trash;

public class NotColletiblesPaper : Sprite
{
    public NotColletiblesPaper() : base("TrashPaper")
    {
    }

    public override void Start()
    {
        base.Start();
        transform.position = new Vector2(1650f, 360f);
        transform.scale = new Vector2(2f, 2f);
        sortingOrder = 2;
    }
}