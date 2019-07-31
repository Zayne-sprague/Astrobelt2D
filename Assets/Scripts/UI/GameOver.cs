using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    [SerializeField] Text ScoreCount;
    [SerializeField] Text FluberionCount;

    [SerializeField] float timerToCount = 0.2f;

    [SerializeField] bool debug;
    [SerializeField] int debug_score;
    [SerializeField] int debug_fluberion;

    private int score;
    private int flubes;

    private int displayed_score;
    private int displayed_flubes;

    // Start is called before the first frame update
    void Start()
    {
        ScoreCount.text = "0";
        FluberionCount.text = "0";

        displayed_score = 0;
        displayed_flubes = 0; 

        if (debug)
        {
            score = debug_score;
            flubes = debug_fluberion;
        } else
        {
            score = PlayerPrefs.GetInt("last_game_score", 0);
            flubes = PlayerPrefs.GetInt("last_game_fluberions", 0);
        }

        StartCoroutine(ScoreUpdater());
        StartCoroutine(FlubesUpdater());


    }

    private IEnumerator ScoreUpdater()
    {
        while (true)
        {
            bool slower_count = (score - displayed_score) <= 10;

            if (displayed_score < score)
            {
                displayed_score++;
                ScoreCount.text = displayed_score.ToString();
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(slower_count ? timerToCount * 2 : timerToCount);
        }
    }

    private IEnumerator FlubesUpdater()
    {
        while (true)
        {

            bool slower_count = (flubes - displayed_flubes) <= 10;

            if (displayed_flubes < flubes)
            {
                displayed_flubes++;
                FluberionCount.text = displayed_flubes.ToString();
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(slower_count ? timerToCount * 2 : timerToCount);
        }
    }
}
