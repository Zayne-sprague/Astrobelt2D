using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class Settings : MonoBehaviour
{

    [SerializeField] Text sfx_off_label;
    [SerializeField] Text sfx_on_label;

    [SerializeField] Text music_off_label;
    [SerializeField] Text music_on_label;

    [SerializeField] Text alerts_off_label;
    [SerializeField] Text alerts_on_label;

    [SerializeField] Button sfx_button;
    [SerializeField] Button music_button;
    [SerializeField] Button alerts_button;

    private float off_r = 130;
    private float off_g = 130;
    private float off_b = 130;


    // Start is called before the first frame update
    void Start()
    {
        buildOut();

        sfx_button.onClick.AddListener(delegate { settingsToggle("sfx"); });

        music_button.onClick.AddListener(delegate { settingsToggle("music"); });

        alerts_button.onClick.AddListener(delegate { settingsToggle("alerts"); });
    }

    void buildOut()
    {
        int sfx_option = PlayerPrefs.GetInt("settings_sfx", 1);
        int music_option = PlayerPrefs.GetInt("settings_music", 1);
        int alerts_option = PlayerPrefs.GetInt("settings_alerts", 1);

        if (sfx_option > 0)
        {
            MasterAudio.UnmuteBus("SFX");
            whiteText(sfx_on_label);
            grayText(sfx_off_label);
        }
        else
        {
            MasterAudio.MuteBus("SFX");
            whiteText(sfx_off_label);
            grayText(sfx_on_label);
        }

        if (music_option > 0)
        {
            MasterAudio.PlaylistsMuted = false;
            whiteText(music_on_label);
            grayText(music_off_label);
        }
        else
        {
            MasterAudio.PlaylistsMuted = true;
            whiteText(music_off_label);
            grayText(music_on_label);
        }

        if (alerts_option > 0)
        {
            whiteText(alerts_on_label);
            grayText(alerts_off_label);
        }
        else
        {
            whiteText(alerts_off_label);
            grayText(alerts_on_label);
        }
    }

    void whiteText(Text text)
    {
        text.color = Color.white;
    }

    void grayText(Text text)
    {
        text.color = Color.gray; //new Color(off_r, off_g, off_b);
    }

    public void settingsToggle(string name)
    {
        int value = (PlayerPrefs.GetInt("settings_" + name, 1) + 1) % 2;
        PlayerPrefs.SetInt("settings_" + name, value);
        PlayerPrefs.Save();
        buildOut();
    }
}
