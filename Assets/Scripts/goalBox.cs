using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalBox : MonoBehaviour
{
    [SerializeField] int pointsForReachingGoal = 1;
    [SerializeField] float waitTimeForCollapse = .25f;

    [SerializeField] Astroid astroid;

    Player player;
    Rigidbody2D myRigidBody;
    BoxCollider2D myBoxCollider;

    private bool triggered;
    private bool collapsed;

    private bool entrance;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (triggered) { return; }
        FindObjectOfType<GameSession>().AddToScore(pointsForReachingGoal);

        if (FindObjectOfType<GameSession>().score > 1)
        {
            FindObjectOfType<GameSession>().CreateNewRoom();
        }

        triggered = true;
 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!triggered || collapsed) { return; }
        collapsed = true;
        StartCoroutine(waitForTriggerToSpawn());

    }


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();

        triggered = false;
        collapsed = false;
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

    private IEnumerator waitForTriggerToSpawn()
    {
        float seconds = 0f;
        while ( seconds  <= waitTimeForCollapse)
        {
            seconds += Time.deltaTime;
            yield return 0;
        }

        collapseGoalBox();

    }

    private void collapseGoalBox()
    {
        float err = 0.05f;
        float width = 0.7f + err;
        float height = 0.625f + err;

        float offset_y = myBoxCollider.size.y * 0f;

        int number_of_roids_y = (int)Mathf.Ceil(myBoxCollider.size.y / height);
        int number_of_roids_x = (int)Mathf.Ceil(myBoxCollider.size.x / width);

        float top_y = transform.position.y - myBoxCollider.size.y/2;
        float left_x = transform.position.x - myBoxCollider.size.x/2;

        for (int i = 0; i < number_of_roids_y; i++)
        {
            for (int j = 0; j < number_of_roids_x; j++)
            {
                float x = j * width + left_x;
                float y = i * height + top_y + offset_y;

                Astroid tmp = Instantiate(astroid, new Vector3(x, y, 0f), Quaternion.identity);
                tmp.transform.parent = transform;
            }
        }
    }
}
