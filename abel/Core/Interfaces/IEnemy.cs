using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son;

public interface IEnemy : ICollidable , IDamageable
{
    int ContactDamage { get; } // How much damage the enemy did damage by touching 
    
    void Update(GameTime gameTime);
}