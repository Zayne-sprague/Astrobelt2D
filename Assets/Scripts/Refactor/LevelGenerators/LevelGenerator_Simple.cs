using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator_Simple : MonoBehaviour
{

    [SerializeField] AsteroidRoom room;
    [SerializeField] int roomsRendered = 3;
    private int totalRoomsRendered;
    private AsteroidRoom[] rooms;
    private int head = -1;

    [SerializeField] float wallWidth = 3.1f;


    [SerializeField] public bool debug = false;
    [SerializeField] public float debugRefresh = 2f;
    private float debugCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        buildOut();
    }

    void buildOut()
    {
        totalRoomsRendered = roomsRendered + 1;
        rooms = new AsteroidRoom[totalRoomsRendered];
        head = -1;
        for (int i = 0; i < roomsRendered; i++)
        {
            createRoom(true);
        }

    }

    public void createRoom(bool skipDelete=false)
    {
        int entry;
        int goal = -1;
        Vector3 pos;

        if (head == -1)
        {
            pos = transform.position;
            entry = -1;
            goal = Random.Range(0, 3);
        }
        else
        {
            entry = (rooms[head].goalWall + 2) % 4;

            print(" " + rooms[head].goalWall + " : " + entry);
            while (goal == -1 || goal == entry)
            {
                goal = Random.Range(0, 3);
            }

            pos = getNextRoomPosition(entry);

        }


        AsteroidRoom new_room = Instantiate(room, pos, Quaternion.identity);
        new_room.transform.parent = transform;

        new_room.debug = debug;
        new_room.debugRefresh = debugRefresh;

        new_room.goalWall = goal;
        new_room.entryWall = entry;
        new_room.roomWallWidth = wallWidth; 

        new_room.buildOut();

        head = (head + 1) % (totalRoomsRendered);

        if (rooms[head])
        {
            print("DELETE THE ROOM : " + head);
            Destroy(rooms[head].gameObject);
        }

        rooms[head] = new_room;

    }

    Vector3 getNextRoomPosition(int entry)
    {
        Vector3 head_transform = rooms[head].transform.position;
        float room_width = rooms[head].width / 2 + wallWidth + wallWidth;
        float room_height = rooms[head].height / 2 + wallWidth + wallWidth;
         
        //TOP, RIGHT, BOTTOM, LEFT
        if (entry == 0)
        {
            return new Vector3(head_transform.x, head_transform.y - room_height);
        }else if (entry == 1)
        {
            return new Vector3(head_transform.x - room_width, head_transform.y);
        }else if (entry == 2)
        {
            return new Vector3(head_transform.x, head_transform.y + room_height);
        }
        else
        {
            return new Vector3(head_transform.x + room_width, head_transform.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            debugCounter += Time.deltaTime;

            if (debugRefresh < debugCounter)
            {
                debugCounter = 0f;

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                buildOut();
            }
        }
    }
}
