using System;
using System.Collections.Generic;
using Abel_The_Last_Son.Core.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Abel_The_Last_Son.Enemies;

public class Zombie : Sprite , IEnemy
{
    private readonly Player target;
    
    // Zombie Animation
    private SpriteSheet frontAniamtion;
    private SpriteSheet backAniamtion;
    private SpriteSheet leftAniamtion;
    private SpriteSheet rightAniamtion;
    private int currentFrame = 0;
    private float animationTimer = 0f;
    private const float AnimationSpeed = 0.12f;
    private const float MovementSpeed = 110f;
    private Direction facingDirection = Direction.Down;
    private float deathTimer = 0f;
    private float roomEntryDelayTimer = 0f;
    private const float DeathDuration = 0.8f;
    private const float FadeStartTime = 0.2f;
    
    public bool ReadyToRemove { get; private set; } = false;
    
    private SoundEffectInstance zombieSoundInstance;
    
    public event Action OnDeath;
    
    // Zombie stats
    public int MaxHealth { get; } = 3;
    public int Health { get; private set; } = 3;
    public bool IsDead => Health <= 0;
    public bool CanDealContactDamage => roomEntryDelayTimer <= 0f && !IsDead;
    public int ContactDamage { get; } = 1;
    
    // Making collider for our zombie
    public Rectangle Collider
    {
        get
        {
            int width = 70;
            int height = 100;
            
            // center collider on the zombie
            int x = (int)transform.position.X - width / 2;
            int y = (int)transform.position.Y - height / 2 + 10;
            return new Rectangle(x, y, width, height);
        }
    }
    
    public List<Rectangle> CurrentWalls { get; set; } = new List<Rectangle>();

    public Zombie(Player player) : base("ZombieFrontAnimation")
    {
        target = player;
    }

    public override void Start()
    {
        base.Start();
        
        zombieSoundInstance = Game1.zombieSound.CreateInstance();
        zombieSoundInstance.IsLooped = true;
        zombieSoundInstance.Volume = 0.3f;
        
        frontAniamtion = SpriteManager.GetSprite("ZombieFrontAnimation");
        backAniamtion = SpriteManager.GetSprite("ZombieBackAnimation");
        leftAniamtion = SpriteManager.GetSprite("ZombieLeftAnimation");
        rightAniamtion = SpriteManager.GetSprite("ZombieRightAnimation");
        
        // spawn Zombie 
        transform.position = new Vector2(400f, 400f);
        transform.scale = new Vector2(4f, 4f);

        sortingOrder = 4;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            UpdateDeathEffect(gameTime);
            
            return;
        }
        
        if (zombieSoundInstance != null && zombieSoundInstance.State != SoundState.Playing)
        {
            zombieSoundInstance.Play();
        }

        if (roomEntryDelayTimer > 0f)
        {
            roomEntryDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return;
        }

        FollowPlayer(gameTime); // follow our player
        Animate(gameTime); // play the walk animation for the zombie
    }

    public void StartRoomEntryDelay(float seconds)
    {
        roomEntryDelayTimer = MathF.Max(0f, seconds);
        animationTimer = 0f;
        currentFrame = 0;
        SetFrame(0, 0);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
        {
            return;
        }
        
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            StartDeathEffect();
            zombieSoundInstance?.Stop();
            OnDeath?.Invoke();
        }

        if (IsDead)
        {
            Console.WriteLine("Zombie Dead");
        }
    }

    private void FollowPlayer(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Vector2 direction = target.transform.position - transform.position;
        
        float distance = direction.Length();

        if (distance <= 5f)
        {
            return;
        }
        
        direction.Normalize();
        
        transform.position.X += direction.X * MovementSpeed * deltaTime;
        
        foreach (var wall in CurrentWalls)
        {
            if (Collider.Intersects(wall))
            {
                // If we hit a wall, undo the X movement!
                transform.position.X -= direction.X * MovementSpeed * deltaTime;
                break; // No need to check other walls
            }
        }
        
        transform.position.Y += direction.Y * MovementSpeed * deltaTime;
        
        foreach (var wall in CurrentWalls)
        {
            if (Collider.Intersects(wall))
            {
                // If we hit a wall, undo the X movement!
                transform.position.Y -= direction.Y * MovementSpeed * deltaTime;
                break; // No need to check other walls
            }
        }
        
        ChooseAnimation(direction);
    }

    private void ChooseAnimation(Vector2 movement)
    {
        if (MathF.Abs(movement.X) > MathF.Abs(movement.Y))
        {
            if (movement.X > 0f)
            {
                facingDirection = Direction.Right;
                ChangeAnimation(rightAniamtion);
            }
            else
            {
                facingDirection = Direction.Left;
                ChangeAnimation(leftAniamtion);
            }
        }
        else
        {
            if (movement.Y > 0f)
            {
                facingDirection = Direction.Down;
                ChangeAnimation(frontAniamtion);
                
            }
            else
            {
                facingDirection = Direction.Up;
                ChangeAnimation(backAniamtion);
            }
        }
    }

    private void Animate(GameTime gameTime)
    {
        animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (animationTimer < AnimationSpeed)
        {
            return;
        }
        
        animationTimer = 0f;
        
        currentFrame++;
        
        int totalFrames = spriteSheet.columns *spriteSheet.rows;

        if (currentFrame >= totalFrames)
        {
            currentFrame = 0;
        }
        
        int column = currentFrame % spriteSheet.columns;
        int row = currentFrame / spriteSheet.columns;
        
        SetFrame(column, row);
    }

    private void ChangeAnimation(SpriteSheet newAnimation)
    {
        if (spriteSheet == newAnimation)
        {
            return;
        }    
        spriteSheet = newAnimation;
        texture = spriteSheet.texture;
        currentFrame = 0;
        animationTimer = 0f;
        SetFrame(0,0);
    }

    public void StartDeathEffect()
    {
        deathTimer = 0f;

        ReadyToRemove = false;

        color = Color.White;
    }

    private void UpdateDeathEffect(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        deathTimer += deltaTime;

        float redProgress = MathHelper.Clamp(deathTimer / FadeStartTime, 0f, 1f);
        
        float fadeProgress = MathHelper.Clamp((deathTimer - FadeStartTime) / (DeathDuration - FadeStartTime), 0f, 1f);
        
        float opacity = 1f - fadeProgress;
        
        Color redTint = Color.Lerp(
            Color.White,
            new Color(255, 60, 60),
            redProgress);
        
        color = redTint * opacity;

        if (deathTimer >= DeathDuration)
        {
            zombieSoundInstance?.Dispose();
            ReadyToRemove = true;
        }
    }
    public void StopSound()
    {
        zombieSoundInstance?.Stop();
    }
}