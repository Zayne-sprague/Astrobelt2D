using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{


    [SerializeField] public float length = 2.0f;
    [SerializeField] public float breadthOfWall = 3.2f;
    [SerializeField] public bool isRotated = false;
    
    [SerializeField] public int points = 1;
    public bool triggered = false;

    BoxCollider2D myCollider;
    GameManager manager;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void buildOut()
    {
        myCollider = GetComponent<BoxCollider2D>();
        manager = FindObjectOfType<GameManager>();
        triggered = false;

        if (isRotated)
        {
            myCollider.size = new Vector2(length, breadthOfWall);
        }
        else
        {
            myCollider.size = new Vector2(breadthOfWall, length);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        manager.exitRoom(points);
        points -= 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(triggered) { return; }
        triggered = true;
        manager.exitGoal();

    }


}
