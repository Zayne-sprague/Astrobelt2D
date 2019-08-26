using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public int number_of_goals = 2;
    private CorridorGoal[] goals;
    private int head_of_goals;

    public void buildOut()
    {
        goals = new CorridorGoal[number_of_goals];
        head_of_goals = 0;
    }

    public void SpawnGoal(Corridor[] corridors, CorridorGoal prefab, int edge_padding = 1)
    {
        for (int i = 0; i < corridors.Length; i++)
        {
            SpawnGoal(corridors[i], prefab, edge_padding);
        }
    }

    public void SpawnGoal(Corridor corridor, CorridorGoal prefab, int edge_padding = 1)
    {
        float offset_from_edge = 0.5f;
        CorridorGoal inst;

        float length = (corridor.corridorLength - 1) - offset_from_edge;
        float x = corridor.x;
        float y = corridor.y;

        head_of_goals = (head_of_goals + 1) % number_of_goals;

        if (goals[head_of_goals] != null)
        {
            Destroy(goals[head_of_goals].gameObject);
        }

        switch (corridor.direction)
        {
            case Direction.North:
                inst = Instantiate(prefab, new Vector3(x, (float)(y + length), 0), Quaternion.Euler(0, 0, 90));
                inst.BuildCollisionBox(1 , 1 + (edge_padding * 2), 0);

                break;
            case Direction.East:
                inst = Instantiate(prefab, new Vector3((float)(x + length), y, 0), Quaternion.identity);
                inst.BuildCollisionBox(1, 1 + (edge_padding * 2), 0);
                break;
            case Direction.South:
                inst = Instantiate(prefab, new Vector3(x, (float)(y + offset_from_edge), 0), Quaternion.Euler(0, 0, -90));
                inst.BuildCollisionBox(1 , 1 + (edge_padding * 2), 0);

                break;
            default: //west
                inst = Instantiate(prefab, new Vector3((float)(x + offset_from_edge), corridor.y, 0), Quaternion.Euler(0, 0, 180));
                inst.BuildCollisionBox(1, 1 + (edge_padding * 2), 0);

                break;
        }

        goals[head_of_goals] = inst;

    }

}
