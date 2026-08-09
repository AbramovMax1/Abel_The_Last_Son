using System;
using Abel_The_Last_Son.World.Floor;
using Abel_The_Last_Son.World.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Abel_The_Last_Son.Core.Helpers;
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

    //  =========== Floor & Rooms =========== 
    private Room activeRoom = null;
    private Floor currentFloor;
    
    
    private Camera camera;

    private bool gameStarted = false; // game run or not

    private bool gameOver = false;

    // ============
    // KeyBinds
    private KeyboardState previousCombatKeyboard;
    
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
    
    // =========
    // HeartUI
    private Texture2D heartTexture;
  
    
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
        new SpriteManager(Content);
        
        

        // TODO: use this.Content to load your game content here
        
        // Load Camera
        camera = new Camera(GraphicsDevice.Viewport);

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
        SpriteManager.AddSprite("DoorOneLocked", "Images/DoorFrameLockedFloorOne-export");
        SpriteManager.AddSprite("DoorTwoLocked", "Images/DoorFrameLockedFlootTwo");
        
        
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

        //=============
        // Weapons
        //=============
        HolyWaterSprite();
        

        
        Start();
    }

    private void HolyWaterSprite()
    {
        SpriteManager.AddSprite("HolyWater", "Images/HollyWater");
    }
    
    void TrashPaper()
    {
        SpriteManager.AddSprite("TrashPaper", "Images/Paper");
    }
    
    void RightDoorLocked()
    {
        SpriteManager.AddSprite("RightDoorLocked", "Images/DoorFrameLocked");
    }

    void FloorLevelOne()
    {
        SpriteManager.AddSprite("FloorOne", "Images/FloorOne");
    }
    

    void PlayerSprite()
    {
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

    void HeartUI()
    {
        SpriteManager.AddSprite("HeartUI", "UI/Heart");
        heartTexture =
            SpriteManager.GetSprite("HeartUI").texture;
    }
    
    void Start()
    {
        // ===== UI ======
        // Buttons
        
        StartGame();
        SettingsBtttonOnClick();
        QuitGame();
        
        //heart
        HeartUI();
        //=============
        
        // Floor
        floorOne = new FloorLevelOne();
        floorOne.Start();
        
        // Trash
        notColletiblesPaper = new NotColletiblesPaper();
        notColletiblesPaper.Start();
        
        // doors
        lockedDoor = new LockedDoor();
        lockedDoor.Start();
        
        // player
        player = new Player();
        player.Start();

        player.Died += HandlePlayerDeath;
        
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

            currentFloor = new Floor();
            currentFloor.generateFloor(Floor.Difficulty.Easy, player);
            
            // Loop through all the rooms the floor just generated
            foreach (Sprite sprite in currentFloor.GetRoomSprites())
            {
                // 1. ADD THE ROOM TO THE DRAW LIST!
                sprites.Add(sprite);

                // 2. Find the starting room
                if (sprite is Room room)
                {
                    if (room.isStartRoom) activeRoom = room;
                    // Note: We removed the "break;" here so the loop 
                    // continues and adds ALL the rooms to the sprites list!
                    foreach (Zombie enemy in room.enemyList)
                    {
                        sprites.Add(enemy);
                    }
                }
                
            }

            if (activeRoom != null)
            {
                // Center player in the starting room
                player.transform.position = activeRoom.transform.position;
                
                // Snap the camera directly to the starting room 
                camera.Position = activeRoom.transform.position;
            }
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
        
        if (gameStarted && !gameOver)
        {
            // 1. Pass the active room's walls to the player so they can slide against them
            if (activeRoom != null)
            {
                activeRoom.currentPlayer = player;
                player.CurrentWalls = activeRoom.WallColliders;

                foreach (var enemy in activeRoom.enemyList)
                {
                    enemy.CurrentWalls = activeRoom.WallColliders;
                    if (!enemy.IsDead)
                    {
                        enemy.Update(gameTime); // You were missing this for room zombies!
                    }
                }
            }

            // 2. Check for door transitions
            CheckRoomTransitions();
                
            player.Update(gameTime);
            
            HandlePlayerAttack();
            
            foreach (Zombie enemy in zombies)
            {
                enemy.Update(gameTime);
            }
            
            CheckProjectileEnemyCollisions();
            
            RemoveDeadZombies();
            
            CheckEnemyPlayerCollisions();
        }
        else if (!gameStarted)
        {
            startingButton.Update();
            SettingsButton.Update();
            QuitButton.Update();
        }
        
        // ======== Esc button ======== (quit the game)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        
        inputManager.FullscreenFlip(_graphics);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // ==========================================
        // 1. DRAW WORLD (Moves with the Camera)
        // ==========================================
        
        // Notice we added 'transformMatrix: camera.Transform' here!
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform); 
        
        // Texture/Sprite
        foreach (Sprite sprite in sprites.OrderBy(sprite => sprite.sortingOrder))
        {
            sprite.DrawSprite(_spriteBatch);
        }
        
        // Draw World Colliders and Projectiles
        if (gameStarted) 
        {
            
            DrawCollider(player.Collider, Color.LimeGreen);

            foreach (Zombie enemy in zombies)
            {
                if (!enemy.IsDead)
                {
                    DrawCollider(enemy.Collider, Color.Red);
                }
            }
            
            if (activeRoom != null)
            {
                foreach (Zombie enemy in activeRoom.enemyList)
                {
                    if (!enemy.IsDead) DrawCollider(enemy.Collider, Color.Red);
                }
            }
            
            DrawWeaponProjectiles();
            
            IReadOnlyList<IProjectile> projectiles = player.Weapon.Projectiles;
            for (int i = 0; i < projectiles.Count; i++)
            {
                DrawCollider(projectiles[i].Collider, Color.Cyan);
            }
        }
        
        _spriteBatch.End();


        // ==========================================
        // 2. DRAW UI (Stays fixed to the screen)
        // ==========================================
        
        // No camera transform here, so UI stays glued to the monitor
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if (!gameStarted)
        {
            startingButton.Draw(_spriteBatch);
            SettingsButton.Draw(_spriteBatch);
            QuitButton.Draw(_spriteBatch);
        }
        else
        {
            DrawPlayerHealth();
        }
        
        _spriteBatch.End();

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
    
    private void CheckRoomTransitions()
    {
        if (activeRoom == null) return;

        // Loop through all 4 doors of the current room
        for (int i = 0; i < 4; i++)
        {
            Door door = activeRoom.doors[i];
            
            // Check if door exists, is open (or unlocked), and touches player
            // You can add `&& door.open` if you want to ensure locked doors don't trigger!
            if (door != null && door.Collider.Intersects(player.Collider)) 
            {
                int nextCol = activeRoom.collumn;
                int nextRow = activeRoom.row;
                int oppositeDoorIndex = 0;
                Vector2 spawnOffset = Vector2.Zero;

                // Figure out which way we are going on the grid based on the door hit
                switch (i)
                {
                    case 0: // Went UP
                        nextRow += 1;
                        oppositeDoorIndex = 2; // Arrive at DOWN door
                        spawnOffset = new Vector2(0, -250); // Push slightly UP into the room
                        break;
                    case 1: // Went RIGHT
                        nextCol += 1;
                        oppositeDoorIndex = 3; // Arrive at LEFT door
                        spawnOffset = new Vector2(250, 0); // Push slightly RIGHT into the room
                        break;
                    case 2: // Went DOWN
                        nextRow -= 1;
                        oppositeDoorIndex = 0; // Arrive at UP door
                        spawnOffset = new Vector2(0, 250); // Push slightly DOWN into the room
                        break;
                    case 3: // Went LEFT
                        nextCol -= 1;
                        oppositeDoorIndex = 1; // Arrive at RIGHT door
                        spawnOffset = new Vector2(-250, 0); // Push slightly LEFT into the room
                        break;
                }

                // Get the next room from the floor array
                Room nextRoom = currentFloor.GetRoomAt(nextCol, nextRow);

                if (nextRoom != null)
                {
                    // Update active room
                    activeRoom = nextRoom;

                    // Move player to the new door and apply the offset so they aren't stuck inside the door trigger!
                    Door arrivalDoor = activeRoom.doors[oppositeDoorIndex];
                    if (arrivalDoor != null)
                    {
                        player.transform.position = arrivalDoor.transform.position + spawnOffset;
                    }

                    // Move the Camera to center on the new room!
                    camera.Position = activeRoom.transform.position;
                    break;
                }
            }
        }
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
        if (activeRoom != null)
        {
            foreach (Zombie enemy in activeRoom.enemyList)
            {
                if (enemy.IsDead) continue;
                if (enemy.Collider.Intersects(player.Collider))
                {
                    player.TakeDamage(enemy.ContactDamage);
                    Console.WriteLine($"Player health: {player.Health}");
                }
            }
        }
    }

    private void HandlePlayerAttack()
    {
        KeyboardState keyboard =  Keyboard.GetState();
        
        bool spaceWasPressed = keyboard.IsKeyDown(Keys.Space) && previousCombatKeyboard.IsKeyUp(Keys.Space);

        if (spaceWasPressed)
        {
            Vector2 shootingDirection = DirectionHelper.ToVector(player.FacingDirection);

            bool shotWasCreated = player.Weapon.TryAttack(player.transform.position, shootingDirection);

            if (!shotWasCreated)
            {
                Console.WriteLine("Holy water could not fire");
            }
        }
        previousCombatKeyboard = keyboard;
    }

    private void DrawWeaponProjectiles()
    {
        IReadOnlyList<IProjectile> projectiles = player.Weapon.Projectiles;

        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].IsActive)
            {
                continue;
            }
            projectiles[i].DrawSprite(_spriteBatch);
        }
    }

    private void CheckProjectileEnemyCollisions()
    {
        IReadOnlyList<IProjectile> projectiles = player.Weapon.Projectiles;

        for (int projectileIndex = 0; projectileIndex < projectiles.Count; projectileIndex++)
        {
            IProjectile projectile = projectiles[projectileIndex];

            if (!projectile.IsActive)
            {
                continue;
            }

            for (int enemyIndex = 0; enemyIndex < zombies.Count; enemyIndex++)
            {
                Zombie enemy = zombies[enemyIndex];

                if (enemy.IsDead)
                {
                    continue;
                }
                
                bool hitEnemy = projectile.Collider.Intersects(enemy.Collider);

                if (hitEnemy)
                {
                    enemy.TakeDamage(projectile.Damage);
                    projectile.Destroy();
                    break;
                }
            }
            
            if (activeRoom != null)
            {
                foreach (Zombie enemy in activeRoom.enemyList)
                {
                    if (enemy.IsDead) continue;
                
                    if (projectile.Collider.Intersects(enemy.Collider))
                    {
                        enemy.TakeDamage(projectile.Damage);
                        projectile.Destroy();
                        break;
                    }
                }
            }
        }
    }

    private void RemoveDeadZombies()
    {
        int lastIndex = zombies.Count - 1;

        for (int i = lastIndex; i >= 0; i--)
        {
            if (zombies[i].IsDead)
            {
                sprites.Remove(zombies[i]);
                
                zombies.RemoveAt(i);
            }
        }
        if (activeRoom == null) return;

        // Loop backwards through the active room's enemy list
        lastIndex = activeRoom.enemyList.Count - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            if (activeRoom.enemyList[i].IsDead)
            {
                // 1. Remove the zombie from the drawing list so it disappears
                sprites.Remove(activeRoom.enemyList[i]);
            
                // 2. Remove the zombie from the room's active logic list
                activeRoom.enemyList.RemoveAt(i);
            }
        }
    }

    private void DrawPlayerHealth()
    {
        for (int i = 0; i < player.Health; i++)
        {
            int x = 30 + i * 70;
            int y = 30;

            int size = 64;
            
            Rectangle heartRectangle = new Rectangle(x, y, size, size);
            
            _spriteBatch.Draw(heartTexture, heartRectangle, Color.White);
        }

        
    }

    private void HandlePlayerDeath()
    {
        gameOver = true;
        IsMouseVisible = true;
        Console.WriteLine("Game Over");
    }
}