using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelCreator : MonoBehaviour
{

    [SerializeField] int rows;
    [SerializeField] int cols;

    [SerializeField] int total_rooms;


    [SerializeField] public int minRoomWidth = 3;
    [SerializeField] public int maxRoomWidth = 10;

    public IntRange roomWidth;

    [SerializeField] public int minRoomHeight = 3;
    [SerializeField] public int maxRoomHeight = 10;
    public IntRange roomHeight;

    [SerializeField] public int minCorridorLength = 1;
    [SerializeField] public int maxCorridorLength = 2;
    public IntRange corridorLength;

    [SerializeField] public int edge_padding = 1;
    [SerializeField] public int corridor_padding = 1;

    [SerializeField] public float time_to_collapse = 5f;

    [SerializeField] GameObject wall_tile;
    [SerializeField] GameObject room_tile;
    [SerializeField] GameObject corridor_tile;
    [SerializeField] GameObject background_tile;

    [SerializeField] CorridorGoal corridor_goal;

    [SerializeField] FlyingAsteroid flying_roids;

    [SerializeField] Coin coin_prefab;

    // Debug
    [SerializeField] public bool debug = false;
    [SerializeField] public float debug_Timer = 0.5f;
    private float debug_current_timer = 0;

    private GameObject board_holder;
    private GameObject background_holder;

    private GameObject[][] tiles;
    private Room[] rooms;
    private Corridor[] corridors;
    private ItemGenerator item_generator;

    private int last_created_room = 0;
    private int last_created_corridor = 0;

    private IntRange place_star = new IntRange(0, 100);
    private bool creatingRoom = false;

    private void Start()
    {
        // Initialize the board
        InitializeBoard();

        // Build out the board
        BuildOut();
    }

    private void InitializeBoard()
    {
        // Fill the tiles array with empty game objects that we will fill later.
        tiles = new GameObject[cols][];
        for (int i = 0; i < rows; i++)
        {
            tiles[i] = new GameObject[rows];
        }

        // Initialize the rooms and corridors array, corridors will be 1 - total # of rooms.
        rooms = new Room[total_rooms];
        corridors = new Corridor[total_rooms - 1];
    }

    public void BuildOut()
    { 
        // Don't attempt to create more rooms during initialization 
        creatingRoom = true;

        roomWidth = new IntRange(minRoomWidth, maxRoomWidth);
        roomHeight = new IntRange(minRoomHeight, maxRoomHeight);
        corridorLength = new IntRange(minCorridorLength, maxCorridorLength);

        // Makes all the items yo
        item_generator = new ItemGenerator();
        item_generator.number_of_goals = total_rooms - 1;
        item_generator.buildOut();

        //Holds all tiles
        board_holder = new GameObject("Board Holder");
        background_holder = new GameObject("Background Holder");

        // Fill board with wall tiles
        fillOutBoard();

        // Initial room and corridor creation
        CreateRoom(0, -1);
        CreateCorridor(0, 0);
        CreateRoom(1, 0);
        CreateCorridor(1, 1);
        CreateRoom(2, 1);

        last_created_room = 2;
        last_created_corridor = 1;

        // You can create more rooms now
        creatingRoom = false;
    }

    public Room AddARoom()
    {
        if (creatingRoom) { return rooms[last_created_room]; }
        creatingRoom = true;

        int next_room_index = (last_created_room + 1) % total_rooms;
        int next_corridor_index = (last_created_corridor + 1) % (total_rooms - 1);

        fillInRect(rooms[next_room_index], wall_tile);
        StartCoroutine(fillInRect(corridors[next_corridor_index], wall_tile, 0, 0, true));

        CreateCorridor(next_corridor_index, last_created_room);
        CreateRoom(next_room_index, next_corridor_index);

        last_created_room = next_room_index;
        last_created_corridor = next_corridor_index;

        creatingRoom = false;

        return rooms[next_room_index];
    }

    private void CreateRoom(int roomIndex, int corridorIndex)
    {

        if (corridorIndex == -1)
        {
            rooms[0] = new Room();
            rooms[0].SetupRoom(roomWidth, roomHeight, cols, rows);
        }
        else
        {
            rooms[roomIndex] = new Room();
            rooms[roomIndex].SetupRoom(roomWidth, roomHeight, corridors[corridorIndex], cols, rows);
        }

        // This works with first and other rooms, since first only matters due to overriden constructor.
        fillInRect(rooms[roomIndex], room_tile);


    }

    private void CreateCorridor(int corridorIndex, int roomIndex)
    {

        corridors[corridorIndex] = new Corridor();
        corridors[corridorIndex].SetupCorridor(rooms[roomIndex], rooms, corridorLength, roomWidth, roomHeight, cols, rows, edge_padding);

        item_generator.SpawnGoal(corridors[corridorIndex], corridor_goal, edge_padding);
        StartCoroutine(fillInRect(corridors[corridorIndex], corridor_tile));

    }

    /* Lay tiles */

    private void fillOutBoard()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (place_star.Random > 90)
                {
                    createPermanentTile(x, y, background_tile);
                }
                createTile(x, y, wall_tile);
            }
        }
    }

    private void fillInRect(Room room, GameObject prefab, int xOffset = 0, int yOffset = 0)
    {
        fillInRect(room.xPos, room.yPos, room.width, room.height, prefab, xOffset, yOffset);
    }




    private void fillInRect(int xPos, int yPos, int width, int height, GameObject prefab, int xOffset = 0, int yOffset = 0)
    {

        for (int x = xPos - xOffset; x < xPos + width + xOffset; x++)
        {
            for (int y = yPos - yOffset; y < yPos + height + yOffset; y++)
            {
                createTile(x, y, prefab);
            }
        }
        

    }


    /* Tile constructors */

    private void createTile(int xPos, int yPos, GameObject prefab)
    {
        // Before we create a tile - we have to destroy anything that was there previously.
        destroyTile(xPos, yPos);

        float xCoord = (float) xPos;
        float yCoord = (float) yPos;

        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        GameObject tileInstance = Instantiate(prefab, position, Quaternion.identity);

        tiles[xPos][yPos] = tileInstance;
        tileInstance.transform.parent = board_holder.transform;
    }

    private void createPermanentTile(int xPos, int yPos, GameObject prefab)
    {
        // Before we create a tile - we have to destroy anything that was there previously.
        destroyTile(xPos, yPos);

        float xCoord = (float)xPos;
        float yCoord = (float)yPos;

        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        GameObject tileInstance = Instantiate(prefab, position, Quaternion.identity);

        tileInstance.transform.parent = background_holder.transform;
    }

    private void destroyTile(int xPos, int yPos)
    {
        if (tiles[xPos][yPos] != null)
            Destroy(tiles[xPos][yPos]);
    }


    /* v ROIDS! v */

    public void spawn_roids(IntRange number_of_roids_per_room)
    {

        // For each room 
        for (int i = 0; i < rooms.Length; i++)
        {
            // spawn a random amount of asteroids
            for (int x = 0; x < number_of_roids_per_room.Random; x++)
            {
                FlyingAsteroid roid = Instantiate(flying_roids);
                roid.BuildOut(rooms[i]);

            }
        }

    }

    /* ^ ROIDS! ^ */

    /* v COINS! v */

    public void spawn_coins(Room room, int value)
    {
        int x = (new IntRange(room.xPos, room.xPos + room.width)).Random;
        int y = (new IntRange(room.yPos, room.yPos + room.height)).Random;

        Coin _coin = Instantiate(coin_prefab, new Vector3(x, y, 0), Quaternion.identity);
        _coin.value = value;

        //_coin.buildOut();
    }

    /* ^ COINS! ^ */


    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            debug_current_timer += Time.deltaTime;

            if (debug_Timer < debug_current_timer)
            {
                debug_current_timer = 0f;

                AddARoom();

            }
        }
    }


    // * fill in a corridor blehg * //
    private IEnumerator fillInRect(Corridor corridor, GameObject prefab, int xOffset = 0, int yOffset = 0, bool collapse_slowly = false)
    {
        int xPos = corridor.x;
        int yPos = corridor.y;
        int width = corridor.width;
        int height = corridor.height;

        // Add the offset to make corridors wider if needed.
        if (corridor.direction == Direction.East)
        {
            yOffset += corridor_padding;

            for (int x = xPos - xOffset; x < xPos + width + xOffset; x++)
            {
                float sec = 0;


                while (sec < (time_to_collapse / width) && collapse_slowly)
                {
                    if (tiles[xPos + width + 1][yPos].name == "WallTile(Clone)") { collapse_slowly = false; }

                    sec += Time.deltaTime;
                    yield return 0;
                }

                for (int y = yPos - yOffset; y < yPos + height + yOffset; y++)
                {

                    createTile(x, y, prefab);

                }
            }



        }
        else if (corridor.direction == Direction.West)
        {
            yOffset += corridor_padding;

           

            for (int x = xPos + width + xOffset - 1; x >= xPos - xOffset; x--)
            {
                float sec = 0;

                while (sec < (time_to_collapse / width) && collapse_slowly)
                {
                    if (tiles[xPos - 1][yPos].name == "WallTile(Clone)") { collapse_slowly = false; }

                    sec += Time.deltaTime;
                    yield return 0;
                }

                for (int y = yPos - yOffset; y < yPos + height + yOffset; y++)
                {
                    createTile(x, y, prefab);
                }
            }
        }
        else if (corridor.direction == Direction.North)
        {
            xOffset += corridor_padding;

            for (int y = yPos - yOffset; y < yPos + height + yOffset; y++)
            {
                float sec = 0;

                while (sec < (time_to_collapse / height) && collapse_slowly)
                {
                    if (tiles[xPos][yPos + height + 1].name == "WallTile(Clone)") { collapse_slowly = false; }

                    sec += Time.deltaTime;
                    yield return 0;
                }

                for (int x = xPos + width + xOffset - 1; x >= xPos - xOffset; x--)
                {

                    createTile(x, y, prefab);
                }
            }

        }
        else
        {
            xOffset += corridor_padding;

            for (int y = yPos + height + yOffset - 1; y >= yPos - yOffset; y--)
            {
                float sec = 0;

                while (sec < (time_to_collapse / height) && collapse_slowly)
                {
                    if (tiles[xPos][yPos - 1].name == "WallTile(Clone)") { collapse_slowly = false; }

                    sec += Time.deltaTime;
                    yield return 0;
                }

                for (int x = xPos + width + xOffset - 1; x >= xPos - xOffset; x--)
                {

                    createTile(x, y, prefab);
                }
            }
        }

        


    }
}

