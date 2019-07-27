using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAsteroid : MonoBehaviour
{

    [SerializeField] public GameObject[] assets;

    [SerializeField] public int Max_Speed = 5;
    [SerializeField] public int Min_Speed = 2;
    public IntRange speed;

    private int time_to_cross;
    private float t;

    private GameObject asteroid;
    private GameObject tmp;
    private bool ready_to_fly = false;

    private Vector3 startPosition;
    private Vector3 endPosition;



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


    // Start is called before the first frame update
    void Start()
    {

        speed = new IntRange(Min_Speed, Max_Speed);

        time_to_cross = speed.Random;
            
    }

    public void BuildOut(Room room)
    {
        Direction[] dirs = new Direction[4] { Direction.North, Direction.East, Direction.South, Direction.West };
        int index = Random.Range(0, dirs.Length);

        Direction starting_direction = dirs[index];

        RemoveAt(ref dirs, index);

        index = Random.Range(0, dirs.Length);
        Direction ending_direction = dirs[index];

        BuildOut(get_random_location(starting_direction, room), get_random_location(ending_direction, room));

    }

    public void BuildOut(Vector3 starting_vector, Vector3 ending_vector)
    {
        BuildOut((int)starting_vector.x, (int)starting_vector.y, (int)ending_vector.x, (int)ending_vector.y);
    }

    public void BuildOut(int x, int y, int end_x, int end_y)
    {

        startPosition = new Vector3(x, y, 0);
        transform.position = startPosition;

        endPosition = new Vector3(end_x, end_y, 0);

        int index = Random.Range(0, assets.Length);
        asteroid = assets[index];

        tmp = Instantiate(asteroid, startPosition, Quaternion.identity);
        tmp.transform.parent = transform;

        ready_to_fly = true;

    }

    private Vector3 get_random_location(Direction dir, Room room)
    {
        Vector3 location = new Vector3(0,0,0);

        switch (dir)
        {
            case Direction.North:
                location.x = Random.Range(room.xPos - 1, room.xPos + room.width + 1);
                location.y = room.yPos + room.height + 1;
                break;
            case Direction.East:
                location.x = room.xPos + room.width + 1;
                location.y = Random.Range(room.yPos - 1, room.yPos + room.height + 1);
                break;
            case Direction.South:
                location.x = Random.Range(room.xPos - 1, room.xPos + room.width + 1);
                location.y = room.yPos - 1;
                break;
            case Direction.West:
                location.x = room.xPos - 1;
                location.y = Random.Range(room.yPos - 1, room.yPos + room.height + 1);
                break;

        }

        //Default
        return location;
    }

    // Update is called once per frame
    void Update()
    {
        if (ready_to_fly)
        {
            t += Time.deltaTime / (float)time_to_cross;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }

        if (transform.position == endPosition)
        {
            Destroy(tmp);
            Destroy(gameObject);
        }

    }
}
