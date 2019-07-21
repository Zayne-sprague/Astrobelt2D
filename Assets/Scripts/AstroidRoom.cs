using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidRoom : MonoBehaviour
{
    [SerializeField] astroidGoalWall goalWall;
    [SerializeField] AstroidWall wall;
    [SerializeField] AstroidEntranceWall entWall;
    [SerializeField] public float height_of_wall = 16.1f;
    [SerializeField] public float width_of_wall = 3.2f;


    public float x_origin = 0f;
    public float y_origin = 0f;
                         // 0 == wall, 1 == entry, 2 == goal
                         // top    down   left   right
    public int[] side = {    0    , 0    , 0    , 2 };

    private Player player;
    private Rigidbody2D myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        myRigidBody = GetComponent<Rigidbody2D>();


        initiateWalls();


    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isAlive)
        {
            myRigidBody.velocity = new Vector2(0f, 0f);
            return;
        }
        move();
    }

    private void move()
    {
        Vector2 movement = player.getMovement();
        myRigidBody.velocity = movement;

    }


    void initiateWalls()
    {

        float x_from_origin = .5f * height_of_wall;
        float y_from_origin = .5f * height_of_wall + width_of_wall * .5f;
        AstroidWall tmp;
        astroidGoalWall tmp2;
        AstroidEntranceWall tmp3;

        //top
        int top = side[0];
        if (top == 0)
        {
            tmp = Instantiate(wall, new Vector3(x_origin, y_origin + y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp.transform.parent = transform;
        }
        else if (top == 1)
        {
            tmp3 = Instantiate(entWall, new Vector3(x_origin, y_origin + y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp3.transform.parent = transform;
        }
        else
        {

            tmp2 = Instantiate(goalWall, new Vector3(x_origin, y_origin + y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp2.transform.parent = transform;
        }

        //down
        int down = side[1];
        if (down == 0)
        {
            tmp = Instantiate(wall, new Vector3(x_origin, y_origin - y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp.transform.parent = transform;

        }
        else if (down == 1)
        {
            tmp3 = Instantiate(entWall, new Vector3(x_origin, y_origin - y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp3.transform.parent = transform;
        }
        else
        {
            tmp2 = Instantiate(goalWall, new Vector3(x_origin, y_origin - y_from_origin, 0), Quaternion.Euler(0f, 0f, 90f));
            tmp2.transform.parent = transform;

        }

        //left
        int left = side[2];
        if (left == 0)
        {
            tmp = Instantiate(wall, new Vector3(x_origin - x_from_origin, y_origin, 0), Quaternion.identity);
            tmp.transform.parent = transform;
        }
        else if (left == 1)
        {
            tmp3 = Instantiate(entWall, new Vector3(x_origin - x_from_origin, y_origin, 0), Quaternion.identity);
            tmp3.transform.parent = transform;
        }
        else
        {
            tmp2 = Instantiate(goalWall, new Vector3(x_origin - x_from_origin, y_origin, 0), Quaternion.identity);
            tmp2.transform.parent = transform;

        }

        //right
        int right = side[3];
        if (right == 0)
        {
            tmp = Instantiate(wall, new Vector3(x_origin + x_from_origin, y_origin, 0), Quaternion.identity);
            tmp.transform.parent = transform;

        }
        else if (right == 1)
        {
            tmp3 = Instantiate(entWall, new Vector3(x_origin + x_from_origin, y_origin, 0), Quaternion.identity);
            tmp3.transform.parent = transform;
        }
        else
        {
            tmp2 = Instantiate(goalWall, new Vector3(x_origin + x_from_origin, y_origin, 0), Quaternion.identity);
            tmp2.transform.parent = transform;

        }

    }
}