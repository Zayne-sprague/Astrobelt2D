﻿/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction_TEMP
{
    North, East, South, West
}

public class Room_TEMP : MonoBehaviour
{

    public int xPos;
    public int yPos;
    public int roomWidth;
    public int roomHeight;
    public Direction enteringCorridor;

    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;
        xPos = Mathf.RoundToInt(rows / 2f - roomHeight / 2f);
        yPos = Mathf.RoundToInt(rows / 2f - roomHeight / 2f);
    }

    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows, Corridor corridor)
    {
        enteringCorridor = corridor.direction;

        roomWidth = widthRange.Random;
        roomHeight = heightRange.Random;

        int offset = 2;

        switch (corridor.direction)
        {
            case Direction.North:
                roomHeight = Mathf.Clamp(roomHeight, 1, rows - corridor.EndPositionY);
                yPos = corridor.EndPositionY;
                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1 + offset, corridor.EndPositionX - offset);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.East:
                roomWidth = Mathf.Clamp(roomWidth, 1, columns - corridor.EndPositionX);
                xPos = corridor.EndPositionX;

                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1 + offset, corridor.EndPositionY - offset);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;
            case Direction.South:
                roomHeight = Mathf.Clamp(roomHeight, 1, corridor.EndPositionY);
                yPos = corridor.EndPositionY - roomHeight + 1;

                xPos = Random.Range(corridor.EndPositionX - roomWidth + 1 + offset, corridor.EndPositionX - offset);
                xPos = Mathf.Clamp(xPos, 0, columns - roomWidth);
                break;
            case Direction.West:
                roomWidth = Mathf.Clamp(roomWidth, 1, corridor.EndPositionX);
                xPos = corridor.EndPositionX - roomWidth + 1;

                yPos = Random.Range(corridor.EndPositionY - roomHeight + 1 + offset, corridor.EndPositionY - offset);
                yPos = Mathf.Clamp(yPos, 0, rows - roomHeight);
                break;

        }
    }

}
*/