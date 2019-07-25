using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProceduralGameManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] public int score = 0;
    [SerializeField] LevelCreator lvlCreator;
    [SerializeField] PlayerController player;


    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<ProceduralGameManager>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
    }

    public void ProcessPlayerDeath()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }


    public void scored(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();

        player.increaseShipSpeed();
    }

    public void level_complete()
    {
        lvlCreator.AddARoom();
        return;
    }
}
