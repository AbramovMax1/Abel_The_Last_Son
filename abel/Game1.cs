using System;
using Abel_The_Last_Son.World.Floor;
using Abel_The_Last_Son.World.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Abel_The_Last_Son.Manager;
using Abel_The_Last_Son.World.Doors;
using Abel_The_Last_Son.World.Trash;

namespace Abel_The_Last_Son;

public class Game1 : Game
{
    Camera camera;

    // =========== references =============
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private InputManager inputManager;

    private bool gameStarted = false; // game run or not

    // ============
    // Texture
    private Texture2D _logo;

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
    // floor references
    private Floor generatedFloor;


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
        base.Initialize();

        // TODO: Add your initialization logic here
        inputManager = new InputManager();
        camera = new Camera(GraphicsDevice.Viewport);

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        new SpriteManager(Content);

        SpriteManager.WhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        SpriteManager.WhiteTexture.SetData(new Color[] { Color.White });

        // TODO: use this.Content to load your game content here

        // ============
        // Texture/Sprite
        // ============

        // Dungen generation
        FirstFloor();
        SecondFloor();
        FirstDoorLocked();
        SecondDoorLocked();



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

        Pixel();

        Start();
    }

    void FirstFloor()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("FloorOne", "Images/FloorOne");
    }

    void SecondFloor()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("FloorTwo", "Images/FloorTwo");
    }

    void FirstDoorLocked()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("DoorOneLocked", "Images/DoorFrameLockedFloorOne-export");
    }

    void SecondDoorLocked()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("DoorTwoLocked", "Images/DoorFrameLockedFlootTwo");
    }

    void TrashPaper()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("TrashPaper", "Images/Paper");
    }

    void RightDoorLocked()
    {
        //new SpriteManager(Content);
        SpriteManager.AddSprite("RightDoorLocked", "Images/DoorFrameLocked");
    }

    // void FloorLevelOne()
    // {
    //     new SpriteManager(Content);
    //     SpriteManager.AddSprite("FloorOne", "Images/FloorOne");
    // }

    void Pixel()
    {
       // new SpriteManager(Content);
        SpriteManager.AddSprite("pixel", "Images/pixel");
    }

    void PlayerSprite()
    {
        // player
        //new SpriteManager(Content);
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
        
        SpriteManager.AddSprite("StartButton", "UI/StartButton");

        // settingsButton
        //new SpriteManager(Content);
        SpriteManager.AddSprite("SettingsButton", "UI/SettingsButton");

        // quitButton
        //new SpriteManager(Content);
        SpriteManager.AddSprite("QuitButton", "UI/QuitButtons");
    }

    void Start()
    {
        // ===== UI ======
        // Buttons

        StartGame();
        SettingsBtttonOnClick();
        QuitGame();
        // Trash
        notColletiblesPaper = SceneManager.Create<NotColletiblesPaper>();
        
        // doors
        lockedDoor = SceneManager.Create<LockedDoor>();

        // player
        player = SceneManager.Create<Player>();
       
        SceneManager.Instance.Start();

        // The list will use sortingOrder to decide what draws first.
        //sprites.Add(floorOne);
        sprites.Add(notColletiblesPaper);
        //sprites.Add(wallLevelFirst);
        sprites.Add(lockedDoor);
        sprites.Add(player);
        
        
        player.Collider.RegisterOnCollision(player.OnCollisionEnter);
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

            generatedFloor = new Floor(); // creates a new floor
            generatedFloor.generateFloor(Floor.Difficulty.Easy);
            sprites.AddRange(generatedFloor.GetRoomSprites());

            // Find the start room and move the player into it!
            Room startRoom = generatedFloor.GetRoomSprites().OfType<Room>().FirstOrDefault(r => r.isStartRoom);
            if (startRoom != null)
            {
                player.transform.position = startRoom.transform.position;
            }
        };
    }

    void SettingsBtttonOnClick()
    {
        SettingsButton = new Buttons(GraphicsDevice,
            new Rectangle(760, 430, 400, 100));

        SettingsButton.SetTexture(SpriteManager.GetSprite("SettingsButton").texture);

        SettingsButton.OnClick += () => { Console.WriteLine("Settings"); };

    }

    void QuitGame()
    {
        QuitButton = new Buttons(GraphicsDevice, new Rectangle(760, 560, 400, 100));
        QuitButton.SetTexture(SpriteManager.GetSprite("QuitButton").texture);
        QuitButton.OnClick += () => { Exit(); };
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        if (gameStarted)
        {
            player.Update(gameTime);
            camera.Follow(player.transform.position);
        }
        else
        {
            startingButton.Update();
            SettingsButton.Update();
            QuitButton.Update();
        }

        SceneManager.Instance.Update(gameTime);

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

        // ==========================================
        // 1. WORLD DRAW (Affected by Camera)
        // ==========================================
        // Pass BOTH the samplerState (to stop blur) AND the camera transform!
        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: camera.Transform);

        // Draw all the in-game stuff (Room, Player, Enemies)
        foreach (Sprite sprite in sprites.OrderBy(sprite => sprite.sortingOrder))
        {
            sprite.DrawSprite(_spriteBatch);
        }

        _spriteBatch.End();


        // ==========================================
        // 2. UI DRAW (Fixed to the Screen)
        // ==========================================
        // Start a brand new batch for UI WITHOUT the camera matrix.
        // (We keep PointClamp here so your button pixel art stays crisp too).
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (!gameStarted)
        {
            startingButton.Draw(_spriteBatch);
            SettingsButton.Draw(_spriteBatch);
            QuitButton.Draw(_spriteBatch);
        }

        _spriteBatch.End();


        base.Draw(gameTime);
    }
}