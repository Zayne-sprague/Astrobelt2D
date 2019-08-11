using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    [SerializeField] Text ScoreCount;
    [SerializeField] Text FluberionCount;
    [SerializeField] Text BestCount;
    [SerializeField] Image Medal;

    [SerializeField] Sprite[] medals;
    [SerializeField] int[] medal_cut_offs;

    [SerializeField] float timerToCount = 0.2f;

    [SerializeField] bool debug;
    [SerializeField] int debug_score;
    [SerializeField] int debug_fluberion;
    [SerializeField] int debug_best;

    private int score;
    private int flubes;
    private int best;

    private int displayed_score;
    private int displayed_flubes;
    private int displayed_best;

    // Start is called before the first frame update
    void Start()
    {
        ScoreCount.text = "0";
        FluberionCount.text = "0";
        BestCount.text = "0";

        Medal.gameObject.SetActive(false);





        if (debug)
        {
            score = debug_score;
            flubes = debug_fluberion;
            best = debug_best;
        } else
        {
            score = PlayerPrefs.GetInt("last_game_score", 0);
            flubes = PlayerPrefs.GetInt("last_game_fluberions", 0);
            best = PlayerPrefs.GetInt("highscore", 0);
        }

        int index = medal_cut_offs.Length - 1;

        for (int i = 0; i < medal_cut_offs.Length; i++)
        {
            if (score < medal_cut_offs[i])
            {
                index = Mathf.Max(i - 1, 0);
                break;
            }
        }

        Medal.sprite = medals[index];

        displayed_score = 0;
        displayed_flubes = 0;
        displayed_best = 0;

        StartCoroutine(ScoreUpdater());
        StartCoroutine(FlubesUpdater());
        StartCoroutine(BestUpdater());

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

        Medal.gameObject.SetActive(true);
    }

    private IEnumerator BestUpdater()
    {
        while (true)
        {

            bool slower_count = (best - displayed_best) <= 10;

            if (displayed_best < best)
            {
                displayed_best++;
                BestCount.text = displayed_best.ToString();
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
