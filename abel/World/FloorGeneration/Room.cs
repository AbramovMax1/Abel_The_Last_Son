using System;
using System.Collections.Generic;
using System.Diagnostics;
using Abel_The_Last_Son;
using Abel_The_Last_Son.Core.Enums;
using Abel_The_Last_Son.Enemies;
using Abel_The_Last_Son.Manager;
using Abel_The_Last_Son.World.Trash;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Abel_The_Last_Son;

public class Room : Sprite
{
    private bool isVisited = false;
    private int enemyAmount;
    private bool roomCleared = false;
    public int row { get; private set; }
    public int collumn { get; private set; }
    public List<Door> doors = new(4);
    public List<bool> doorGenerationAttempt = new(4);
    public bool isStartRoom { get; private set; }
    public List<Sprite> objectList {get; private set;}
    
    public Player currentPlayer;

    // door sprite
    private string doorSpriteName;
    private string lockedDoorSpriteName;
    private string closedSpriteName;
    private string openSpriteName; 
    public string lockedSpriteName { get; private set; }

    private const int scale = 15;

    public enum RoomType
    {
        bacicRoom,
        EndRoom,
        LockedEndRoom
    } 
    // dividing the room to a grid to spawn objects easily
    private Vector2[,] roomGrid;
    
    public Direction entranceDoorDirection { get; private set; }

    private RoomType currentRoom;
    // prints the room as the room type and cords at console
    public override string ToString()
    {
        switch (currentRoom)
        {
            case RoomType.EndRoom:
            {
                return $"E at {row}, {collumn}";
            }
            case RoomType.LockedEndRoom:
            {
                return $"LE at {row}, {collumn}";
            }
            case RoomType.bacicRoom:
            {
                return $"B at {row}, {collumn}"; 
            }
            default: return $"{currentRoom.ToString()} at {row}, {collumn}";
        }

    }
    // get room values upon creation
    public Room(Player player, string spriteName, Floor.Difficulty difficulty, int currentRow, int currentCullumn, Vector2 grideSecnter ,
       Direction entranceDoorDirection = Direction.None, bool isStartRoom = false) : base(spriteName)
    {
        currentPlayer = player;
        currentRoom = RoomType.bacicRoom;
        collumn = currentCullumn;
        row = currentRow;

        objectList = new List<Sprite>(); // set a list to the stuff that will be in the room
        
        transform.scale = new Vector2(scale,scale); // make room size up to regular scale
        Start();
        
        // Calculate actual pixel dimensions of the scaled room
        float roomWidth = texture.Width * scale;
        float roomHeight = texture.Height * scale;

        // calculate the offset of the room based on grid center
        float gridCenterX = grideSecnter.X * 0.5f; 
        float gridCenterY = grideSecnter.Y * 0.5f; 

        float offsetX = (collumn - gridCenterX) * roomWidth;
        float offsetY = (row - gridCenterY) * roomHeight;

        // Position the room relative to the screen center plus its grid offset
        transform.position = new Vector2(Game1._screenCenter.X + offsetX, Game1._screenCenter.Y - offsetY);

        roomGrid = DivideRoomToGrid(); // divide the current room to a grid 
        
        this.isStartRoom = isStartRoom;
        this.entranceDoorDirection = entranceDoorDirection;

        isVisited = false;
        CalculateBasedOnDifficulty(difficulty); // currently only set door sprite based on difficulty
        
        // set up the doors to be ready to start generating
        for (int i = 0; i < 4; i++)
        {
            doorGenerationAttempt.Add(false);
            doors.Add(null);
        }

        if (!isStartRoom) // if it is not a starting room add a door in the direction that connects to the other room
        {
            AddDoor(entranceDoorDirection, true);
            doorGenerationAttempt[(int)entranceDoorDirection] = true;

            objectList = ChoosObjectSpawnPositions(); //generate enemy's, obstecals and visual papers
            
        }
        else // if it is a starting room, the player visited it
        {
            isVisited = true;
        }

    }
    
    
    public override void Update(GameTime gameTime)
    {
        foreach (var door in doors)
        {
            if (door == null) continue;

            if (door.locked)
            {
                // create just a little bigger hitbox to register key touche
                Rectangle interactionBox = door.Collider;
                interactionBox.Inflate(8, 8); // Expands the detection box by 8 pixels in all directions

                // Check if player interacts with the locked door and has at least one key
                if (currentPlayer != null && interactionBox.Intersects(currentPlayer.Collider))
                {
                    if (currentPlayer.GetKeyCount() > 0) // check if the player owns any keys
                    {
                        currentPlayer.UseKey(); // Consumes a key
                        door.Unlock(); // Unlocks and opens the door
                        Game1.doorUnlockSound.Play();
                    }
                }
            }
            else if (enemyAmount <= 0)
            {
                door.Open();
            }
            else
            {
                door.Close();
            }
        }

        base.Update(gameTime);
    }

