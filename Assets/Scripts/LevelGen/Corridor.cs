using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{

    public int startXPos;
    public int startYPos;
    public int corridorLength;
    public Direction direction;

    // Might have to rethink how to pluck values out of an array
    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            // moving elements downwards, to fill the gap at [index]
            arr[a] = arr[a + 1];
        }
        // finally, let's decrement Array's size by one
        System.Array.Resize(ref arr, arr.Length - 1);
    }


    public int EndPositionX
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return startXPos;
            }
            if (direction == Direction.East)
            {
                return startXPos + corridorLength;
            }
            return startXPos - corridorLength + 1;
        }
    }

    public int EndPositionY
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
            {
                return startYPos;
            }
            if (direction == Direction.North)
            {
                return startYPos + corridorLength;
            }
            return startYPos - corridorLength + 1;
        }
    }

    public int x
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return startXPos;
            }
            if (direction == Direction.West)
            {
                return EndPositionX;
            }
            return startXPos;
        }
    }

    public int y
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
            {
                return startYPos;
            }
            if (direction == Direction.South)
            {
                return EndPositionY;
            }
            return startYPos;
        }
    }

    public int width
    {
        get
        {
            if (direction == Direction.North || direction == Direction.South)
            {
                return 1;
            }
     
            return corridorLength;
        }
    }

    public int height
    {
        get
        {
            if (direction == Direction.East || direction == Direction.West)
            {
                return 1;
            }

            return corridorLength;
        }
    }

    public void SetupCorridor(Room room, Room[] rooms, IntRange length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, int edge_padding = 1, bool firstCorridor = false)
    {
        // Find the opposite direction
        int oppositeDirection = (((int)room.enteringCorridor + 2) % 4);

        // Add all directions that aren't in the opposite direction to an array of possible directions
        Direction[] dirs = new Direction[] { (Direction)((oppositeDirection + 1) % 4), (Direction)((oppositeDirection + 2) % 4), (Direction)((oppositeDirection + 3) % 4) };

        // Iterate through our rooms and directions to see if we can place a room, if not increment the size of the corridor by 2 and repeat
        loopPossibleGenerations(room, rooms, dirs, length.Random, roomWidth, roomHeight, columns, rows, edge_padding, 2);


    }

    private void loopPossibleGenerations(Room room, Room[] rooms, Direction[] possible_directions, int length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, int edge_padding = 1, int len_incrementer = 2)
    {
        int xPos, yPos, max_length;
        bool found = false;
        Direction[] original_dirs = possible_directions;
        int distance_from_edge = 10;

        if (length > columns || length > rows)
        {
            throw (new System.Exception("Failed to find a reasonably lengthed corridor"));
        }

        //print("attempt to find a direction with length: " + length);

        do
        {

            //print("IN LOOP");

            // Pick a direction and remove it from the array so you don't re-pick it.
            int index = Random.Range(0, possible_directions.Length);
            Direction dir = possible_directions[index];
            RemoveAt(ref possible_directions, index);


            switch (dir)
            {

                case Direction.North:

                    xPos = Random.Range(room.xPos + edge_padding, room.xPos + room.width - 1 - edge_padding);
                    yPos = room.yPos + room.height;
                    max_length = rows - yPos - roomHeight.m_Min - distance_from_edge;

                    if (length <= max_length && !check_for_collisions(rooms, xPos - roomWidth.m_Max, yPos, roomWidth.m_Max * 2, roomHeight.m_Max))
                    {
                        direction = dir;
                        corridorLength = length;
                        startXPos = xPos;
                        startYPos = yPos;
                        found = true;
                    }

                    break;
                case Direction.East:
                    xPos = room.xPos + room.width;
                    yPos = Random.Range(room.yPos + edge_padding, room.yPos + room.height - 1 - edge_padding);
                    max_length = columns - xPos - roomWidth.m_Min - distance_from_edge;

                    if (length <= max_length && !check_for_collisions(rooms, xPos, yPos - roomHeight.m_Max, roomWidth.m_Max, roomHeight.m_Max * 2))
                    {
                        direction = dir;
                        corridorLength = length;
                        startXPos = xPos;
                        startYPos = yPos;
                        found = true;

                    }


                    break;
                case Direction.South:
                    xPos = Random.Range(room.xPos + edge_padding, room.xPos + room.width - edge_padding);
                    yPos = room.yPos - 1;
                    max_length = yPos - roomHeight.m_Min - distance_from_edge;

                    if (length <= max_length && !check_for_collisions(rooms, xPos - roomWidth.m_Max, yPos, roomWidth.m_Max * 2, -roomHeight.m_Max))
                    {
                        direction = dir;
                        corridorLength = length;
                        startXPos = xPos;
                        startYPos = yPos;
                        found = true;

                    }

                    break;
                case Direction.West:
                    xPos = room.xPos - 1;
                    yPos = Random.Range(room.yPos + edge_padding, room.yPos + room.height - edge_padding);
                    max_length = xPos - roomWidth.m_Min - distance_from_edge;

                    if (length <= max_length && !check_for_collisions(rooms, xPos, yPos - roomHeight.m_Max, -roomWidth.m_Max, roomHeight.m_Max * 2))
                    {
                        direction = dir;
                        corridorLength = length;
                        startXPos = xPos;
                        startYPos = yPos;
                        found = true;

                    }

                    break;

            }

        }
        while (possible_directions.Length != 0);

        if (!found)
            loopPossibleGenerations(room, rooms, original_dirs, length + len_incrementer, roomWidth, roomHeight, columns, rows, edge_padding, len_incrementer);

    }

    private bool check_for_collisions (Room[] rooms, int xPos, int yPos, int width, int height)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i] != null && rooms[i].collision(xPos, yPos, width, height))
            {
                return true;
            }
        }
        return false;

    }
}
