using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProceduralGameManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text coinScore;
    [SerializeField] public int score = 0;
    private int current_game_flubes = 0;
    [SerializeField] LevelCreator lvlCreator;
    [SerializeField] PlayerController player;

    [SerializeField] public int min_time_to_spawn_roids = 10;
    [SerializeField] public int max_time_to_spawn_roids = 20;
    [SerializeField] public int min_number_of_roids = 0;
    [SerializeField] public int max_number_of_roids = 10;
    [SerializeField] public int starting_level_for_roids = 10;

    [SerializeField] public int starting_level_for_coins = 0;


    private IntRange spawnTimeForRoids;
    private IntRange numberOfRoadsToSpawn;

    private float next_spawn_time = 0;
    private float current_spawn_time = 0;

    // Debug
    [SerializeField] public bool debug = false;
    [SerializeField] public float debug_Timer = 0.5f;
    private float debug_current_timer = 0;

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
        coinScore.text = "" + PlayerPrefs.GetInt("coins", 0);

        spawnTimeForRoids = new IntRange(min_time_to_spawn_roids, max_time_to_spawn_roids);
        numberOfRoadsToSpawn = new IntRange(min_number_of_roids, max_number_of_roids);

        next_spawn_time = (float)spawnTimeForRoids.Random;
    }

    public void ProcessPlayerDeath()
    {



        PlayerPrefs.SetInt("last_game_score", score);
        PlayerPrefs.SetInt("last_game_fluberions", current_game_flubes);


        // Save high score //

        if (PlayerPrefs.GetInt("highscore") < score)
        {
            PlayerPrefs.SetInt("highscore", score);
        }

        // *************** //

        PlayerPrefs.Save();


        // Set Death Achievment //
        Achievements.UnlockAchievement(1);
        // ******************** //

        SceneManager.LoadScene(4);
        Destroy(gameObject);
    }

    public void coinPickedUp(int pointsToAdd)
    {
        int coins = PlayerPrefs.GetInt("coins", 0);

        coins += pointsToAdd;
        current_game_flubes += pointsToAdd;

        coinScore.text = "" + coins;

        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
    }

    public void scored(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();

        // Set Level 10 achievement //
        if (score == 10)
        {
            Achievements.UnlockAchievement(2);
        }
        // ************************ //

        // Do this better
        if (!debug)
        {
            player.increaseShipSpeed();

        }
    }

    public void level_complete()
    {
        Room room = lvlCreator.AddARoom();

        if (score >= starting_level_for_coins)
        {
            lvlCreator.spawn_coins(room, 1);
        }

        return;
    }

    private void Update()
    {
        if (score >= starting_level_for_roids)
        {
            current_spawn_time += Time.deltaTime;

            if (current_spawn_time >= next_spawn_time)
            {
                current_spawn_time = 0;
                next_spawn_time = spawnTimeForRoids.Random;
                lvlCreator.spawn_roids(numberOfRoadsToSpawn);
            }

        }

        if (debug)
        {
            debug_current_timer += Time.deltaTime;

            if (debug_Timer < debug_current_timer)
            {
                spawnTimeForRoids = new IntRange(min_time_to_spawn_roids, max_time_to_spawn_roids);
                numberOfRoadsToSpawn = new IntRange(min_number_of_roids, max_number_of_roids);

                debug_current_timer = 0f;

                scored(1);
                level_complete();

            }
        }
    }
}
