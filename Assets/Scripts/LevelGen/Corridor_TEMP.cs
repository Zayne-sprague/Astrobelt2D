/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor2 : MonoBehaviour
{
    public int startXPos;
    public int startYPos;
    public int corridorLength;
    public Direction direction;
    public CorridorGoal goal_gameobject;

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
                return startXPos + corridorLength - 1;
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
                return startYPos + corridorLength - 1;
            }
            return startYPos - corridorLength + 1;
        }
    }

    public void SetupCorridor(Room room, IntRange length, IntRange roomWidth, IntRange roomHeight, int columns, int rows, bool firstCorridor)
    {
        direction = (Direction)Random.Range(0, 4);
        Direction oppositeDirection = (Direction)(((int)room.enteringCorridor + 2) % 4);

        if (!firstCorridor && direction == oppositeDirection)
        {
            int directionInt = (int)direction;
            directionInt = directionInt + Random.Range(1, 4);
            directionInt = directionInt % 4;
            direction = (Direction)directionInt;
        }

        corridorLength = length.Random;

        int maxLength = length.m_Max;

        int offset = 2;
        float grid_offset_for_goal = 0.5f;

        CorridorGoal goal;

        switch (direction)
        {
            case Direction.North:
                startXPos = Random.Range(room.xPos + offset, room.xPos + room.roomWidth - 1 - offset);
                startYPos = room.yPos + room.roomHeight;
                maxLength = rows - startYPos - roomHeight.m_Min;
                corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);

                //Spawn Goal//
                goal = Instantiate(goal_gameobject, new Vector3(startXPos + grid_offset_for_goal, startYPos + corridorLength / 2, 0), Quaternion.identity);
                goal.BuildCollisionBox(3, corridorLength);

                break;
            case Direction.East:
                startXPos = room.xPos + room.roomWidth;
                startYPos = Random.Range(room.yPos + offset, room.yPos + room.roomHeight - 1 - offset);
                maxLength = columns - startXPos - roomWidth.m_Min;
                corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);

                //Spawn Goal//
                goal = Instantiate(goal_gameobject, new Vector3(startXPos + corridorLength / 2, startYPos + grid_offset_for_goal, 0), Quaternion.identity);
                goal.BuildCollisionBox(corridorLength, 3);

                break;
            case Direction.South:
                startXPos = Random.Range(room.xPos + offset, room.xPos + room.roomWidth - offset);
                startYPos = room.yPos;
                maxLength = startYPos - roomHeight.m_Min;
                corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);

                //Spawn Goal//
                goal = Instantiate(goal_gameobject, new Vector3(startXPos + grid_offset_for_goal, startYPos - corridorLength / 2, 0), Quaternion.identity);
                goal.BuildCollisionBox(3, corridorLength);

                break;
            case Direction.West:
                startXPos = room.xPos;
                startYPos = Random.Range(room.yPos + offset, room.yPos + room.roomHeight - offset);
                maxLength = startXPos - roomWidth.m_Min;
                corridorLength = Mathf.Clamp(corridorLength, 1, maxLength);

                //Spawn Goal//
                goal = Instantiate(goal_gameobject, new Vector3(startXPos - corridorLength / 2, startYPos + grid_offset_for_goal, 0), Quaternion.identity);
                goal.BuildCollisionBox(corridorLength, 3);

                break;
        }









    }
}
*/