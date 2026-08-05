using System;
using System.Runtime.Intrinsics.X86;
using Abel_The_Last_Son.Core.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Abel_The_Last_Son;

public class Player : Sprite , ICollidable, IDamageable
{
    // ================================
    // Player settings
    // ================================
    private float playerSpeedMovement = 300f;
    private float playerSpeedRotation = 0f;
    private int currentFrame = 0;
    private float animationTimer = 0f;
    private float animationSpeed = 0.12f;
    private bool isMoving = false; // false mean abel is standing still.
    private KeyboardState previousKeyboardState;
    
    private float damageCooldownTimer = 0f;
    private const float DamageProtectionTime = 1f;
    public bool CanTakeDamage => damageCooldownTimer <= 0f;
    
    // Player Stats 

    public int MaxHealth { get; } = 3; // Max health of the player is 3 hearts 
    public int Health { get; private set; } = 3; 
    public bool IsDead => Health <= 0;
    
    // ANIMATION 
    private SpriteSheet frontAnimation;
    private SpriteSheet backAnimation;
    private SpriteSheet rightAnimation;
    private SpriteSheet leftAnimation;


    public Player() : base("AbelFrontAnimation")
    {
    }

    // Other objects may read the direction (get is public) but only player can changge it (set private)
    public Direction FacingDirection { get; private set; } = Direction.Front;

    
    public override void Start()
    {
        base.Start();
        
        frontAnimation = SpriteManager.GetSprite("AbelFrontAnimation");
        backAnimation = SpriteManager.GetSprite("AbelBackAnimation");
        rightAnimation = SpriteManager.GetSprite("AbelRightAnimation");
        
        transform.position = Game1._screenCenter; // center the player on the middle screen
        transform.scale = new Vector2(4f, 4f); // player scale
        sortingOrder = 4;
    }

    public override void Update(GameTime gameTime)
    {
        if (damageCooldownTimer > 0f)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            damageCooldownTimer -= deltaTime;
        }
        
        PlayerMovement(gameTime);
        PlayerAnimation(gameTime);
    }

