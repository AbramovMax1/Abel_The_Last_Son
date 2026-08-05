using System;
using Abel_The_Last_Son.World.Floor;
using Abel_The_Last_Son.World.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Abel_The_Last_Son.Enemies;
using Abel_The_Last_Son.Manager;
using Abel_The_Last_Son.World.Doors;
using Abel_The_Last_Son.World.Trash;

namespace Abel_The_Last_Son;

public class Game1 : Game
{
    // =========== references =============
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputManager inputManager;

    private bool gameStarted = false; // game run or not

    // ============
    // Texture
    private Texture2D _logo;
    private Texture2D debugPixel;

    // ============
    // screenCenter
    public static Vector2 _screenCenter;

    // ============
    // Fonts
    private SpriteFont _fontOswald;

    // ============
    // Player
    private Player player = null;

    // ============
    // walls
    //private WallLevelFirst wallLevelFirst = null;

    // ============
    // Doors
    private LockedDoor lockedDoor = null;
    
    // ============
    // Floors
    private FloorLevelOne floorOne = null;
    
    // =============
    // NotCollectibles floor trash
    private NotColletiblesPaper notColletiblesPaper = null;
    
    // =============
    // Buttons
    private Buttons startingButton;
    private Buttons SettingsButton;
    private Buttons QuitButton;

    // ==========
    // sorting order sprit
    private List<Sprite> sprites = new List<Sprite>();
    
    // ==========
    // Enemies:
    // Zombie
    private Zombie zombie;
    private readonly List<Zombie> zombies = new List<Zombie>(); //list of zombies
    
  
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // setting the screen resolution for Full HD.
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;

        _graphics.IsFullScreen = false; // make the interface full screen. 

        // center screen positon 
        _screenCenter = new Vector2(
            _graphics.PreferredBackBufferWidth * 0.5f,
            _graphics.PreferredBackBufferHeight * 0.5f
        );

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        inputManager = new InputManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        // ============
        // Texture/Sprite
        // ============

        debugPixel = new Texture2D(GraphicsDevice, 1, 1);
        debugPixel.SetData(new[] { Color.White });
        
        // Floor
        FloorLevelOne(); // wall sprit for level one
        
        //NotCollectible Trash
        TrashPaper();
        
        // doors
        RightDoorLocked();
        
        // Characters
        PlayerSprite(); // player sprite
        PlayerFrontAnimation(); // player front animation
        PlayerBackAnimation(); // player back animation
        PlayerRightAnimation(); // player right animation 

        // ============
        // UI - Buttons
        // ============

        UiButtonsSprite(); // UI buttons sprite 
        
        
        //=============
        // Enemies
        //=============
        ZombieAnimation(); // zombie animation

