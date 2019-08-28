using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class Menu : MonoBehaviour
{
    [SerializeField] Text highscoreLabel;
    [SerializeField] Canvas loadingPanel;

    private bool startingLevel = false;

    private void Start()
    {
        if (highscoreLabel)
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

        int sfx_option = PlayerPrefs.GetInt("settings_sfx", 1);
        int music_option = PlayerPrefs.GetInt("settings_music", 1);

        if (music_option > 0)
        {
            MasterAudio.PlaylistsMuted = false;
        }
        else
        {
            MasterAudio.PlaylistsMuted = true;
        }

        if (sfx_option > 0)
        {
            MasterAudio.UnmuteBus("SFX");
        }
        else
        {
            MasterAudio.MuteBus("SFX");
        }



    }


    public void StartFirstLevel()
    {
        if (!startingLevel)
        {
            startingLevel = true;
            StartCoroutine(this.__startlevel());
        }
    }

    private IEnumerator __startlevel()
    {
        float seconds = 0f;

        while (seconds <= 0)
        {
            seconds += Time.deltaTime;
            yield return 0;
        }
        startingLevel = false;
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

    // scene 4 is game over

    public void GoToSettings()
    {
        SceneManager.LoadScene(5);
    }


    public void reset_player_prefs()
    {
        PlayerPrefs.DeleteAll();
        MasterAudio.PlaylistsMuted = false; // manual reset for this
        SceneManager.LoadScene(0); // reload
    }


    public void show_loading()
    {
        if (true)
        {
            print(" SHOW LOADING ");
            loadingPanel.gameObject.SetActive(true);
        }
    }

    public void hide_loader()
    {
        if (loadingPanel)
        {
            loadingPanel.gameObject.SetActive(true);
        }
    }

}
