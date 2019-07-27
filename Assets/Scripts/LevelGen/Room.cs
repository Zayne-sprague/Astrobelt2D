using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North, East, South, West
}

public class Room : MonoBehaviour
{

    public int xPos;
    public int yPos;
    public int width;
    public int height;
    public Direction enteringCorridor;

    public void SetupRoom(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        width = widthRange.Random;
        height = heightRange.Random;

        // Creates the room right in the center of the map
        xPos = Mathf.RoundToInt(rows / 2f - width / 2f);
        yPos = Mathf.RoundToInt(rows / 2f - height / 2f);

        enteringCorridor = Direction.North;
    }

    public void SetupRoom(IntRange widthRange, IntRange heightRange, Corridor corridor, int columns, int rows, int corridor_padding = 2)
    {

        enteringCorridor = corridor.direction;

        width = widthRange.Random;
        height = heightRange.Random;

        switch (corridor.direction)
        {
            case Direction.North:
                height = Mathf.Clamp(height, 1, rows - corridor.EndPositionY);
                yPos = corridor.EndPositionY;
                xPos = Random.Range(corridor.EndPositionX - width + 1 + corridor_padding, corridor.EndPositionX - corridor_padding);
                xPos = Mathf.Clamp(xPos, 0, columns - width);
                break;
            case Direction.East:
                width = Mathf.Clamp(width, 1, columns - corridor.EndPositionX);
                xPos = corridor.EndPositionX;

                yPos = Random.Range(corridor.EndPositionY - height + 1 + corridor_padding, corridor.EndPositionY - corridor_padding);
                yPos = Mathf.Clamp(yPos, 0, rows - height);
                break;
            case Direction.South:
                height = Mathf.Clamp(height, 1, corridor.EndPositionY);
                yPos = corridor.EndPositionY - height;

                xPos = Random.Range(corridor.EndPositionX - width + 1 + corridor_padding, corridor.EndPositionX - corridor_padding);
                xPos = Mathf.Clamp(xPos, 0, columns - width);
                break;
            case Direction.West:
                width = Mathf.Clamp(width, 1, corridor.EndPositionX);
                xPos = corridor.EndPositionX - width;

                yPos = Random.Range(corridor.EndPositionY - height + 1 + corridor_padding, corridor.EndPositionY - corridor_padding);
                yPos = Mathf.Clamp(yPos, 0, rows - height);
                break;

        }
    }

    public bool collision(int txPos, int tyPos, int twidth, int theight, int edge_padding = 1) 
    {
        int t_EndXPos = txPos + twidth - 1 + edge_padding;
        int t_EndYPos = tyPos + theight - 1 + edge_padding;

        int xEndPos = xPos + width - 1;
        int yEndPos = yPos + height - 1;

        bool overlap_left = xPos < t_EndXPos && xEndPos  > txPos;
        bool overlap_right = xEndPos > txPos && xPos < t_EndXPos;
        bool overlap_top = yPos < t_EndYPos && yEndPos > tyPos;
        bool overlap_bottom = yEndPos > tyPos && yPos < t_EndYPos;

        int overlaps = ((overlap_top || overlap_bottom ? 1 : 0) + (overlap_left || overlap_right ? 1 : 0));

        // If two or more overlap that means we have an x and y overlap meaning they are ontop of one another.
        if (overlaps > 1)
        {
            return true;
        }

        return false;
    }


    public bool collision(Room room, int edge_padding = 1)
    {

        int t_EndXPos = room.xPos + room.width - 1 + edge_padding;
        int t_EndYPos = room.yPos + room.height - 1 + edge_padding;

        int xEndPos = xPos + width - 1;
        int yEndPos = yPos + height - 1;

        bool overlap_left = xPos < t_EndXPos && xEndPos > room.xPos;
        bool overlap_right = xEndPos > room.xPos && xPos < t_EndXPos;
        bool overlap_top = yPos < t_EndYPos && yEndPos > room.yPos;
        bool overlap_bottom = yEndPos > room.yPos && yPos < t_EndYPos;

        int overlaps = ((overlap_top || overlap_bottom ? 1 : 0) + (overlap_left || overlap_right ? 1 : 0));

        // If two or more overlap that means we have an x and y overlap meaning they are ontop of one another.
        if (overlaps > 1)
        {
            return true;
        }

        return false;
    }

}
