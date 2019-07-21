using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidWall : MonoBehaviour
{
    [SerializeField] int minNumberOfAsteroids = 20;
    [SerializeField] int maxNumberOfAsteroids = 80;
    [SerializeField] int extraAsteroidsForGoalCollapse = 10;

    [SerializeField] float err = 0.03f;

    [SerializeField] float tinyComposition = 0.3f;
    [SerializeField] float smallComposition = 0.45f;
    [SerializeField] float mediumComposition = 0.2f;
    [SerializeField] float largeComposition = 0.05f;

    [SerializeField] Asteroid asteroidObject;
    private int numberOfAsteroids = 0;

    [SerializeField] Goal goalBox;
    private Goal myGoalbox;

    [SerializeField] public float width = 16.1f;
    [SerializeField] public float height = 3.2f;

    [SerializeField] public bool hasGoal = false;
    private bool hasCollapsedGoal = false;

    [SerializeField] public bool isRotated = false;


    [SerializeField] float percentGoalIsOffCenter = 0.25f;
    [SerializeField] float minGapLength = 1.25f;
    [SerializeField] float maxGapLength = 2.50f;

    private Vector3 goalPosition;
    private float goalLength;

    [SerializeField] float timeToCollapseGoal = 1f;

    [SerializeField] public bool debug = false;
    [SerializeField] public float debugRefresh = 2f;
    private float debugCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //buildOut();
    }

    public void buildOut()
    {
        myGoalbox = new Goal();

        if (hasGoal)
        {
            goalLength = Random.Range(minGapLength, maxGapLength);
            goalPosition = getGoalPosition();

            myGoalbox = Instantiate(goalBox, goalPosition, Quaternion.identity);
            myGoalbox.length = goalLength;
            myGoalbox.isRotated = isRotated;
            myGoalbox.transform.parent = transform;

            myGoalbox.buildOut();
        }

        float tmp_tinyComposition = Random.Range(0f, err) + tinyComposition;
        float tmp_smallComposition = Random.Range(0f, err) + smallComposition;
        float tmp_mediumComposition = Random.Range(0f, err) + mediumComposition;
        float tmp_largeComposition = Random.Range(0f, err) + largeComposition;

        int number_of_asteroids = Random.Range(minNumberOfAsteroids, maxNumberOfAsteroids);
        int number_of_tiny = Mathf.CeilToInt(tmp_tinyComposition * number_of_asteroids);
        int number_of_small = Mathf.CeilToInt(tmp_smallComposition * number_of_asteroids);
        int number_of_medium = Mathf.CeilToInt(tmp_mediumComposition * number_of_asteroids);
        int number_of_large = Mathf.CeilToInt(tmp_largeComposition * number_of_asteroids);

        createRoids(number_of_tiny, "tiny");
        createRoids(number_of_small, "small");
        createRoids(number_of_medium, "medium");
        createRoids(number_of_large, "large");

    }

    public void buildOutInGoal()
    { 

        float tmp_tinyComposition = Random.Range(0f, err) + tinyComposition;
        float tmp_smallComposition = Random.Range(0f, err) + smallComposition;
        float tmp_mediumComposition = Random.Range(0f, err) + mediumComposition;
        float tmp_largeComposition = Random.Range(0f, err) + largeComposition;

        int number_of_asteroids = Random.Range(numberOfAsteroids, (maxNumberOfAsteroids + extraAsteroidsForGoalCollapse + Mathf.CeilToInt(extraAsteroidsForGoalCollapse * Random.Range(0f, err))));

        int number_of_tiny = Mathf.CeilToInt(tmp_tinyComposition * number_of_asteroids);
        int number_of_small = Mathf.CeilToInt(tmp_smallComposition * number_of_asteroids);
        int number_of_medium = Mathf.CeilToInt(tmp_mediumComposition * number_of_asteroids);
        int number_of_large = Mathf.CeilToInt(tmp_largeComposition * number_of_asteroids);

        createRoids(number_of_tiny, "tiny", true);
        createRoids(number_of_small, "small", true);
        createRoids(number_of_medium, "medium", true);
        createRoids(number_of_large, "large", true);

    }

    Vector3 getGoalPosition()
    {
        if (!isRotated)
        {
            float y_origin = Random.Range(transform.position.y - (height/2 * percentGoalIsOffCenter), transform.position.y + (height/2 * percentGoalIsOffCenter));
            return new Vector3(transform.position.x, y_origin, 0f);
        }
        else
        {
            float x_origin = Random.Range(transform.position.x - (height/2 * percentGoalIsOffCenter), transform.position.x + (height/2 * percentGoalIsOffCenter));
            return new Vector3(x_origin, transform.position.y, 0f);
        }
    }

    void createRoids(int count, string type, bool putInGoal = false)
    {
        for (int i = 0; i < count; i++)
        {
            Asteroid temp_roid = asteroidObject;
            temp_roid.size = type;

            Vector3 position = getRoidTransform(putInGoal);

            if (hasGoal && inGoalArea(position) && !putInGoal ) { continue; }

            Asteroid asteroid = Instantiate(temp_roid, position, Quaternion.identity);
            asteroid.transform.parent = transform;

        }
    }

    Vector3 getRoidTransform(bool putInGoal = false)
    {
        float minX, maxX, minY, maxY;
        float spawn_width = width;
        float spawn_height = height;
        float x_pos = transform.position.x;
        float y_pos = transform.position.y;

        if (putInGoal)
        {
            spawn_width = myGoalbox.breadthOfWall;
            spawn_height = myGoalbox.length * 2f;
            x_pos = myGoalbox.transform.position.x;
            y_pos = myGoalbox.transform.position.y;
        }


        if (!isRotated)
        {
            minX = x_pos - spawn_width / 2;
            maxX = x_pos + spawn_width / 2;

            minY = y_pos - spawn_height / 2;
            maxY = y_pos + spawn_height / 2;
        }
        else
        {
            minX = x_pos - spawn_height / 2;
            maxX = x_pos + spawn_height / 2;

            minY = y_pos - spawn_width / 2;
            maxY = y_pos + spawn_width / 2;
        }


        // TEMP (no guarentee that it will be distributed throughout).
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        return new Vector3(x, y, 0f);

    }

    bool inGoalArea(Vector3 pos)
    {
        bool in_x = isRotated && (pos.x < goalPosition.x + goalLength && pos.x > goalPosition.x - goalLength);
        bool in_y = !isRotated && (pos.y < goalPosition.y + goalLength && pos.y > goalPosition.y - goalLength);
        if (in_x || in_y)
        {
            return true;
        }

        return false;
    }

    private IEnumerator CollapseGoal()
    {
        float time = 0;
        while( time < timeToCollapseGoal)
        {
            time += Time.deltaTime;

            yield return 0;
        }
        buildOutInGoal();
    }

    // Update is called once per frame
    void Update()
    {

        if (hasGoal && !hasCollapsedGoal && myGoalbox.triggered)
        {
            hasCollapsedGoal = true;
            StartCoroutine(CollapseGoal());
        }




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
