using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Abel_The_Last_Son.Core.Enums;

namespace Abel_The_Last_Son;

public class Floor 
{
    public enum Difficulty
    {
        Easy, Medium, Hard
    }
    private int roomAmount;
    private int currentRoomAmount;
    private Room[,] roomArray;
    private int collumns;
    private int rows;
    private bool plasedStartingRoom;
    private Queue<Room> roomQueue = new Queue<Room>();
    private Queue<Room> endRoomQueue = new Queue<Room>();
    private Random rnd = new Random();
    private bool dequeueRoom = false;
    private  bool checkFail = false;
    private int newRows = -1;
    private int newCollumns = -1;
    private bool generatedRoom = false;
    
    public Floor generateFloor(Difficulty difficulty, Player player)
    {
        CalculateBasedOnDiffiulty(difficulty, out Vector2 grideCenter);
        bool generationSuccessful = false;
        while (!generationSuccessful) // loop until making a complete dungeon
        {
            // clears the arrays in case of a previous generation attempt
            Array.Clear(roomArray, 0, roomArray.Length); 
            roomQueue.Clear();
            endRoomQueue.Clear();
            currentRoomAmount = 0;

            // generate the first room 
            roomQueue.Enqueue(roomArray[collumns / 2, rows / 2] =
                new Room(player,"FloorOne", difficulty, rows / 2, collumns / 2,grideCenter , Direction.None, true, true));
            Room currentRoom = null;
            dequeueRoom = true;

            while (roomQueue.Count > 0 && currentRoomAmount < roomAmount - 1) // checks that we don't generate more rooms than the room amount 
            {
                if (dequeueRoom) // check if there is a need to dequeue a room
                {
                    if (roomQueue.Count == 0)
                    {
                        Console.WriteLine("Dungeon dead ended early! No more rooms in queue.");
                        break;
                    }
                    currentRoom = roomQueue.Dequeue(); // sets the current room to the dequeued room
                    dequeueRoom = false;
                }

                int doorAmount = 0; // sets door amount to 0 
                int direction;

                // check for each direction if to generate a door(and a room) or not 
                for (direction = 0; direction < 4; direction++)
                {
                    if (doorAmount >= 4) continue; // check if 4 doors where generated already (safety check)

                    // if door already generated then no need to override it just continue to the next door 
                    if (currentRoom.doorGenerationAttempt[direction]) continue;

                    // mark this generetion atempt 
                    currentRoom.doorGenerationAttempt[direction] = true;

                    //if the number of generared rooms is the room amount go to the queue and make every non decided door a no door 
                   
                    
                    // make checks for each direction to see if there is a need to force making/blocking a door 
                    checkFail = false;
                    switch (direction)
                    {
                        case 0:
                        {
                            // check if the door tries to generate to an out of bounds room
                            if (currentRoom.row + 1 >= rows)
                            {
                                checkFail = true;
                            }
                            break;
                        }
                        case 1:
                        {
                            // check if the door tries to generate to an out of bounds room
                            if (currentRoom.collumn + 1 >= collumns)
                            {
                                checkFail = true;
                            }
                            break;
                        }
                        case 2:
                        {
                            // check if the door tries to generate to an out of bounds room
                            if (currentRoom.row - 1 < 0)
                            {
                                checkFail = true;
                            }
                            break;

                        }
                        case 3:
                        {
                            // check if the door tries to generate to an out of bounds room
                            if (currentRoom.collumn - 1 < 0)
                            {
                                checkFail = true;
                            }
                            break;
                        }
                    }

                    // if the check fail force no door 
                    if (checkFail)
                    {
                        Console.WriteLine("checkFail");
                        currentRoom.doors[direction] = null;
                        continue;
                    }

                    DirectionConvertor(direction, currentRoom, out newRows, out newCollumns);
                   
                        
                    // 1. Check the UP Room of the target space 
                    if (newRows + 1 < rows && roomArray[newCollumns , newRows + 1] != null)
                    {
                        Room upRoom = roomArray[newCollumns, newRows + 1];
                        // If the UP Room tried to make a DOWN door (direction 2) and failed then the check fails 
                        if (upRoom.doorGenerationAttempt[2] == true && upRoom.doors[2] == null && upRoom != currentRoom)
                        {
                            Console.WriteLine("fail check upRoom");
                            checkFail = true;
                        }
                    }
                           
                        
                    // 2. Check the RIGHT Room of the target space 
                    if (newCollumns + 1 < collumns && roomArray[newCollumns + 1, newRows] != null)
                    {
                        Room rightRoom = roomArray[newCollumns + 1, newRows];
                        // If the RIGHT Room tried to make a LEFT door (direction 3) and failed then the check fails
                        if (rightRoom.doorGenerationAttempt[3] == true && rightRoom.doors[3] == null &&
                            rightRoom != currentRoom)

                        {
                            Console.WriteLine("fail check rightRoom");
                            checkFail = true;
                        }
                    }
                           
                        
                    // 3. Check the DOWN Room of the target space
                    if (newRows - 1 >= 0 && roomArray[newCollumns, newRows - 1] != null)
                    {
                        Room downRoom = roomArray[newCollumns, newRows - 1];
                        // If the DOWN Room tried to make an UP door (direction 0) and failed then the check fails 
                        if (downRoom.doorGenerationAttempt[0] == true && downRoom.doors[0] == null &&
                            downRoom != currentRoom)
                        {
                            Console.WriteLine("fail check downRoom");
                            checkFail = true; 
                        }
                    }
                           
                        
                    // 4. Check the LEFT Room of the target space 
                    if (newCollumns - 1 >= 0 && roomArray[newCollumns - 1, newRows] != null)
                    {
                        Room leftRoom = roomArray[newCollumns - 1, newRows];
                        // If the LEFT Room tried to make a RIGHT door (direction 1) and failed then the check fails 
                        if (leftRoom.doorGenerationAttempt[1] == true && leftRoom.doors[1] == null &&
                            leftRoom != currentRoom)
                        {
                            Console.WriteLine("fail check left");
                            checkFail = true;
                        }
                    }
                           
                        
                    
                    // If ANY Room has a solid wall pointing at this spot, cancel the room 
                    if (checkFail)
                    {
                        currentRoom.doors[direction] = null;
                        continue;
                    }

                    if (roomArray[newCollumns, newRows] != null)
                    {
                        Room adjacentRoom = roomArray[newCollumns, newRows];

                        // Create the door on the current room 
                        currentRoom.AddDoor(TransferIntToDirection(direction), true);

                        // Create the matching opposite door on the existing room to close the loop 
                        int oppositeDirection = (direction + 2) % 4;
                        adjacentRoom.AddDoor(TransferIntToDirection(oppositeDirection), true);

                        continue; // Skip creating a new room since it's already there 
                    }

                    if (currentRoomAmount == roomAmount)
                    {
                        currentRoom.doors[direction] = null;
                        dequeueRoom = true;
                        continue;
                    }
                    
                    if (!currentRoom.isStartRoom) // checks if the current room is not a starting room 
                    {
                        // makes a random calculation to see if there will be a door here if successful add a door to the current direction 
                        if (rnd.Next(5) == 1) currentRoom.AddDoor(TransferIntToDirection(direction), true);
                        else currentRoom.AddDoor(TransferIntToDirection(direction), false);
                    }
                    else
                    {
                        // makes a random calculation to see if there will be a door here if successful add a door to the current direction 
                        // (more forgiving becuse it is a starting room) 
                        if (rnd.Next(3) >= 1) currentRoom.AddDoor(TransferIntToDirection(direction),true);
                        else currentRoom.AddDoor(TransferIntToDirection(direction), false);
                    }

                    //if there is no door no need for the next logic 
                    if (currentRoom.doors[direction] == null)
                    {
                        continue;

                    }

                    DirectionConvertor(direction, currentRoom, out newRows, out newCollumns);
                    roomQueue.Enqueue(currentRoom);
                    // create a new room based on the current difficulty, array, and makes the entrance direction the inverted of this direction 
                    currentRoom = new Room
                        (player, "FloorOne",difficulty, newRows, newCollumns,grideCenter ,TransferIntToDirection((direction + 2) % 4));
                    roomArray[newCollumns, newRows] = currentRoom;
                    currentRoomAmount++;
                    generatedRoom = true;
                    

                

                   
                }

                if (!generatedRoom)
                {
                    dequeueRoom = true;
                }
                generatedRoom = false;

                

            }
            
            if (roomQueue.Count == 0 && currentRoomAmount < roomAmount)
            {
                Console.WriteLine("not enough rooms generated restarting generation ");
                continue;
            }
            bool lockedRoomAssigned = false;
            foreach (Room room in roomArray) // checks for end rooms and marks them 
            {
                if (room == null) continue;
                int doorCount = 0;
                int doorDirectionIndex = -1;
    
                for (int i = 0; i < 4; i++)
                {
                    if (room.doors[i] != null)
                    {
                        doorCount++;
                        doorDirectionIndex = i; // Store the index of the single door
                    }
                }

                if (!room.isStartRoom && doorCount == 1)
                {
                    if (!lockedRoomAssigned)
                    {
                        room.ChangeRoomType(Room.RoomType.LockedEndRoom);
            
                        // ONLY lock the adjacent room's side (the outside entrance),
                        // leaving the door inside the locked room completely unlocked.
                        if (doorDirectionIndex != -1)
                        {
                            DirectionConvertor(doorDirectionIndex, room, out int neighborRow, out int neighborCol);
                            Room adjacentRoom = GetRoomAt(neighborCol, neighborRow);
                
                            if (adjacentRoom != null)
                            {
                                int oppositeDirection = (doorDirectionIndex + 2) % 4;
                                if (adjacentRoom.doors[oppositeDirection] != null)
                                {
                                    adjacentRoom.doors[oppositeDirection].Lock();
                                }
                            }
                        }
            
                        lockedRoomAssigned = true;
                    }
                    else
                    {
                        room.ChangeRoomType(Room.RoomType.EndRoom);
                    }
                    endRoomQueue.Enqueue(room);
                }
            }

            if (endRoomQueue.Count < 3)
            {
                Console.WriteLine("not enough end rooms generated restarting generation ");
                continue;
            }
            generationSuccessful = true;
        }

        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                room.FinnishedRoomGeneration();
            }
        }
        PrintRoomArray();
        return this;
    }

    private void DirectionConvertor(int direction, Room currentRoom, out int row, out int collumn) // direction calculations
    {
        
        row = -1;
        collumn = -1;
        switch (direction)
        {
            case 0: // up
            {
                row = currentRoom.row + 1;
                collumn = currentRoom.collumn;
                break;
            }
            case 1: // right
            {
                row  = currentRoom.row;
                collumn = currentRoom.collumn + 1;
                break;
            }
            case 2: // down
            {
                row = currentRoom.row - 1;
                collumn = currentRoom.collumn;
                break;
            }
            case 3: // left
            {
                row = currentRoom.row;
                collumn = currentRoom.collumn - 1;
                break;
            }
        }
    }
    
    // calculate the room amount and grid size based on the floor difficulty
    private void CalculateBasedOnDiffiulty(Difficulty difficulty, out Vector2 gridCenter) 
    {
        Random rnd = new Random();
        Vector2 grideCenter;
        switch (difficulty)
        {
            case Difficulty.Easy: // easy difficulty, room amount between 5 - 8, grid size 11
            {
                roomAmount = rnd.Next(5, 8);
                collumns = rows = 11;
                break;
            }
            case Difficulty.Medium: // medium difficulty, room amount between 8 - 10, grid size 13
            {
                roomAmount = rnd.Next(8, 10);
                collumns = rows = 13;
                break;
            }
            case Difficulty.Hard: // hard difficulty, room amount between 10 - 15, grid size 15
            {
                roomAmount = rnd.Next(10, 15);
                collumns = rows = 15;
                break;
            }
        }
        grideCenter.X = rows * 0.5f;
        grideCenter.Y = collumns * 0.5f;
        roomArray = new Room[collumns, rows]; // sets the grid size to the chosen size
        Console.WriteLine($"roomAmount:{roomAmount}, rows:{rows} , collumns:{collumns}");


        gridCenter = grideCenter;
    }
        
    public Room GetRoomAt(int col, int row)
    {
        // Check if the requested grid coordinates are inside the array boundaries
        if (col >= 0 && col < collumns && row >= 0 && row < rows)
        {
            return roomArray[col, row];
        }
        return null; // Room does not exist here
    }

    public Direction TransferIntToDirection(int direction)
    {
        switch (direction)
        {
            case 0:
                return Direction.Up;
            case 1:
                return Direction.Right;
            case 2:
                return Direction.Down;
            case 3:
                return Direction.Left;
        }
        return  Direction.None;
    }

    public void PrintRoomArray() // prints the room to the  consule for testing
    {
        Console.WriteLine($"floorArray:");
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < collumns; y++)
            {
                if (roomArray[x, y] != null) Console.Write(roomArray[x,y] + " ");
                else
                {
                    Console.Write("    " + " ");
                }
                
            }
            Console.WriteLine();

        }
    }

    public List<Sprite> GetRoomSprites()
    {
        List<Sprite> spriteList = new List<Sprite>();
        foreach (Room room in roomArray)
        {
            if (room != null)
            {
                spriteList.Add(room);
            }
        }

        return spriteList;
    }
    
    public  override string ToString() // overrides the to string function to show the room amount and aray size
    {
        return $"roomAmmount: {roomAmount}, roomArray size: {collumns}, {rows}";
    }
}