    public List<Rectangle> WallColliders { get; private set; } = new List<Rectangle>();

    private void CreateWallsAndDoors() //creat the walls based on if there is a door or not
{
    int wallThickness = (int)(12 * scale);
    int doorSize = (int)(12 * scale);

    float roomXPosition = transform.position.X - ((texture.Width * 0.5f) * scale);
    float roomYPosition = transform.position.Y - ((texture.Height * 0.5f) * scale);
    
    float roomWidth = texture.Width * scale;
    float roomHight = texture.Height * scale;

    float halfGap = doorSize * 0.5f;

    // ==========================================
    // 0. UP WALL
    // ==========================================
    if (doors[0] != null)
    {
        float doorX = roomXPosition + (roomWidth * 0.5f);
        float doorY = roomYPosition + (wallThickness * 0.5f);
        doors[0].transform.position = new Vector2(doorX, doorY);
        doors[0].transform.rotation = 0.0f;

        float segmentWidth = (roomWidth * 0.5f) - halfGap;
        CreateWallSegment(roomXPosition, roomYPosition, segmentWidth, wallThickness);
        CreateWallSegment(doorX + halfGap, roomYPosition, segmentWidth, wallThickness);
    }
    else
    {
        CreateWallSegment(roomXPosition, roomYPosition, roomWidth, wallThickness);
    }

    // ==========================================
    // 1. RIGHT WALL
    // ==========================================
    if (doors[1] != null)
    {
        float doorX = roomXPosition + roomWidth - (wallThickness * 0.5f) ;
        float doorY = roomYPosition + (roomHight * 0.5f);
        doors[1].transform.position = new Vector2(doorX, doorY);
        doors[1].transform.rotation = MathHelper.ToRadians(90);

        float segmentHeight = (roomHight * 0.5f) - halfGap;
        CreateWallSegment(roomXPosition + roomWidth - wallThickness, roomYPosition, wallThickness, segmentHeight);
        CreateWallSegment(roomXPosition + roomWidth - wallThickness, doorY + halfGap, wallThickness, segmentHeight);
    }
    else
    {
        CreateWallSegment(roomXPosition + roomWidth - wallThickness, roomYPosition, wallThickness, roomHight);
    }

    // ==========================================
    // 2. DOWN WALL
    // ==========================================
    if (doors[2] != null)
    {
        float doorX = roomXPosition + (roomWidth * 0.5f);
        float doorY = roomYPosition + roomHight - (wallThickness * 0.5f);
        doors[2].transform.position = new Vector2(doorX, doorY);
        doors[2].transform.rotation = MathHelper.ToRadians(180);

        float segmentWidth = (roomWidth * 0.5f) - halfGap;
        CreateWallSegment(roomXPosition, roomYPosition + roomHight - wallThickness, segmentWidth, wallThickness);
        CreateWallSegment(doorX + halfGap, roomYPosition + roomHight - wallThickness, segmentWidth, wallThickness);
    }
    else
    {
        CreateWallSegment(roomXPosition, roomYPosition + roomHight - wallThickness, roomWidth, wallThickness);
    }

    // ==========================================
    // 3. LEFT WALL
    // ==========================================
    if (doors[3] != null)
    {
        float doorX = roomXPosition + (wallThickness * 0.5f);
        float doorY = roomYPosition + (roomHight * 0.5f);
        doors[3].transform.position = new Vector2(doorX, doorY);
        doors[3].transform.rotation = MathHelper.ToRadians(270);

        float segmentHeight = (roomHight * 0.5f) - halfGap;
        CreateWallSegment(roomXPosition, roomYPosition, wallThickness, segmentHeight);
        CreateWallSegment(roomXPosition, doorY + halfGap, wallThickness, segmentHeight);
    }
    else
    {
        CreateWallSegment(roomXPosition, roomYPosition, wallThickness, roomHight);
    }
}
    
    // create a wall based on the given positions
    private void CreateWallSegment(float xPosition, float yPosition, float width, float height)
    {
        //create a Rectangle collider and add it to the list.
        Rectangle wallRect = new Rectangle((int)xPosition, (int)yPosition, (int)width, (int)height);
        WallColliders.Add(wallRect);
    }
    
    public void AddDoor(Direction direction, bool doorExist) // adds a door in the desired direction
    {
        if (!doorExist) return;
        bool isOpen = isStartRoom;
        doors[DirectionToNumber(direction)] = new Door(direction, closedSpriteName, openSpriteName, lockedSpriteName, isOpen, false);
        doors[DirectionToNumber(direction)].Start();
    }
    public void LockDoor(Direction direction) // locks the door in the direction
    {
        int index = DirectionToNumber(direction);
        if (index >= 0 && doors[index] != null)
        {
            doors[index].ConvertToLocked(lockedSpriteName);
        }
    }

    public void ChangeRoomType(RoomType newRoomType)
    {
        currentRoom = newRoomType;
    }

