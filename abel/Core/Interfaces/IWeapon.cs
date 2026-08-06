using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son;

public interface IWeapon
{
    bool CanAttack { get; }
    
    
    //IReadonly list mean Game1 can only read the projectile but he cant "clear"
    // game1 cant do player.weapon.projectile.clear();
    IReadOnlyList<IProjectile> Projectiles { get;  }
    
    void Update(GameTime gameTime);
    bool TryAttack(Vector2 startingPosition, Vector2 direction);
}