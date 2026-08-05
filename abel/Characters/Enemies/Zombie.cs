using System;
using Abel_The_Last_Son.Core.Enums;
using Microsoft.Xna.Framework;

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
    private Direction facingDirection = Direction.Front;
    
    // Zombie stats
    public int MaxHealth { get; } = 3;
    public int Health { get; private set; } = 3;
    public bool IsDead => Health <= 0;
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

    public Zombie(Player player) : base("ZombieFrontAnimation")
    {
        target = player;
    }

    public override void Start()
    {
        base.Start();
        
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
            return;
        }
        
        FollowPlayer(gameTime); // follow our player
        Animate(gameTime); // play the walk animation for the zombie
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
        
        transform.position += direction * MovementSpeed * deltaTime;
        
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
                facingDirection = Direction.Front;
                ChangeAnimation(frontAniamtion);
                
            }
            else
            {
                facingDirection = Direction.Back;
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
    
}