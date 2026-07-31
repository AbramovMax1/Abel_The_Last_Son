using System;
using System.Collections.Generic;

namespace ConsoleApp1;

public class Room
{
    private bool isVisited = false;
    private int enemyAmount;
    private bool roomCleared = false;
    public int row {get; private set;}
    public int collumn {get; private set;}
    public List<Door> doors = new List<Door>(4);
    public List<bool> doorGenerationAttempt = new List<bool>(4);
    public bool isStartRoom {get; private set;}
    public int? entranceDoorDirection {get; private set;}
    
    public enum RoomType
    {
        bacicRoom,
        EndRoom
    }
    private RoomType currentRoom;
    
    public override string ToString()
    {
        if (currentRoom == RoomType.EndRoom)
        {
            return $"E at {row}, {collumn}" ;
        }
        else if (currentRoom == RoomType.bacicRoom)
        {
            return $"B at {row}, {collumn}";
        }
        else return $"{currentRoom.ToString()} at {row}, {collumn}";
        
    }

    public Room CreateRoom(Floor.Difficulty difficulty, int currentRow, int currentCullumn , int? entranceDoorDirection = null , bool isStartRoom =  false)
    {
        currentRoom = RoomType.bacicRoom;
        collumn = currentCullumn;
        row = currentRow;
        this.isStartRoom = isStartRoom;
        this.entranceDoorDirection = entranceDoorDirection;
        
        isVisited = false;
        
        for (int i = 0; i < 4; i++)
        {
            doorGenerationAttempt.Add(false);
            doors.Add(null);
        }
        if (!isStartRoom)
        {
            AddDoor((int)entranceDoorDirection, new Door((int)entranceDoorDirection));
            doorGenerationAttempt[(int)entranceDoorDirection] = true;
            CalculateBasedOnDifficulty(difficulty);
           
            //generate enemy's
        }
        else
        {
            isVisited = true;
        }
        return this;
    }

    public void AddDoor(int direction, Door? door)
    {
        doors[direction] = door;
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
                enemyAmount = rnd.Next(1, 4);
                break;
            }
            case Floor.Difficulty.Medium:
            {
                enemyAmount = rnd.Next(4, 6);
                break;
            }
            case Floor.Difficulty.Hard:
            {
                enemyAmount = rnd.Next(6, 8);
                break;
            }
        }
    }
}