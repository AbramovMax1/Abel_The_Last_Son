using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son.Manager;


public class SceneManager : IUpdateable, IDrawable, ICollidable
{
    private static List<IUpdateable> _updatables = new();
    private static List<IDrawable> _drawables = new();
    private static List<ICollidable> _colliders = new();

    private static SceneManager instance = null;

    public static T Add<T>(T obj)
    {
        if (obj is IUpdateable updatable)
        {
            _updatables.Add(updatable);
        }
        if (obj is IDrawable drawable)
        {
            _drawables.Add(drawable);
        }
        if (obj is ICollidable collider)
        {
            _colliders.Add(collider);
        }

        return obj;
    }
    public static T Create<T>()  where T : new()
    {
        T obj = new T();
        
        if (obj is IUpdateable updatable)
        {
            _updatables.Add(updatable);
        }
        if (obj is IDrawable drawable)
        {
            _drawables.Add(drawable);
        }
        
        if (obj is ICollidable collider)
        {
            _colliders.Add(collider);
        }
        
        return obj;
    }

    public static void Remove<T>(T obj)
    {
        if (obj is IUpdateable updatable)
        {
            _updatables.Remove(updatable);
        }
        if (obj is IDrawable drawable)
        {
            _drawables.Remove(drawable);
        }
        if (obj is ICollidable collider)
        {
            _colliders.Remove(collider);
        }
    }

    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneManager();
            }

            return instance;
        }
    }

    public void Start()
    {
        _updatables.ForEach(updatable => updatable.Start());
    }

    public void Update(GameTime gameTime)
    {
        _updatables.ForEach(updatable => updatable.Update(gameTime));
        
        HandleCollisions();
    }

    public void HandleCollisions()
    {
        
    }

    public void DrawSprite(SpriteBatch spriteBatch)
    {
        _drawables.ForEach(drawable => drawable.DrawSprite(spriteBatch));
    }

    public Rectangle Collider
    {
        get;
    }
}