public void PlayerMovement(GameTime gameTime)
{
    // Find how many seconds passed since the previous update.
    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

    // Take a new picture of the keyboard.
    KeyboardState keyboard = Keyboard.GetState();

    // Start with no movement.
    Vector2 movement = Vector2.Zero;

    // Start by assuming Abel is standing still.
    isMoving = false;

    // ==========================================
    // MOVEMENT
    // These checks allow diagonal movement.
    // ==========================================

    // Move upward while W is held.
    if (keyboard.IsKeyDown(Keys.W))
    {
        movement.Y -= 1f;
    }

    // Move downward while S is held.
    if (keyboard.IsKeyDown(Keys.S))
    {
        movement.Y += 1f;
    }

    // Move right while D is held.
    if (keyboard.IsKeyDown(Keys.D))
    {
        movement.X += 1f;
    }

    // Move left while A is held.
    if (keyboard.IsKeyDown(Keys.A))
    {
        movement.X -= 1f;
    }

    // ==========================================
    // NEW KEY PRESSES
    // A key is "new" when it is down now,
    // but it was up during the previous update.
    // ==========================================

    // Check whether W was just pressed.
    bool pressedW =
        keyboard.IsKeyDown(Keys.W) &&
        previousKeyboardState.IsKeyUp(Keys.W);

    // Check whether A was just pressed.
    bool pressedA =
        keyboard.IsKeyDown(Keys.A) &&
        previousKeyboardState.IsKeyUp(Keys.A);

    // Check whether D was just pressed.
    bool pressedD =
        keyboard.IsKeyDown(Keys.D) &&
        previousKeyboardState.IsKeyUp(Keys.D);

    // Check whether S was just pressed.
    bool pressedS =
        keyboard.IsKeyDown(Keys.S) &&
        previousKeyboardState.IsKeyUp(Keys.S);

    
    
    // ==========================================
    // ANIMATION SELECTION
    // The newly pressed key chooses the animation.
    // ==========================================

    // A was newly pressed, so face left.
    if (pressedA)
    {
        // Remember the new facing direction.
        FacingDirection = Direction.Left;

        // Left uses the right sheet reflected in a mirror.
        ChangeAnimation(rightAnimation);

        // Flip the picture so right becomes left.
        spriteEffects = SpriteEffects.FlipHorizontally;
    }

    // D was newly pressed, so face right.
    if (pressedD)
    {
        // Remember the new facing direction.
        FacingDirection = Direction.Right;

        // Use the right-facing animation.
        ChangeAnimation(rightAnimation);

        // Remove the mirror effect.
        spriteEffects = SpriteEffects.None;
    }

    // W was newly pressed, so face backward.
    if (pressedW)
    {
        // Remember the new facing direction.
        FacingDirection = Direction.Back;

        // Use the back-facing animation.
        ChangeAnimation(backAnimation);

        // The back animation does not need reflection.
        spriteEffects = SpriteEffects.None;
    }

    // S is checked after A.
    // Therefore, if A and S somehow become new in the same update,
    // S receives animation priority.
    if (pressedS)
    {
        // Remember the new facing direction.
        FacingDirection = Direction.Front;

        // Use the front-facing animation.
        ChangeAnimation(frontAnimation);

        // The front animation does not need reflection.
        spriteEffects = SpriteEffects.None;
    }

    // ==========================================
    // RETURN CONTROL TO A KEY THAT IS STILL HELD
    // ==========================================

    // Check whether the key that selected the current animation
    // is still being held by the player.
    bool currentDirectionIsStillHeld =
        (FacingDirection == Direction.Back && keyboard.IsKeyDown(Keys.W)) ||
        (FacingDirection == Direction.Front && keyboard.IsKeyDown(Keys.S)) ||
        (FacingDirection == Direction.Right && keyboard.IsKeyDown(Keys.D)) ||
        (FacingDirection == Direction.Left && keyboard.IsKeyDown(Keys.A));

    // If the newest direction key was released, choose another
    // direction key that is still held.
    if (!currentDirectionIsStillHeld)
    {
        // W is still held, so go back to the back-facing animation.
        if (keyboard.IsKeyDown(Keys.W))
        {
            FacingDirection = Direction.Back;
            ChangeAnimation(backAnimation);
            spriteEffects = SpriteEffects.None;
        }
        // S is still held, so go back to the front-facing animation.
        else if (keyboard.IsKeyDown(Keys.S))
        {
            FacingDirection = Direction.Front;
            ChangeAnimation(frontAnimation);
            spriteEffects = SpriteEffects.None;
        }
        // D is still held, so go back to the right-facing animation.
        else if (keyboard.IsKeyDown(Keys.D))
        {
            FacingDirection = Direction.Right;
            ChangeAnimation(rightAnimation);
            spriteEffects = SpriteEffects.None;
        }
        // A is still held, so use the mirrored right-facing animation.
        else if (keyboard.IsKeyDown(Keys.A))
        {
            FacingDirection = Direction.Left;
            ChangeAnimation(rightAnimation);
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    // ==========================================
    // APPLY MOVEMENT
    // ==========================================

    // Move only when the movement vector is not empty.
    if (movement != Vector2.Zero)
    {
        // Remember that Abel is walking.
        isMoving = true;

        // Prevent diagonal movement from being faster.
        movement.Normalize();

        // Apply direction, speed, and elapsed time.
        transform.position += movement * playerSpeedMovement * deltaTime;
    }

    // Save today's keyboard picture.
    // During the next update, it becomes the old picture.
    previousKeyboardState = keyboard;
}

    private void PlayerAnimation(GameTime gameTime)
    {
        if (!isMoving)
        {
            currentFrame = 0;  // Select the first frame.
            animationTimer = 0f; // Reset the animation clock.

            // Display the first frame from the first row.
            SetFrame(0, 0);

            // Leave this method because no animation is needed.
            return;
        }
        
        animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f; // Start counting time again for the next frame.
            currentFrame++;
            
            int totalFrames = spriteSheet.columns * spriteSheet.rows;

            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
            
            int column = currentFrame % spriteSheet.columns;
            int row = currentFrame / spriteSheet.columns; // Divide by the number of columns 
            
            SetFrame(column, row);
        }
        
    }
    
    private void ChangeAnimation(SpriteSheet newAnimation)
    {
        if (spriteSheet == newAnimation)
        {
            return;
        }
        
        // Change the active animation sheet.
        spriteSheet = newAnimation;

        // Change the texture drawn by the Sprite class.
        texture = spriteSheet.texture;

        // Restart from the first animation frame.
        currentFrame = 0;

        // Restart the animation clock.
        animationTimer = 0f;

        // Display the first frame immediately.
        SetFrame(0, 0);
    }

    public Rectangle Collider
    {
        get
        {
            int width = 70; // collider width
            int height = 120; // collider height
            
            // This x and y helping to center our collider.
            int x = (int)transform.position.X - width / 2;
            int y = (int)transform.position.Y - height / 2 + 5;
            return new Rectangle(x, y, width, height);
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
        {
            return;
        }

        if (!CanTakeDamage)
        {
            return;
        }
        
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
        }

        damageCooldownTimer = DamageProtectionTime; // when take damage start the cooldown protection.
        
        if (IsDead)
        {
            Console.WriteLine("Player is dead");
        }
    }
}
