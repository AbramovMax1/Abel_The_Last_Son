using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Dataflow;
using System.Windows.Markup;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Abel_The_Last_Son.Weapons.Projectiles;

public class HolyWaterProjectile : Sprite , IProjectile
{
    private Vector2 movementDirection = Vector2.Zero;
    
    // Projectile Settings
    private const float MovementSpeed = 650f; // projectile move 650 pixel per sec
    private const float MaximumLife = 2f; // for every active projectile he has 2 second life
    private float remainingLife = 0f; // count how much life remain during active projectile
    public int Damage { get; } = 1;
    public bool IsActive { get; private set; } = false;
    
    
    //draw the Collider to our projectile 
    public Rectangle Collider
    {
        get
        {
            if (!IsActive)
            {
                return Rectangle.Empty; // no projectile active nothing to draw
            }

            int size = 35;
            
            int x = (int)transform.position.X - size /2;
            int y = (int)transform.position.Y - size / 2;
            return new Rectangle(x, y, size, size);
        }
    }

    public HolyWaterProjectile() : base("HolyWater")
    {
        
    }

    public override void Start()
    {
        base.Start();

        transform.scale = new Vector2(0.75f, 0.75f);

        sortingOrder = 5; 
        
        IsActive = false;
    }

    public void Activate(Vector2 startingPosition, Vector2 direction)
    {
        if (direction == Vector2.Zero)
        {
            movementDirection = Vector2.UnitY;
        }
        else
        {
            movementDirection = Vector2.Normalize(direction);
        }
        
        transform.position = startingPosition +  movementDirection * 70f;

        transform.rotation = 0f;

        color = Color.White;
        
        remainingLife = MaximumLife;
        
        IsActive = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!IsActive)
        {
            return;
        }
        
        float deleteTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        transform.position += movementDirection * MovementSpeed * deleteTime;
        
        remainingLife -= deleteTime;

        if (remainingLife <= 0f)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        IsActive = false;
        
        movementDirection = Vector2.Zero;
        
        remainingLife = 0f;
    }
}