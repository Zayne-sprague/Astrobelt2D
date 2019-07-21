using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelCreator : MonoBehaviour
{
    public enum TileType
    {
        AsteroidGenerator, CorridorTile, Room
    }

    [SerializeField] public CorridorGoal corridor_goal;

    [SerializeField] public int columns = 100;
    [SerializeField] public int rows = 100;

    [SerializeField] public int minRooms = 3;
    [SerializeField] public int maxRooms = 5;

    public IntRange numRooms;

    [SerializeField] public int minRoomWidth = 3;
    [SerializeField] public int maxRoomWidth = 10;

    public IntRange roomWidth;

    [SerializeField] public int minRoomHeight = 3;
    [SerializeField] public int maxRoomHeight = 10;
    public IntRange roomHeight;

    [SerializeField] public int minCorridorLength = 1;
    [SerializeField] public int maxCorridorLength = 2;
    public IntRange corridorLength;

    private TileType[][] tiles;
    private AsteroidGenerator[][] objs;

    private Room[] rooms;
    private Corridor[] corridors;
    private GameObject boardHolder;
    private GameObject roomAndCorridorHolder;

    private int headRoomIndex = 0;
    private int headCorridorIndex = 0;

    [SerializeField] public AsteroidGenerator asset;

    private bool creatingRoom = false;

    // Debug
    [SerializeField] public bool debug = false;
    [SerializeField] public bool debug_room_generation = false;
    [SerializeField] public bool show_debug_icons = false;
    [SerializeField] public float debug_Timer = 0.5f;
    private float debug_current_timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        boardHolder = new GameObject("BoardHolder");

        objs = new AsteroidGenerator[columns][];
        for (int i = 0; i < columns; i++)
        {
            objs[i] = new AsteroidGenerator[rows];
        }

        setUpTiles();
        CreateRoomsAndCorridors();
        SetTileValuesForRooms();
        SetTilesValuesForCorridors();
        InstantiateTiles();
    }

    void BuildOut()
    {
        Destroy(boardHolder);
        boardHolder = new GameObject("BoardHolder");

        objs = new AsteroidGenerator[columns][];
        for (int i = 0; i < columns; i++)
        {
            objs[i] = new AsteroidGenerator[rows];
        }

        setUpTiles();
        CreateRoomsAndCorridors();
        SetTileValuesForRooms();
        SetTilesValuesForCorridors();
        InstantiateTiles();
    }

    /* v ROOM GENERATION v */

    public void createNewRoom()
    {
        if (creatingRoom) { return; }
        creatingRoom = true;

        Room prevRoom = rooms[headRoomIndex];
        Corridor prevCorridor = corridors[headCorridorIndex];
        print(rooms.Length);
        headRoomIndex = (headRoomIndex + 1) % rooms.Length;
        headCorridorIndex = (headCorridorIndex + 1) % corridors.Length;
        
        // Fill oldest room and corridor
        fillRoom(rooms[headRoomIndex]);
        fillCorridor(corridors[headCorridorIndex]);

        //Create corridor to the new room from the last generated room.
        corridorLength = new IntRange(minCorridorLength, maxCorridorLength);

        Room tmp_room;
        Corridor tmp_corridor;

        int attempts = 0;
        int max_attempts = 300;

        do
        {

            if (attempts > 0)
            {
                print("relocation attempt");
            }

            if (attempts > max_attempts / 2)
            {
                corridorLength = new IntRange(minCorridorLength, maxCorridorLength * 3);
            }

            if (attempts > max_attempts * .85)
            {
                corridorLength = new IntRange(minCorridorLength, maxCorridorLength * 9);
            }

            tmp_corridor = new Corridor();
            tmp_corridor.goal_gameobject = corridor_goal;
            tmp_corridor.SetupCorridor(prevRoom, corridorLength, roomWidth, roomHeight, columns, rows, false);


            roomWidth = new IntRange(minRoomWidth, maxRoomWidth);
            roomHeight = new IntRange(minRoomHeight, maxRoomHeight);

            tmp_room = new Room();
            tmp_room.SetupRoom(roomWidth, roomHeight, columns, rows, tmp_corridor);

            attempts++;

        } while (checkRoomOverlap(tmp_room, tmp_corridor) && attempts < max_attempts);

        if (attempts == max_attempts)
        {
            print("relocation fail");
            throw (new Exception("FAIL"));
        }

        rooms[headRoomIndex] = tmp_room;
        corridors[headCorridorIndex] = tmp_corridor;

        emptyRoom(rooms[headRoomIndex]);
        emptyCorridor(corridors[headCorridorIndex]);

        creatingRoom = false;

    }

    bool checkRoomOverlap(Room room, Corridor corridor)
    {

        int xPos = room.xPos;
        int yPos = room.yPos;
        int xEndPos = xPos + room.roomWidth;
        int yEndPos = yPos + room.roomHeight;

        for(int i = 0; i < rooms.Length; i++)
        {
            Room tmp_room = rooms[i];

            int buffer = 1;

            int txPos = tmp_room.xPos - buffer;
            int tyPos = tmp_room.yPos - buffer;
            int txEndPos = txPos + tmp_room.roomWidth + buffer * 2;
            int tyEndPos = tyPos + tmp_room.roomHeight + buffer * 2;

            bool overlap_left = xPos < txEndPos && xEndPos > txPos;
            bool overlap_right = xEndPos > txPos && xPos < txEndPos;
            bool overlap_top = yPos < tyEndPos && yEndPos > tyPos;
            bool overlap_bottom = yEndPos > tyPos && yPos < tyEndPos;

            int overlaps = ((overlap_top || overlap_bottom ? 1 : 0) + (overlap_left || overlap_right ? 1 : 0));
            if ( overlaps > 1)
            {

                print("overlap left: (" + overlap_left + ") overlap right: (" + overlap_right +") overlap top: (" + overlap_top + ") overlap bottom: (" + overlap_bottom + ")");
                return true;
            }

        }

        return false;


    }

    void emptyRoom(Room room)
    {
        for (int i = room.xPos; i < room.xPos + room.roomWidth; i++)
        {
            for (int j = room.yPos; j < room.yPos + room.roomHeight; j++)
            {
                tiles[i][j] = TileType.Room;
                destroyATile(i, j);
                InstantiateFromArray(i, j, true, false);
      
            }
        }
    }

    void emptyCorridor(Corridor corridor)
    {
        if (corridor.direction == Direction.North)
        {
            for (int i = corridor.startYPos; i < corridor.EndPositionY; i++)
            {
                tiles[corridor.startXPos][i] = TileType.CorridorTile;
                tiles[corridor.startXPos + 1][i] = TileType.CorridorTile;
                tiles[corridor.startXPos - 1][i] = TileType.CorridorTile;

                destroyATile(corridor.startXPos, i);
                destroyATile(corridor.startXPos + 1, i);
                destroyATile(corridor.startXPos - 1, i);

                InstantiateFromArray(corridor.startXPos, i, false, true);
                InstantiateFromArray(corridor.startXPos + 1, i, false, true);
                InstantiateFromArray(corridor.startXPos - 1, i, false, true);

            }
        }

        if (corridor.direction == Direction.South)
        {
            for (int i = corridor.startYPos; i >= corridor.EndPositionY; i--)
            { 
                tiles[corridor.startXPos][i] = TileType.CorridorTile;
                tiles[corridor.startXPos + 1][i] = TileType.CorridorTile;
                tiles[corridor.startXPos - 1][i] = TileType.CorridorTile;

                destroyATile(corridor.startXPos, i);
                destroyATile(corridor.startXPos + 1, i);
                destroyATile(corridor.startXPos - 1, i);

                InstantiateFromArray(corridor.startXPos, i, false, true);
                InstantiateFromArray(corridor.startXPos + 1, i, false, true);
                InstantiateFromArray(corridor.startXPos - 1, i, false, true);
            }
        }

        if (corridor.direction == Direction.East)
        {
            for (int i = corridor.startXPos; i < corridor.EndPositionX; i++)
            {
                tiles[i][corridor.startYPos] = TileType.CorridorTile;
                tiles[i][corridor.startYPos - 1] = TileType.CorridorTile;
                tiles[i][corridor.startYPos + 1] = TileType.CorridorTile;

                destroyATile(i, corridor.startYPos);
                destroyATile(i, corridor.startYPos - 1);
                destroyATile(i, corridor.startYPos + 1);

                InstantiateFromArray(i, corridor.startYPos, false, true);
                InstantiateFromArray(i, corridor.startYPos + 1, false, true);
                InstantiateFromArray(i, corridor.startYPos - 1, false, true);

            }
        }

        if (corridor.direction == Direction.West)
        {
            for (int i = corridor.startXPos; i >= corridor.EndPositionX; i--)
            {
                tiles[i][corridor.startYPos] = TileType.CorridorTile;
                tiles[i][corridor.startYPos - 1] = TileType.CorridorTile;
                tiles[i][corridor.startYPos + 1] = TileType.CorridorTile;

                destroyATile(i, corridor.startYPos);
                destroyATile(i, corridor.startYPos - 1);
                destroyATile(i, corridor.startYPos + 1);

                InstantiateFromArray(i, corridor.startYPos, false, true);
                InstantiateFromArray(i, corridor.startYPos + 1, false, true);
                InstantiateFromArray(i, corridor.startYPos - 1, false, true);
            }
        }
    }

    void fillRoom(Room room)
    {

        for (int i = room.xPos; i < room.xPos + room.roomWidth; i++)
        {
            for (int j = room.yPos; j < room.yPos + room.roomHeight; j++)
            {
                tiles[i][j] = TileType.AsteroidGenerator;
                destroyATile(i, j);
                InstantiateFromArray((float)i, (float)j);
            }
        }

    }

    void fillCorridor(Corridor corridor)
    {
        if(corridor.direction == Direction.North)
        {
            for (int i = corridor.startYPos; i < corridor.EndPositionY; i++)
            {
                tiles[corridor.startXPos][i] = TileType.AsteroidGenerator;
                tiles[corridor.startXPos+1][i] = TileType.AsteroidGenerator;
                tiles[corridor.startXPos-1][i] = TileType.AsteroidGenerator;

                destroyATile(corridor.startXPos, i);
                destroyATile(corridor.startXPos + 1, i);
                destroyATile(corridor.startXPos - 1, i);

                InstantiateFromArray((float)corridor.startXPos, (float)i);
                InstantiateFromArray((float)corridor.startXPos + 1, (float)i);
                InstantiateFromArray((float)corridor.startXPos - 1, (float)i);


            }
        }

        if (corridor.direction == Direction.South)
        {
            for (int i = corridor.startYPos; i >= corridor.EndPositionY; i--)
            {
                tiles[corridor.startXPos][i] = TileType.AsteroidGenerator;
                tiles[corridor.startXPos + 1][i] = TileType.AsteroidGenerator;
                tiles[corridor.startXPos - 1][i] = TileType.AsteroidGenerator;

                destroyATile(corridor.startXPos, i);
                destroyATile(corridor.startXPos + 1, i);
                destroyATile(corridor.startXPos - 1, i);

                InstantiateFromArray((float)corridor.startXPos, (float)i);
                InstantiateFromArray((float)corridor.startXPos + 1, (float)i);
                InstantiateFromArray((float)corridor.startXPos - 1, (float)i);
            }
        }

        if (corridor.direction == Direction.East)
        {
            for (int i = corridor.startXPos; i < corridor.EndPositionX; i++)
            {
                tiles[i][corridor.startYPos] = TileType.AsteroidGenerator;
                tiles[i][corridor.startYPos+1] = TileType.AsteroidGenerator;
                tiles[i][corridor.startYPos-1] = TileType.AsteroidGenerator;

                destroyATile(i, corridor.startYPos);
                destroyATile(i, corridor.startYPos - 1);
                destroyATile(i, corridor.startYPos + 1);

                InstantiateFromArray((float)i, (float)corridor.startYPos);
                InstantiateFromArray((float)i, (float)corridor.startYPos+1);
                InstantiateFromArray((float)i, (float)corridor.startYPos-1);

            }
        }

        if (corridor.direction == Direction.West)
        {
            for (int i = corridor.startXPos; i >= corridor.EndPositionX; i--)
            {
                tiles[i][corridor.startYPos] = TileType.AsteroidGenerator;
                tiles[i][corridor.startYPos + 1] = TileType.AsteroidGenerator;
                tiles[i][corridor.startYPos - 1] = TileType.AsteroidGenerator;

                destroyATile(i, corridor.startYPos);
                destroyATile(i, corridor.startYPos - 1);
                destroyATile(i, corridor.startYPos + 1);

                InstantiateFromArray((float)i, (float)corridor.startYPos);
                InstantiateFromArray((float)i, (float)corridor.startYPos + 1);
                InstantiateFromArray((float)i, (float)corridor.startYPos - 1);
            }
        }
    }

    /* ^ ROOM GENERATION ^ */

    /* v INITIAL GENERATION v */

    void setUpTiles()
    {
        tiles = new TileType[columns][];
        for (int i = 0; i < columns; i++)
        {
            tiles[i] = new TileType[rows];
        }
    }

    void CreateRoomsAndCorridors()
    {

        creatingRoom = true;

        numRooms = new IntRange(minRooms, maxRooms);
        roomWidth = new IntRange(minRoomWidth, maxRoomWidth);
        roomHeight = new IntRange(minRoomHeight, maxRoomHeight);
        corridorLength = new IntRange(minCorridorLength, maxCorridorLength);

        rooms = new Room[numRooms.Random];
        corridors = new Corridor[rooms.Length - 1];

        rooms[0] = new Room();
        corridors[0] = new Corridor();
        corridors[0].goal_gameobject = corridor_goal;

        //rooms[0].SetupRoom(roomWidth, roomHeight, columns, rows);
        rooms[0].xPos = 45;
        rooms[0].yPos = 45;
        rooms[0].roomWidth = 10;
        rooms[0].roomHeight = 10;

        corridors[0].SetupCorridor(rooms[0], corridorLength, roomWidth, roomHeight, columns, rows, true);

        for (int i = 1; i< rooms.Length; i++) 
        {
            rooms[i] = new Room();
            rooms[i].SetupRoom(roomWidth, roomHeight, columns, rows, corridors[i - 1]);

            if (i < corridors.Length)
            {
                corridors[i] = new Corridor();
                corridors[i].goal_gameobject = corridor_goal;
                corridors[i].SetupCorridor(rooms[i], corridorLength, roomWidth, roomHeight, columns, rows, false);
            }
        }

        /*rooms[1] = new Room();
        rooms[1].xPos = 91;
        rooms[1].yPos = 88;
        rooms[1].roomWidth = 9;
        rooms[1].roomHeight = 8;

        rooms[2] = new Room();
        rooms[2].xPos = 80;
        rooms[2].yPos = 90;
        rooms[2].roomWidth = 9;
        rooms[2].roomHeight = 10;

        corridors[1] = new Corridor();
        corridors[1].direction = Direction.East;
        corridors[1].startXPos = 89;
        corridors[1].startYPos = 91;
        corridors[1].corridorLength = 2;*/
         

        headRoomIndex = rooms.Length - 1;
        headCorridorIndex = corridors.Length - 1;

        creatingRoom = false;

    }

    void SetTileValuesForRooms() 
    {

        for(int i = 0; i<rooms.Length; i++) 
        {
            Room currentRoom = rooms[i];

            for(int j = 0; j < currentRoom.roomWidth; j++)
            {
                int xCoord = currentRoom.xPos + j;

                for (int k = 0; k < currentRoom.roomHeight; k++)
                {
                    int yCoord = currentRoom.yPos + k;

                    tiles[xCoord][yCoord] = TileType.Room;
                }
            }
        }
         
    }

    void SetTilesValuesForCorridors()
    {
        for (int i = 0; i < corridors.Length; i++)
        {
            Corridor currentCorridor = corridors[i];

            for(int j = 0; j< currentCorridor.corridorLength; j++)
            {
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;

                        tiles[xCoord + 1][yCoord] = TileType.CorridorTile;
                        tiles[xCoord - 1][yCoord] = TileType.CorridorTile;

                        break;
                    case Direction.East:
                        xCoord += j;

                        tiles[xCoord][yCoord + 1] = TileType.CorridorTile;
                        tiles[xCoord][yCoord - 1] = TileType.CorridorTile;

                        break;
                    case Direction.South:
                        yCoord -= j;

                        tiles[xCoord + 1][yCoord] = TileType.CorridorTile;
                        tiles[xCoord - 1][yCoord] = TileType.CorridorTile;

                        break;
                    case Direction.West:
                        xCoord -= j;

                        tiles[xCoord][yCoord + 1] = TileType.CorridorTile;
                        tiles[xCoord][yCoord - 1] = TileType.CorridorTile;

                        break;
                }

                tiles[xCoord][yCoord] = TileType.CorridorTile;

            }
        }
    }


    private void InstantiateTiles()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (tiles[i][j] == TileType.AsteroidGenerator)
                {
                    InstantiateFromArray(i, j);
                }else if(tiles[i][j] == TileType.Room)
                {
                    InstantiateFromArray(i, j, true, false);
                }else if(tiles[i][j] == TileType.CorridorTile)
                {
                    InstantiateFromArray(i, j, false, true);

                }
            }
        }
    }

    /* ^ INITIAL GENERATION ^ */


    void InstantiateFromArray(float xCoord, float yCoord)
    {
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        AsteroidGenerator tileInstance = Instantiate(asset, position, Quaternion.identity);
        tileInstance.SetUpTile();


        //print(" " + xCoord + " , " + yCoord);
        int x = Mathf.FloorToInt(xCoord);
        int y = Mathf.FloorToInt(yCoord);

        objs[x][y] = tileInstance;


        tileInstance.transform.parent = boardHolder.transform;
    }

    void InstantiateFromArray(float xCoord, float yCoord, bool room, bool corridor)
    {

        if (!show_debug_icons) { return; }

        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        AsteroidGenerator tileInstance = Instantiate(asset, position, Quaternion.identity);

        tileInstance.isRoom = room;
        tileInstance.isCorridor = corridor;

        tileInstance.SetUpTile();

        //print(" " + xCoord + " , " + yCoord);
        int x = Mathf.FloorToInt(xCoord);
        int y = Mathf.FloorToInt(yCoord);

        objs[x][y] = tileInstance;


        tileInstance.transform.parent = boardHolder.transform;
    }


    void destroyATile(int xCoord, int yCoord)
    {
        if (objs[xCoord][yCoord])
        {
            Destroy(objs[xCoord][yCoord].gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            debug_current_timer += Time.deltaTime;

            if (debug_Timer < debug_current_timer)
            {
                debug_current_timer = 0f;


                if (debug_room_generation)
                {
                    createNewRoom();
                }
                else
                {
                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }

                    BuildOut();
                }


            }
        }
    }
}
