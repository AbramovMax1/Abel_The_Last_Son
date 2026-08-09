using Microsoft.Xna.Framework;


namespace Abel_The_Last_Son.Items;

public class DoorKey : Sprite, ICollectible
{
    public bool IsCollected { get; private set; } = false;

    public Rectangle Collider
    {
        get
        {
            if (IsCollected)
            {
                return Rectangle.Empty;
            }

            int size = 60;
            
            int x = (int)transform.position.X - size / 2;
            
            int y = (int)transform.position.Y - size / 2;
            
            return new Rectangle(x, y, size, size);
        }
    }

    public DoorKey(Vector2 startingPosition) : base("DoorKey")
    {
        transform.position = startingPosition;
    }

    public override void Start()
    {
        base.Start();

        transform.scale = new Vector2(3f, 3f);

        sortingOrder = 4;
        
    }

    public void Collect(Player player)
    {
        if (IsCollected)
        {
            return;
        }
        
        player.AddKeys(1);
        
        IsCollected = true;
    }
}