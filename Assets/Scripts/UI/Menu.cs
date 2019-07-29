using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] Text highscoreLabel;

    private void Start()
    {
        int score = PlayerPrefs.GetInt("highscore");

        if (score > 0)
        {
            highscoreLabel.text = "High Score: " + score;
        }
        else
        {
            highscoreLabel.text = "";
        }
    }


    public void StartFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToCharacterSelection()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToChallenges()
    {
        SceneManager.LoadScene(3);
    }


}
