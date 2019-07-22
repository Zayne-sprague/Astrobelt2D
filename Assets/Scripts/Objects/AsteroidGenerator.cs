using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    //We are dealing with tiles here, same width and height
    [SerializeField] public int tileSize = 4;

    // Game objects for the assets
    [SerializeField] public GameObject[] assets;

    // Number of asteroids
    [SerializeField] public int minSize = 4;
    [SerializeField] public int maxSize = 10;

    public IntRange size;

    [SerializeField] public GameObject roomTile;
    [SerializeField] public GameObject corridorTile;
    public bool isRoom = false;
    public bool isCorridor = false;

    // Debug
    [SerializeField] public bool debug = false;
    [SerializeField] public float debug_Timer = 10;
    private float debug_current_timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        //SetUpTile();
    }

    public void SetUpTile()
    {
        size = new IntRange(minSize, maxSize);
        float xPos, yPos, xEnd, yEnd;
        xPos = transform.position.x;
        yPos = transform.position.y;
        xEnd = transform.position.x + tileSize;
        yEnd = transform.position.y + tileSize;

        if (isCorridor)
        {
            float new_xPos = (xPos + xEnd)/2;
            float new_yPos = (yPos + yEnd)/2;

            Vector3 newPosition = new Vector3(new_xPos, new_yPos, transform.position.z);

            GameObject asset = Instantiate(corridorTile, newPosition, Quaternion.identity);
            asset.transform.parent = transform;

            return;
        }

        if (isRoom)
        {
            float new_xPos = (xPos + xEnd) / 2;
            float new_yPos = (yPos + yEnd) / 2;

            Vector3 newPosition = new Vector3(new_xPos, new_yPos, transform.position.z);

            GameObject asset = Instantiate(roomTile, newPosition, Quaternion.identity);
            asset.transform.parent = transform;

            return;
        }

        // Create the Assets
        for (int i = 0; i < size.Random; i++)
        {
            int index = Random.Range(0, assets.Length);

            float new_xPos = Random.Range(xPos, xEnd);
            float new_yPos = Random.Range(yPos, yEnd);

            Vector3 newPosition = new Vector3(new_xPos, new_yPos, transform.position.z);

            GameObject asset = Instantiate(assets[index], newPosition, Quaternion.identity);
            asset.transform.parent = transform;


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

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                SetUpTile();
            }
        }
    }
}
