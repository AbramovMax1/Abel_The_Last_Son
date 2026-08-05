using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son;

public class IWeapon
{
    private bool CanAttack { get; }
    
    // Give other game systems read-only access
    // to this weapon's pooled projectiles.
    IReadOnlyList<IProjectile> Projectiles { get; }

    void Update(GameTime gameTime);
    
    bool TryAttack(Vector2 startingPosition, Vector2 direction);
    // fix the red line !!!!!!!!!!
}