        Start();
    }

    
    
    void TrashPaper()
    {
        new SpriteManager(Content);
        SpriteManager.AddSprite("TrashPaper", "Images/Paper");
    }
    
    void RightDoorLocked()
    {
        new SpriteManager(Content);
        SpriteManager.AddSprite("RightDoorLocked", "Images/DoorFrameLocked");
    }

    void FloorLevelOne()
    {
        new SpriteManager(Content);
        SpriteManager.AddSprite("FloorOne", "Images/FloorOne");
    }
    

    void PlayerSprite()
    {
        // player
        new SpriteManager(Content);
        SpriteManager.AddSprite("Abel", "Images/AbelPlayerNew");
    }

    void PlayerFrontAnimation()
    {
        SpriteManager.AddSprite("AbelFrontAnimation", "Images/Front-Animation-Sprite", 4, 2);
    }

    void PlayerBackAnimation()
    {
        SpriteManager.AddSprite("AbelBackAnimation", "Images/Back-Animation-Sprite", 4, 2);
    }

    void PlayerRightAnimation()
    {
        SpriteManager.AddSprite("AbelRightAnimation", "Images/Right-Animation-Player", 4, 1);
    }
    void UiButtonsSprite()
    {
        // ======== UI buttons
        // StartButton 
        new SpriteManager(Content);
        SpriteManager.AddSprite("StartButton", "UI/StartButton");
        
        // settingsButton
        new SpriteManager(Content);
        SpriteManager.AddSprite("SettingsButton", "UI/SettingsButton");
        
        // quitButton
        new SpriteManager(Content);
        SpriteManager.AddSprite("QuitButton", "UI/QuitButtons");
    }
    
    void Start()
    {
        // ===== UI ======
        // Buttons
        
        StartGame();
        SettingsBtttonOnClick();
        QuitGame();
        
        // Floor
        floorOne = new FloorLevelOne();
        floorOne.Start();
        
        // Trash
        notColletiblesPaper = new NotColletiblesPaper();
        notColletiblesPaper.Start();
        
        // wall
        //wallLevelFirst = new WallLevelFirst();
        //wallLevelFirst.Start();
        
        // doors
        lockedDoor = new LockedDoor();
        lockedDoor.Start();
        
        // player
        player = new Player();
        player.Start();
        
        // zombie
        zombie = new Zombie(player);
        zombie.Start();
        zombies.Add(zombie); // add the zombie into the list 
        sprites.Add(zombie); // add zombie into the sprite list 
        
        // The list will use sortingOrder to decide what draws first.
        sprites.Add(floorOne);
        sprites.Add(notColletiblesPaper);
        //sprites.Add(wallLevelFirst);
        sprites.Add(lockedDoor);
        sprites.Add(player);
    }

    void StartGame()
    {
        startingButton = new Buttons(
            GraphicsDevice,
            new Rectangle(760, 300, 400, 100)
        );

        startingButton.SetTexture(SpriteManager.GetSprite("StartButton").texture);

        startingButton.OnClick += () =>
        {
            gameStarted = true;
            IsMouseVisible = false;
        };
    }
    
    void SettingsBtttonOnClick()
    {
        SettingsButton = new Buttons(GraphicsDevice,
            new Rectangle(760, 430, 400, 100));
        
        SettingsButton.SetTexture(SpriteManager.GetSprite("SettingsButton").texture);

        SettingsButton.OnClick += () =>
        {
            Console.WriteLine("Settings");
        };
        
    }
    
    void QuitGame()
    {
        QuitButton = new Buttons(GraphicsDevice, new Rectangle(760, 560, 400, 100));
        QuitButton.SetTexture(SpriteManager.GetSprite("QuitButton").texture);
        QuitButton.OnClick += () =>
        {
            Exit();
        };
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        if (gameStarted)
        {
            player.Update(gameTime);

            foreach (Zombie enemy in zombies)
            {
                enemy.Update(gameTime);
            }
            CheckEnemyPlayerCollisions();
        }
        else
        {
            startingButton.Update();
            SettingsButton.Update();
            QuitButton.Update();
        }
        
        // ======== Esc button ======== (quit the game)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        inputManager.FullscreenFlip(_graphics);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // starting 
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // make monogame not blur and make my pixel art ugly
        
        // ============
        // Texture/Sprite
        // ============
        foreach (Sprite sprite in sprites.OrderBy(sprite => sprite.sortingOrder))
        {
            sprite.DrawSprite(_spriteBatch);
        }
        
        // ============
        // UI - Buttons 
        // ============
        if (!gameStarted)
        {
            startingButton.Draw(_spriteBatch);
            SettingsButton.Draw(_spriteBatch);
            QuitButton.Draw(_spriteBatch);
        }
        
        // ==========
        // Draw Colliders
        // ==========
        if (gameStarted) // if the game is started draw the collider.
        {
            DrawCollider(player.Collider, Color.LimeGreen);

            foreach (Zombie enemy in zombies)
            {
                if (!enemy.IsDead)
                {
                    DrawCollider(enemy.Collider, Color.Red);
                }
            }
        }
        
        // ending 
        _spriteBatch.End();
        
        
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    private void DrawCollider(Rectangle rectangle, Color color)
    {
        int thickness = 3; // this is the thickness of the collider
        
        // top collider
        _spriteBatch.Draw(
            debugPixel,
            new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness),
            color);
        
        // Bottom collider
        _spriteBatch.Draw(
            debugPixel,
            new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness),
            color);
        
        // Left collider
        _spriteBatch.Draw(debugPixel,
            new Rectangle(rectangle.X ,rectangle.Y ,thickness, rectangle.Height),
                color);
        
        // Right Collider
        _spriteBatch.Draw(
            debugPixel,
            new Rectangle(rectangle.Right -  thickness, rectangle.Y, thickness, rectangle.Height),
            color);
        
    }

    private void ZombieAnimation()
    {
        SpriteManager.AddSprite("ZombieFrontAnimation", "Images/Front-Animation-Zombie", 4, 2);
        SpriteManager.AddSprite("ZombieBackAnimation", "Images/Back-Animation-Zombie", 4, 2);
        SpriteManager.AddSprite("ZombieLeftAnimation", "Images/Left-Aniamtion-Zombie", 4, 1);
        SpriteManager.AddSprite("ZombieRightAnimation", "Images/Right-Animation-Zombie", 4, 1);
    }

    private void CheckEnemyPlayerCollisions()
    {
        foreach (Zombie enemy in zombies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            bool isTouchingPlayer = enemy.Collider.Intersects(player.Collider);

            if (isTouchingPlayer)
            {
                player.TakeDamage(enemy.ContactDamage);
                Console.WriteLine($"Player health: {player.Health}");
            }
        }
    }
}