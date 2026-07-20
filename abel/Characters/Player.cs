using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Abel_The_Last_Son;

public class Player : Sprite
{
    // ================================
    // Player settings
    // ================================
    private float playerSpeedMovement = 300f;
    private float playerSpeedRotation = 0f;

    public Player() : base("Abel")
    {
    }

    public override void Start()
    {
        base.Start();
        transform.position = Game1._screenCenter; // center the player on the middle screen
        transform.scale = new Vector2(4f, 4f); // player scale
        sortingOrder = 4;
    }

    public override void Update(GameTime gameTime)
    {
        PlayerMovement(gameTime);
    }

    public void PlayerMovement(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (Keyboard.GetState().IsKeyDown(Keys.D)) // Pressing D going left 
        {
            spriteEffects = SpriteEffects.None;
            transform.position += new Vector2(playerSpeedMovement * deltaTime, 0);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.A)) // Pressing A going left 
        {
            spriteEffects = SpriteEffects.FlipHorizontally; // flip the sprite
            transform.position += new Vector2(-playerSpeedMovement * deltaTime, 0);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.S)) // Pressing S going left 
        {
            transform.position += new Vector2(0, playerSpeedMovement * deltaTime);
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.W)) // Pressing W going left 
        {
            transform.position += new Vector2(0, -playerSpeedMovement * deltaTime);
        }
    }
}