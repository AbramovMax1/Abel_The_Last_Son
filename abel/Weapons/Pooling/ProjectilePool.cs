using System;
using System.Collections.Generic;

namespace Abel_The_Last_Son.Weapons.Pooling;


// T stand for Type 
public sealed class ProjectilePool<T>
    where T: class, IProjectile
{
    private readonly List<T> items;
    public IReadOnlyList<T> Items => items;

    public ProjectilePool(int poolSize, Func<T> createProjectile)
    {
        if (poolSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(poolSize),
                "Pool size must be greater then zero!");
        }
        
        items = new List<T>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            T projectile = createProjectile(); // creating one projectile
            items.Add(projectile); // Stores the projectile into the pool
        }
    }

    public bool TryGet(out T projectile)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].IsActive)
            {
                projectile = items[i];
                
                return true;
            }
        }

        projectile = null;
        
        return false;
    }
}