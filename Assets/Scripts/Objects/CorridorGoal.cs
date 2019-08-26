using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class CorridorGoal : MonoBehaviour
{
    [SerializeField] public float total_time_to_create_room = 5;
    [SerializeField] public int pointsToRecieve = 1;

    public SpriteRenderer mySpriteRenderer;
    ProceduralGameManager game_manager;
    BoxCollider2D myBoxCollider;

    // Start is called before the first frame update
    void Start()
    {

        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        game_manager = FindObjectOfType<ProceduralGameManager>();
    }

    public void BuildCollisionBox(float x, float y, int rZ)
    {
        myBoxCollider = GetComponent<BoxCollider2D>();
        myBoxCollider.size = new Vector2(x, y);
        //myBoxCollider.transform.rotation = Quaternion.Euler(0, 0, rZ);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print("Collision Left");
        StartCoroutine(make_new_room());
    }

    IEnumerator make_new_room()
    {
        float t = 0;

        while (t < total_time_to_create_room)
        {
            t += Time.deltaTime;
            yield return 0;
        }

        game_manager.level_complete();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MasterAudio.FireCustomEvent("Scored",transform);
        //print("Collision Hit");
        game_manager.scored(pointsToRecieve);
        mySpriteRenderer.enabled = false;
    }
}
