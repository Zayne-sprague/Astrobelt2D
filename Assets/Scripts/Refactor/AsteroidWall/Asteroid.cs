using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] GameObject[] Large;
    [SerializeField] GameObject[] Medium;
    [SerializeField] GameObject[] Small;
    [SerializeField] GameObject[] Tiny;

    [SerializeField] public string size = "large";

    void Start()
    {
        setAsteroid();
    }

    void setAsteroid()
    {

        GameObject[] roids;

        switch (size)
        {
            case "large":
                roids = Large;
                break;
            case "medium":
                roids = Medium;
                break;
            case "small":
                roids = Small;
                break;
            case "tiny":
                roids = Tiny;
                break;
            default:
                roids = Small;
                break;
        }

        int index = Random.Range(0, roids.Length);

        GameObject asteroid = Instantiate(roids[index], transform);
        asteroid.transform.parent = transform;



    }

    // Update is called once per frame
    void Update()
    {

    }
}
