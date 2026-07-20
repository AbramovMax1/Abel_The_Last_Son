using Microsoft.Xna.Framework;

namespace Abel_The_Last_Son;

public interface IUpdateable
{
    void Start();
    
    void Update(GameTime gameTime);
}