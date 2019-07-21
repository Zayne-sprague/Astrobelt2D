using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRoom : MonoBehaviour
{
    /*
        Rooms keys
        0 - top
        1 - right
        2 - bottom
        3 - left
    */

    [SerializeField] AsteroidWall wall;

    [SerializeField] public int goalWall = 1;
    [SerializeField] public int entryWall = -1;

    [SerializeField] public float width = 16.2f;
    [SerializeField] public float height = 16.2f;
    [SerializeField] public float roomWallWidth = 3.2f;

    [SerializeField] public bool debug = false;
    [SerializeField] public float debugRefresh = 2f;

    private float debugCounter = 0f;

    const int TOP = 0;
    const int RIGHT = 1;
    const int BOTTOM = 2;
    const int LEFT = 3;

    // Start is called before the first frame update
    void Start()
    {
        //buildOut();
    }

    public void buildOut()
    {
        buildTopWall();
        buildRightWall();
        buildBottomWall();
        buildLeftWall();
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

    void buildTopWall()
    {
        if (entryWall == TOP) { return; }

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + height / 2, 0f);
        AsteroidWall new_wall = Instantiate(wall, pos, Quaternion.identity);

        new_wall.isRotated = true;
        new_wall.height = height;

        new_wall.debug = debug;
        new_wall.debugRefresh = debugRefresh;

        new_wall.transform.parent = transform;

        if (goalWall == TOP) { new_wall.hasGoal = true; }
        new_wall.buildOut();
    }

    void buildRightWall()
    {
        if (entryWall == RIGHT) { return; }

        Vector3 pos = new Vector3(transform.position.x + height / 2, transform.position.y, 0f);
        AsteroidWall new_wall = Instantiate(wall, pos, Quaternion.identity);

        new_wall.isRotated = false;
        new_wall.height = width;

        new_wall.debug = debug;
        new_wall.debugRefresh = debugRefresh;

        new_wall.transform.parent = transform;

        if (goalWall == RIGHT) { new_wall.hasGoal = true; }
        new_wall.buildOut();

    }

    void buildBottomWall()
    {
        if (entryWall == BOTTOM) { return; }

        Vector3 pos = new Vector3(transform.position.x, transform.position.y - height / 2, 0f);
        AsteroidWall new_wall = Instantiate(wall, pos, Quaternion.identity);

        new_wall.isRotated = true;
        new_wall.height = height;

        new_wall.debug = debug;
        new_wall.debugRefresh = debugRefresh;

        new_wall.transform.parent = transform;

        if (goalWall == BOTTOM) { new_wall.hasGoal = true; }
        new_wall.buildOut();
    }

    void buildLeftWall()
    {
        if (entryWall == LEFT) { return; }

        Vector3 pos = new Vector3(transform.position.x - height / 2, transform.position.y, 0f);
        AsteroidWall new_wall = Instantiate(wall, pos, Quaternion.identity);

        new_wall.isRotated = false;
        new_wall.height = width;

        new_wall.debug = debug;
        new_wall.debugRefresh = debugRefresh;

        new_wall.transform.parent = transform;

        if (goalWall == LEFT) { new_wall.hasGoal = true; }
        new_wall.buildOut();
    }

    public float[] getBoundingRect()
    {
        float top = transform.position.y + height / 2 + roomWallWidth;
        float bottom = transform.position.y - (height / 2 + roomWallWidth);
        float right = transform.position.x + (width / 2 + roomWallWidth);
        float left = transform.position.x - (width / 2 + roomWallWidth);

        return new float[] { top, bottom, right, left};

    }
}