    private void CalculateBasedOnDifficulty(Floor.Difficulty difficulty) // set the door sprite to the sellected difficulty
    {
        Random rnd = new Random();
        switch (difficulty)
        {
            case Floor.Difficulty.Easy:
            {
                closedSpriteName = "CloseDoorOne";
                openSpriteName = "OpenDoorOne";
                lockedSpriteName = "DoorOneLocked";
                break;
            }
            case Floor.Difficulty.Medium:
            {
                closedSpriteName = "CloseDoorTwo";
                openSpriteName = "OpenDoorTwo";
                lockedSpriteName = "DoorTwoLocked";
                break;
            }
            case Floor.Difficulty.Hard:
            {
                closedSpriteName = "CloseDoorOne";
                openSpriteName = "OpenDoorOne";
                lockedSpriteName = "DoorOneLocked";
                break;
            }
        }
    }

    public void FinnishedRoomGeneration() 
    {
        CreateWallsAndDoors();
    }

    private int DirectionToNumber(Direction direction)
    {
        switch (direction)
        {
         case Direction.Up:
             return 0;
         case Direction.Right:
             return 1;
         case Direction.Down:
             return 2;
         case Direction.Left:
             return 3;
         default:
             return -1;
        }
    }

    private Vector2[,] DivideRoomToGrid()
    {
        // Create the 2D array to hold the Vector2 positions
        Vector2[,] gridPositions = new Vector2[15, 7]; 
    
        float cellWidth = 106;
        float cellHeight = 90;
        
        // Calculate the total size of the grid
        float totalGridWidth = 15 * cellWidth;
        float totalGridHeight = 7 * cellHeight;

        // Find the very top-left starting coordinate of the walkable room.
        // Assuming transform.position is the exact center of the room.
        float startX = transform.position.X - (totalGridWidth / 2f) + (cellWidth / 2f);
        float startY = transform.position.Y - (totalGridHeight / 2f) + (cellHeight / 2f);
        
        for (int col = 0; col < 15; col++)
        {
            for (int row = 0; row < 7; row++)
            {
                // multiply the current row/col by the cell size to get the exact world position
                float cellX = startX + (col * cellWidth);
                float cellY = startY + (row * cellHeight);

                // Store the position in the grid
                gridPositions[col, row] = new Vector2(cellX, cellY);
            }
        }
    
        return gridPositions;
    }
    

    List<Sprite> ChoosObjectSpawnPositions()
    {
        List<Sprite> tempList = new();
        Random rnd = new Random();
        int chosenEnemySpawn = rnd.Next(4);
        switch (chosenEnemySpawn) // "randomis" the selected room spawns
        {
            case 0:
            {
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[10,1]));
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[10,5]));
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[8,5]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[3,6]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[2,6]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[1,6]));
                break;
            }
            case 1:
            {
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[10,3]));
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[3,5]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[9,5]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[8,5]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[9,6]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[8,6]));
                
                break;
            }
            case 2:
            {
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[3,3]));
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[10,5]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(1,15),rnd.Next(1,7)]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[2,5]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[5,2]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[14,5]));
                tempList.Add(InitializeEntity(new Rock(),roomGrid[12,1]));
                break;
            }
            case 3:
            {
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[8,2]));
                tempList.Add(InitializeEntity(new Zombie(currentPlayer),roomGrid[14,2]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeEntity(new NotColletiblesPaper(),roomGrid[rnd.Next(15),rnd.Next(7)]));
                break;
            }
        }
        
       
        
        return tempList;
    }

    private T InitializeEntity<T>(T entity, Vector2 position) where T : Sprite
    {
        // 1. Handle common logic that applies to EVERY entity
        entity.Start();
        entity.transform.position = position;

        // 2. Handle specific logic based on the type of the entity
        switch (entity)
        {
            case Rock rock:
                // Register the rock's collider as a wall
                WallColliders.Add(rock.Collider);
                break;

            case Zombie zombie:
                // Handle enemy tracking and death events
                enemyAmount++;
                zombie.OnDeath += () =>
                {
                    enemyAmount--;
                    Console.WriteLine($"received on death, enemy amount is {enemyAmount}");
                };
                break;
        }

        return entity;
    }

    public override void DrawSprite(SpriteBatch spriteBatch)
    {
        base.DrawSprite(spriteBatch);
        
        
        foreach (Door door in doors)
        {
            if (door == null || door.texture == null) continue; 

            // Calculate the center of the texture so it rotates properly
            Vector2 origin = new Vector2(door.texture.Width / 2f, door.texture.Height / 2f);
            // Draw using position, rotation, origin, and scale!
            spriteBatch.Draw(
                door.texture,
                door.transform.position,
                null, // draw the whole texture
                Color.White,
                door.transform.rotation, // Your 90/180/270 degree rotations
                origin, // Center point
                door.transform.scale, // Your Vector2(6f, 6f)
                SpriteEffects.None,
                0f
            );
        }
    }
}