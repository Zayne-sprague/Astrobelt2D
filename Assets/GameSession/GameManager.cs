using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text scoreLabel;
    [SerializeField] public int score;

    [SerializeField] LevelGenerator_Simple simpleLevel;
    private LevelGenerator_Simple lvl;

    [SerializeField] int RandomSeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        Random.seed = RandomSeed;

        lvl = Instantiate(simpleLevel, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void exitRoom(int points)
    {
        score += points;
        lvl.createRoom(score == 1 ? true : false);

        scoreLabel.text = "" + score + "";

        print("YOU SCORED " + points + " POINTS");
    }

    public void exitGoal()
    {
        print("EXIT GOAL");
    }
}
