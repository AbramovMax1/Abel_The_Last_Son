using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class SpriteManager
{
    private static Dictionary<string, SpriteSheet> sprites = new Dictionary<string, SpriteSheet>();
    
    private static ContentManager _content;

    public SpriteManager(ContentManager content)
    {
        _content = content;
    }

    public static void AddSprite(string spriteName, string fileName, int columns = 1, int rows = 1)
    {
        sprites[spriteName] = new SpriteSheet();
        sprites[spriteName].texture = _content.Load<Texture2D>(fileName);
        sprites[spriteName].columns = columns;
        sprites[spriteName].rows = rows;
    }

    public static SpriteSheet GetSprite(string spriteName)
    {
        if (sprites.ContainsKey(spriteName)) return sprites[spriteName];
        
        return null;
    }
}