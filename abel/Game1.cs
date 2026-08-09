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
using Abel_The_Last_Son.Items;

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

    private const float RoomEntryZombieDelay = 0.8f;

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
    private Buttons retryButton;
    private Buttons backButton;
    
    // =============
    // UI
    private Texture2D mainMenuBackgroundTexture;
    
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
  
    //=========
    // Items
    private Texture2D keyTexture;
    private readonly List<DoorKey> DoorKeys = new List<DoorKey>();
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
        SpriteManager.AddSprite("Rock", "Images/Rock", 1,1);
        
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
        // UI 
        // ============
        MainMenuBackground(); // backgornd for the main menu 
        UiButtonsSprite(); // UI buttons sprite 
        
        
        //=============
        // Enemies
        //=============
        ZombieAnimation(); // zombie animation

        //=============
        // Weapons
        //=============
        HolyWaterSprite();
        
        //=============
        // Items
        //=============
        DoorKeySprite();
        
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

    void MainMenuBackground()
    {
        mainMenuBackgroundTexture = Content.Load<Texture2D>("UI/MainMenuBackground");
    }
    
    void UiButtonsSprite()
    {
        // ======== UI buttons
        // StartButton 
        SpriteManager.AddSprite("StartButton", "UI/StartButton");
        
        // settingsButton
        SpriteManager.AddSprite("SettingsButton", "UI/SettingsButton");
        
        // quitButton
        SpriteManager.AddSprite("QuitButton", "UI/QuitButton");
        SpriteManager.AddSprite("RetryButton", "UI/RetryButton");
        SpriteManager.AddSprite("BackButton", "UI/BackButton");
    }

    void HeartUI()
    {
        SpriteManager.AddSprite("HeartUI", "UI/Heart");
        heartTexture =
            SpriteManager.GetSprite("HeartUI").texture;
    }
    
    private void DoorKeySprite()
    {
       
        SpriteManager.AddSprite("DoorKey", "Items/KeyDoor");

        
        keyTexture = SpriteManager.GetSprite("DoorKey").texture;
    }
    
    void Start()
    {
        // ===== UI ======
        // Buttons
        
        StartGame();
        SettingsBtttonOnClick();
        QuitGame();
        CreateDeathMenuButtons();
        
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

        startingButton.OnClick += BeginNewRun;
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

    private void CreateDeathMenuButtons()
    {
        retryButton = new Buttons(GraphicsDevice, new Rectangle(760, 400, 400, 100));
        retryButton.SetTexture(SpriteManager.GetSprite("RetryButton").texture);
        retryButton.OnClick += BeginNewRun;

        backButton = new Buttons(GraphicsDevice, new Rectangle(760, 530, 400, 100));
        backButton.SetTexture(SpriteManager.GetSprite("BackButton").texture);
        backButton.OnClick += ReturnToMainMenu;
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        
        if (gameStarted && !gameOver)
        {
            // 1. Pass the active room's walls to the player so they can slide against them
            if (activeRoom != null)
            {
                activeRoom.Update(gameTime);
                activeRoom.currentPlayer = player;
                player.CurrentWalls = activeRoom.WallColliders;
                
                List<Rectangle> closedDoors = new List<Rectangle>();
                foreach (var door in activeRoom.doors) // check each door for close or open
                {
                    if (door != null && !door.open) // if close add to the list 
                    {
                        closedDoors.Add(door.Collider);
                    }
                    else if (door != null && door.open) // if open remove from the list
                    {
                        closedDoors.Remove(door.Collider);
                    }
                }
               
                player.CurrentDoors = closedDoors;

                foreach (Sprite obj in activeRoom.objectList)
                {
                    if (obj is Zombie zombie)
                    {
                        zombie.CurrentWalls = activeRoom.WallColliders;
                        if (!zombie.IsDead)
                        {
                            zombie.Update(gameTime);
                        }
                    }
                }
            }

            // 2. Check for door transitions
            CheckRoomTransitions();
                
            player.Update(gameTime);
            
            CheckDoorKeyCollection();
            
            HandlePlayerAttack();
            
            foreach (Zombie enemy in zombies)
            {
                enemy.Update(gameTime);
            }
            
            CheckProjectileEnemyCollisions();
            
            RemoveDeadZombies();
            
            CheckEnemyPlayerCollisions();
        }
        else if (gameStarted && gameOver)
        {
            retryButton.Update();
            backButton.Update();
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
                foreach (Sprite obj in activeRoom.objectList)
                {
                    if (obj is Zombie zombie && !zombie.IsDead) 
                    {
                        DrawCollider(zombie.Collider, Color.Red);
                    }
                }
            }

            foreach (DoorKey doorKey in DoorKeys)
            {
                DrawCollider(doorKey.Collider, Color.Yellow);
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
            Rectangle backgroundRectangle = new Rectangle(
                0,
                0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            
            _spriteBatch.Draw(
                mainMenuBackgroundTexture,
                backgroundRectangle,
                Color.White);
            
            
            startingButton.Draw(_spriteBatch);
            SettingsButton.Draw(_spriteBatch);
            QuitButton.Draw(_spriteBatch);
        }
        else if (gameOver)
        {
            Rectangle darkOverlay = new Rectangle(
                0,
                0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            _spriteBatch.Draw(debugPixel, darkOverlay, Color.Black * 0.70f);
            retryButton.Draw(_spriteBatch);
            backButton.Draw(_spriteBatch);
        }
        else
        {
            DrawPlayerHealth();
            
            DrawPlayerKeys();
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
                    oppositeDoorIndex = 2; 
                    spawnOffset = new Vector2(0, -250); 
                    break;
                case 1: // Went RIGHT
                    nextCol += 1;
                    oppositeDoorIndex = 3; 
                    spawnOffset = new Vector2(250, 0); 
                    break;
                case 2: // Went DOWN
                    nextRow -= 1;
                    oppositeDoorIndex = 0; 
                    spawnOffset = new Vector2(0, 250); 
                    break;
                case 3: // Went LEFT
                    nextCol -= 1;
                    oppositeDoorIndex = 1; 
                    spawnOffset = new Vector2(-250, 0); 
                    break;
            }

            // Get the next room from the floor array
            Room nextRoom = currentFloor.GetRoomAt(nextCol, nextRow);

            if (nextRoom != null)
            {
                // 1. Remove old room's objects from the global draw list
                foreach (Sprite obj in activeRoom.objectList)
                {
                    sprites.Remove(obj);
                }

                // Update active room
                activeRoom = nextRoom;

                // 2. Add new room's objects to the global draw list so they render!
                foreach (Sprite obj in activeRoom.objectList)
                {
                    sprites.Add(obj);
                }

                // If it's a new room with enemies, trigger their spawn delay
                StartRoomEnemyDelay(activeRoom);

                // Move player to the new door and apply offset
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
            if (!enemy.CanDealContactDamage)
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
            foreach (Sprite obj in activeRoom.objectList)
            {
                if (obj is Zombie zombie)
                {
                    if (!zombie.CanDealContactDamage) continue;
                    if (zombie.Collider.Intersects(player.Collider))
                    {
                        player.TakeDamage(zombie.ContactDamage);
                        Console.WriteLine($"Player health: {player.Health}");
                    }
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
                foreach (Sprite obj in activeRoom.objectList)
                {
                    if (obj is Zombie zombie)
                    {
                        if (zombie.IsDead) continue;
        
                        if (projectile.Collider.Intersects(zombie.Collider))
                        {
                            zombie.TakeDamage(projectile.Damage);
                            projectile.Destroy();
                            break;
                        }
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
            if (zombies[i].IsDead || zombies[i].ReadyToRemove)
            {
                sprites.Remove(zombies[i]);
                zombies.RemoveAt(i);
            }
        }
        if (activeRoom == null) return;

        // Loop backwards through the active room's enemy list
        lastIndex = activeRoom.objectList.Count - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            if (activeRoom.objectList[i] is Zombie zombie && (zombie.IsDead || zombie.ReadyToRemove))
            {
                // 1. Remove the zombie from the drawing list so it disappears
                sprites.Remove(activeRoom.objectList[i]);
        
                // 2. Remove the zombie from the room's active logic list
                activeRoom.objectList.RemoveAt(i);
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

    private void BeginNewRun()
    {
        ClearPreviousRun();
        player.ResetForNewGame();

        gameStarted = true;
        gameOver = false;
        IsMouseVisible = false;
        previousCombatKeyboard = new KeyboardState();

        currentFloor = new Floor();
        currentFloor.generateFloor(Floor.Difficulty.Easy, player);

        foreach (Sprite sprite in currentFloor.GetRoomSprites())
        {
            sprites.Add(sprite);

            if (sprite is Room room)
            {
                if (room.isStartRoom)
                {
                    activeRoom = room;
                    
                    foreach (Sprite obj in room.objectList)
                    {
                        sprites.Add(obj);
                    }

                }
            }
        }

        if (activeRoom != null)
        {
            player.transform.position = activeRoom.transform.position;
            camera.Position = activeRoom.transform.position;
            StartRoomEnemyDelay(activeRoom);

            Vector2 keyPosition = activeRoom.transform.position + new Vector2(250f, 0f);
            SpawnDoorKey(keyPosition);
        }
    }

    private void StartRoomEnemyDelay(Room room)
    {
        foreach (Sprite obj in room.objectList)
        {
            if (obj is Zombie enemy)
            {
                enemy.StartRoomEntryDelay(RoomEntryZombieDelay);
            }
        }
    }

    
    private void ClearPreviousRun()
    {
        for (int i = DoorKeys.Count - 1; i >= 0; i--)
        {
            sprites.Remove(DoorKeys[i]);
        }
        DoorKeys.Clear();

        if (currentFloor != null)
        {
            foreach (Sprite sprite in currentFloor.GetRoomSprites())
            {
                if (sprite is Room room)
                {
                    foreach (Zombie enemy in room.objectList)
                    {
                        sprites.Remove(enemy);
                    }
                }

                sprites.Remove(sprite);
            }
        }

        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            sprites.Remove(zombies[i]);
        }
        zombies.Clear();

        activeRoom = null;
        currentFloor = null;
    }

    private void ReturnToMainMenu()
    {
        ClearPreviousRun();
        player.ResetForNewGame();

        gameStarted = false;
        gameOver = false;
        IsMouseVisible = true;
    }
    
    private void SpawnDoorKey(Vector2 position)
    {
        DoorKey doorKey = new DoorKey(position);
        
        doorKey.Start();
        DoorKeys.Add(doorKey);
        sprites.Add(doorKey);
    }

    private void CheckDoorKeyCollection()
    {
        for (int i = DoorKeys.Count - 1; i >= 0; i--)
        {
            DoorKey doorKey = DoorKeys[i];
            
            bool playerTouchedKey = player.Collider.Intersects(doorKey.Collider);

            if (!playerTouchedKey)
            {
                continue;
            }
            
            doorKey.Collect(player);
            sprites.Remove(doorKey);
            DoorKeys.RemoveAt(i);
        }
    }

    private void DrawPlayerKeys()
    {
        int numberOfKeys = player.GetKeyCount();

        for (int i = 0; i < numberOfKeys; i++)
        {
            int x = 30 + i * 50;
            int y = 110;

            int size = 40;
            
            Rectangle keyRectangle = new Rectangle(x, y, size, size);
            
            _spriteBatch.Draw(keyTexture, keyRectangle, Color.White);
        }
    }
}
