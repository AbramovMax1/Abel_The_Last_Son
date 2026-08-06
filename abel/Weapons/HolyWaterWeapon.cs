using System.Collections.Generic;
using Abel_The_Last_Son.Weapons.Pooling;
using Abel_The_Last_Son.Weapons.Projectiles;
using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son.Weapons;

public class HolyWaterWeapon : IWeapon
{
    private const float AttackCooldown = 0.4f;

    private float cooldowntimer = 0f;

    private readonly ProjectilePool<HolyWaterProjectile> ProjectilePool;
    private IWeapon _weaponImplementation;

    public bool CanAttack => cooldowntimer <= 0f;

    public IReadOnlyList<IProjectile> Projectiles => ProjectilePool.Items;

    public HolyWaterWeapon(int poolSize)
    {
        ProjectilePool = new ProjectilePool<HolyWaterProjectile>(poolSize, CreateProjectile);
    }

    private HolyWaterProjectile CreateProjectile()
    {
        HolyWaterProjectile projectile = new HolyWaterProjectile();
        
        projectile.Start();
        return projectile;
    }

    public void Update(GameTime gameTime)
    {
        if (cooldowntimer > 0f)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            cooldowntimer -= deltaTime;
        }

        IReadOnlyList<HolyWaterProjectile> projectiles = ProjectilePool.Items;

        for (int i = 0; i < projectiles.Count; i++)
        {
            if (projectiles[i].IsActive)
            {
                projectiles[i].Update(gameTime);
            }
        }

    }

    public bool TryAttack(Vector2 startingPosition, Vector2 direction)
    {
        if (!CanAttack)
        {
            return false;
        }

        if (direction == Vector2.Zero)
        {
            return false;
        }

        bool foundProjectile = ProjectilePool.TryGet(out HolyWaterProjectile projectile);

        if (!foundProjectile)
        {
            return false;
        }
        
        projectile.Activate(startingPosition, direction);

        cooldowntimer = AttackCooldown;
        
        return true;
    }
}
