using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son;

public interface IProjectile : ICollidable, IUpdateable, IDrawable
{
    int Damage { get; }
    bool IsActive { get; }
    
    void Activate(Vector2 startingPosition, Vector2 direction); // make inactive projectile to in active from the pool
    
    void Destroy(); // destroy the projectile when needed.
}