using System;
using System.Collections.Generic;
using System.Diagnostics;
using Abel_The_Last_Son;
using Abel_The_Last_Son.Core.Collider;
using Abel_The_Last_Son.Manager;
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

    // door sprite
    private string doorSpriteName;
    private string lockedDoorSpriteName;

    // wall colliders
    private List<Collider> wallColliders = new List<Collider>();

    // wall sprites
    private List<Sprite> wallSprites = new List<Sprite>();

    private const int scale = 15;

    public enum RoomType
    {
        bacicRoom,
        EndRoom
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
    
    public Direction entranceDoorDirection { get; private set; }

    private RoomType currentRoom;

    public override string ToString()
    {
        if (currentRoom == RoomType.EndRoom)
        {
            return $"E at {row}, {collumn}";
        }
        else if (currentRoom == RoomType.bacicRoom)
        {
            return $"B at {row}, {collumn}";
        }
        else return $"{currentRoom.ToString()} at {row}, {collumn}";

    }


    public Room(string spriteName, Floor.Difficulty difficulty, int currentRow, int currentCullumn, Vector2 grideSecnter ,
       Direction entranceDoorDirection = Direction.None, bool isStartRoom = false, bool isCurrentRoom = false) : base(spriteName)
    {
        currentRoom = RoomType.bacicRoom;
        collumn = currentCullumn;
        row = currentRow;
        
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


            //generate enemy's
        }
        else
        {
            isVisited = true;
        }

    }
    
    public List<Collider> GetWallColliders()
    {
        return wallColliders;
    }
    

    public void CreateWallsAndDoors()
    {
        int wallThickness = (int)(12 * scale); // wall thickness modifier
        int doorSize = (int)(12 * scale);

        float roomXPosition = transform.position.X - ((texture.Width * 0.5f) * scale);
        float roomYPosition = transform.position.Y - ((texture.Height * 0.5f) * scale);
        
        float roomWidth = texture.Width * scale;
        float roomHight = texture.Height * scale;

        // up wall
        if (doors[0] != null)
        {
            float doorXPosition =
                roomXPosition + (roomWidth * 0.5f) - (doorSize * 0.5f); // find the middle of the wall to plant the door
            doors[0].transform.position = new Vector2(doorXPosition, roomYPosition); // plant the door there
            doors[0].transform.rotation = 0.0f;

            // creates a wall to the left of the door
            CreateWallSegment(roomXPosition, roomYPosition, (roomWidth * 0.5f) - (doorSize * 0.5f), wallThickness);
            // creates a wall to the right of the door
            CreateWallSegment(doorXPosition + doorSize, roomYPosition, (roomWidth * 0.5f) - (doorSize * 0.5f),
                wallThickness);
        }
        else
        {
            CreateWallSegment(roomXPosition, roomYPosition, roomWidth, wallThickness);
        }

        // ==========================================
        // 1. RIGHT WALL (Right Edge)
        // ==========================================
        if (doors[1] != null)
        {
            float doorYPosition = roomYPosition + (roomHight * 0.5f) - (doorSize * 0.5f);

            doors[1].transform.position = new Vector2(roomXPosition + roomWidth - wallThickness, doorYPosition);
            doors[1].transform.rotation = MathHelper.ToRadians(90);

            // Split Wall: Top piece
            CreateWallSegment(roomXPosition + roomWidth - wallThickness, roomYPosition, wallThickness,
                (roomHight * 0.5f) - (doorSize * 0.5f));
            // Split Wall: Bottom piece
            CreateWallSegment(roomXPosition + roomWidth - wallThickness, doorYPosition + doorSize, wallThickness,
                (roomHight * 0.5f) - (doorSize * 0.5f));
        }
        else
        {
            CreateWallSegment(roomXPosition + roomWidth - wallThickness, roomYPosition, wallThickness, roomHight);
        }

        // ==========================================
        // 2. DOWN WALL (Bottom Edge)
        // ==========================================
        if (doors[2] != null)
        {
            float doorX = roomXPosition + (roomWidth * 0.5f) - (doorSize * 0.5f);

            doors[2].transform.position = new Vector2(doorX, roomYPosition + roomHight - wallThickness);
            doors[2].transform.rotation = MathHelper.ToRadians(180);

            // Split Wall: Left piece
            CreateWallSegment(roomXPosition, roomYPosition + roomHight - wallThickness,
                (roomWidth * 0.5f) - (doorSize * 0.5f), wallThickness);
            // Split Wall: Right piece
            CreateWallSegment(doorX + doorSize, roomYPosition + roomHight - wallThickness,
                (roomWidth * 0.5f) - (doorSize * 0.5f), wallThickness);
        }
        else
        {
            CreateWallSegment(roomXPosition, roomYPosition + roomHight - wallThickness, roomWidth, wallThickness);
        }

        // ==========================================
        // 3. LEFT WALL (Left Edge)
        // ==========================================
        if (doors[3] != null)
        {
            float doorY = roomYPosition + (roomHight * 0.5f) - (doorSize * 0.5f);

            doors[3].transform.position = new Vector2(roomXPosition, doorY);
            doors[3].transform.rotation = MathHelper.ToRadians(270);

            // Split Wall: Top piece
            CreateWallSegment(roomXPosition, roomYPosition, wallThickness, (roomHight * 0.5f) - (doorSize * 0.5f));
            // Split Wall: Bottom piece
            CreateWallSegment(roomXPosition, doorY + doorSize, wallThickness, (roomHight * 0.5f) - (doorSize * 0.5f));
        }
        else
        {
            CreateWallSegment(roomXPosition, roomYPosition, wallThickness, roomHight);
        }

    }

    // A helper method to keep the code above clean
    private void CreateWallSegment(float xPosition, float yPosition, float width, float height)
    {
        Sprite wallSprite = new Sprite("pixel");
        wallSprite.Start();
        
        
        wallSprite.color = Color.Transparent;
        
        wallSprite.transform.position = new Vector2(
            xPosition + (width * 0.5f), 
            yPosition + (height * 0.5f));
            
        wallSprite.transform.scale = new Vector2(width, height);

        Collider wallCollider = new Collider();
        wallCollider.Parent = wallSprite;
        wallCollider.IsTrigger = false;

        wallSprites.Add(wallSprite);
        wallColliders.Add(wallCollider);
    }   

    public Room GetRoom()
    {
        return this;
    }

    public Room GetRoomInDirection(int direction)
    {
        Room desierdedRoom = null;
        switch (direction)
        {
            case 0:
            {
                
                break;
            }
            case 1:
            {
                break;
            }
            case 2:
            {
                break;
            }
            case 3:
            {
                break;
            }
        }
        return desierdedRoom;
    }
    public void AddDoor(Direction direction, bool doorExist)
    {
        if (!doorExist) return;
        if (isStartRoom)
        {
            SceneManager.Create<Door(direction, doorSpriteName, true)r>()
            doors[DirectionToNumber(direction)] = new Door(direction, doorSpriteName, true);
        }
        else
        {
            doors[DirectionToNumber(direction)] = new Door(direction, doorSpriteName, false);
        }
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
            //TODo: change enemy generation to an aray of set incounters
            case Floor.Difficulty.Easy:
            {
                doorSpriteName = "DoorOneLocked";
                enemyAmount = rnd.Next(1, 4);
                break;
            }
            case Floor.Difficulty.Medium:
            {
                doorSpriteName = "DoorTwoLocked";
                enemyAmount = rnd.Next(4, 6);
                break;
            }
            case Floor.Difficulty.Hard:
            {
                doorSpriteName = "DoorOneLocked";
                enemyAmount = rnd.Next(6, 8);
                break;
            }
        }
    }

    public void FinnishedRoomGeneration()
    {
        CreateWallsAndDoors();
    }

    public int DirectionToNumber(Direction direction)
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

    public override void DrawSprite(SpriteBatch spriteBatch)
    {
        
        base.DrawSprite(spriteBatch);
        
        foreach (var wallSprite in wallSprites)
        {
            wallSprite.DrawSprite(spriteBatch);
        }
        
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.DrawSprite(spriteBatch);
            }
        }
        
#if DEBUG
        
        // Loop through the list to draw every wall piece, no matter how many there are
        foreach (var wallCollider in wallColliders)
        {
            wallCollider.DrawSprite(spriteBatch);
        }
        
#endif
    }
}