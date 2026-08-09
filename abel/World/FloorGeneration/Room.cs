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
    private bool isCurrentRoom = false;
    private int enemyAmount;
    private bool roomCleared = false;
    public int row { get; private set; }
    public int collumn { get; private set; }
    public List<Door> doors = new List<Door>(4);
    public List<bool> doorGenerationAttempt = new List<bool>(4);
    public bool isStartRoom { get; private set; }
    public List<Sprite> objectList {get; private set;}
    
    public Player currentPlayer;

    // door sprite
    private string doorSpriteName;
    private string lockedDoorSpriteName;

    private const int scale = 15;

    public enum RoomType
    {
        bacicRoom,
        EndRoom,
        LockedEndRoom
    }

    private Vector2[,] roomGrid;
    
    public Direction entranceDoorDirection { get; private set; }

    private RoomType currentRoom;

    public override string ToString()
    {
        if (currentRoom == RoomType.EndRoom)
        {
            return $"E at {row}, {collumn}";
        }
        else if (currentRoom == RoomType.LockedEndRoom)
        {
            return $"LE at {row}, {collumn}";
        }
        else if (currentRoom == RoomType.bacicRoom)
        {
            return $"B at {row}, {collumn}";
        }
        else return $"{currentRoom.ToString()} at {row}, {collumn}";    

    }
    
    public Room(Player player, string spriteName, Floor.Difficulty difficulty, int currentRow, int currentCullumn, Vector2 grideSecnter ,
       Direction entranceDoorDirection = Direction.None, bool isStartRoom = false, bool isCurrentRoom = false) : base(spriteName)
    {
        currentPlayer = player;
        currentRoom = RoomType.bacicRoom;
        collumn = currentCullumn;
        row = currentRow;

        objectList = new List<Sprite>();
        
        transform.scale = new Vector2(scale,scale); // make room size up to regular scale
        Start();
        
        // Calculate actual pixel dimensions of the scaled room
        float roomWidth = texture.Width * scale;
        float roomHeight = texture.Height * scale;

        // Since the grid center is (collumns / 2, rows / 2), 
        // we offset each room relative to the screen center based on its grid distance.
        // (Note: we subtract 5 or 6 depending on your grid size, or dynamically calculate grid center).
        // For an 11x11 grid, the center column/row is 5.
        float gridCenterX = grideSecnter.X * 0.5f; 
        float gridCenterY = grideSecnter.Y * 0.5f; 

        float offsetX = (collumn - gridCenterX) * roomWidth;
        float offsetY = (row - gridCenterY) * roomHeight;

        // Position the room relative to the screen center plus its grid offset
        transform.position = new Vector2(Game1._screenCenter.X + offsetX, Game1._screenCenter.Y - offsetY);

        roomGrid = DivideRoomToGrid();
        
        this.isStartRoom = isStartRoom;
        this.entranceDoorDirection = entranceDoorDirection;
        this.isCurrentRoom = isCurrentRoom;

        isVisited = false;
        CalculateBasedOnDifficulty(difficulty);

        for (int i = 0; i < 4; i++)
        {
            doorGenerationAttempt.Add(false);
            doors.Add(null);
        }

        if (!isStartRoom)
        {
            AddDoor(entranceDoorDirection, true);
            doorGenerationAttempt[(int)entranceDoorDirection] = true;

            objectList = ChoosZombiesSpawnPositions();
            //generate enemy's
        }
        else
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
                // Check if player collides with the locked door and has at least one key
                if (currentPlayer != null && door.Collider.Intersects(currentPlayer.Collider))
                {
                    // Assumes player has a Keys property or a method to consume a key
                    if (currentPlayer.GetKeyCount() > 0)
                    {
                        currentPlayer.UseKey(); // Consumes a key
                        door.Unlock(); // Unlocks and opens the door
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

    private void CreateWallsAndDoors()
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
    
    // A helper method to keep the code above clean
    private void CreateWallSegment(float xPosition, float yPosition, float width, float height)
    {
        // Just create the Rectangle collider and add it to the list.
        Rectangle wallRect = new Rectangle((int)xPosition, (int)yPosition, (int)width, (int)height);
        WallColliders.Add(wallRect);
    }
    
    public void AddDoor(Direction direction, bool doorExist)
    {
        if (!doorExist) return;
        if (isStartRoom)
        {
            doors[DirectionToNumber(direction)] = new Door(direction, doorSpriteName, true);
        }
        else
        {
            doors[DirectionToNumber(direction)] = new Door(direction, doorSpriteName, false);
        }
        doors[DirectionToNumber(direction)].Start();
    }

    public void ChangeRoomType(RoomType newRoomType)
    {
        currentRoom = newRoomType;
    }

    private void CalculateBasedOnDifficulty(Floor.Difficulty difficulty)
    {
        Random rnd = new Random();
        switch (difficulty)
        {
            case Floor.Difficulty.Easy:
            {
                doorSpriteName = "DoorOneLocked";
                break;
            }
            case Floor.Difficulty.Medium:
            {
                doorSpriteName = "DoorTwoLocked";
                break;
            }
            case Floor.Difficulty.Hard:
            {
                doorSpriteName = "DoorOneLocked";
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

        // Your nested loops!
        for (int col = 0; col < 15; col++)
        {
            for (int row = 0; row < 7; row++)
            {
                // MULTIPLY the current row/col by the cell size to get the exact world position
                float cellX = startX + (col * cellWidth);
                float cellY = startY + (row * cellHeight);

                // Store the position in the grid
                gridPositions[col, row] = new Vector2(cellX, cellY);
            }
        }
    
        return gridPositions;
    }
    

    List<Sprite> ChoosZombiesSpawnPositions()
    {
        List<Sprite> tempList = new();
        Random rnd = new Random();
        int chosenEnemySpawn = rnd.Next(4);
        switch (chosenEnemySpawn)
        {
            case 0:
            {
                tempList.Add(inisializeZombie(roomGrid[10,1]));
                tempList.Add(inisializeZombie(roomGrid[10,5]));
                tempList.Add(inisializeZombie(roomGrid[2,5]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeRock(roomGrid[3,1]));
                tempList.Add(InitializeRock(roomGrid[2,1]));
                tempList.Add(InitializeRock(roomGrid[1,1]));
                break;
            }
            case 1:
            {
                tempList.Add(inisializeZombie(roomGrid[10,3]));
                tempList.Add(inisializeZombie(roomGrid[3,5]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(InitializeRock(roomGrid[9,6]));
                tempList.Add(InitializeRock(roomGrid[8,6]));
                tempList.Add(InitializeRock(roomGrid[9,6]));
                tempList.Add(InitializeRock(roomGrid[8,6]));
                
                break;
            }
            case 2:
            {
                tempList.Add(inisializeZombie(roomGrid[3,3]));
                tempList.Add(inisializeZombie(roomGrid[10,5]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(1,15),rnd.Next(1,7)]));
                tempList.Add(InitializeRock(roomGrid[2,5]));
                tempList.Add(InitializeRock(roomGrid[5,2]));
                tempList.Add(InitializeRock(roomGrid[14,5]));
                tempList.Add(InitializeRock(roomGrid[12,1]));
                break;
            }
            case 3:
            {
                tempList.Add(inisializeZombie(roomGrid[8,2]));
                tempList.Add(inisializeZombie(roomGrid[14,2]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                tempList.Add(inisializepPaper(roomGrid[rnd.Next(15),rnd.Next(7)]));
                break;
            }
        }
        
       
        
        return tempList;
    }

    private NotColletiblesPaper inisializepPaper(Vector2 position)
    {
        NotColletiblesPaper paper = new NotColletiblesPaper();
        paper.Start();
        paper.transform.position = position;
        return paper;
    }
    private Rock InitializeRock(Vector2 position)
    {
        Rock rock = new Rock();
        rock.Start();
        rock.transform.position = position;

        // ADD THIS: Register the rock's collider as a wall so the player slides against it!
        WallColliders.Add(rock.Collider);

        return rock;
    }
    private Zombie inisializeZombie(Vector2 position)
    {
        Zombie zombie = new Zombie(currentPlayer);
        zombie.Start();
        zombie.transform.position = position;
        enemyAmount++;

        zombie.OnDeath += () =>
        {
            enemyAmount--;
            Console.WriteLine($"resived on death, enemy amount is  {enemyAmount}");
        };
        return zombie;
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