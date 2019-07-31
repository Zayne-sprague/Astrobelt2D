using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio; //for later

public class Achievements : MonoBehaviour
{
    [SerializeField] AchievementCanvas panel;
    private AchievementCanvas inst_panel;

    [SerializeField] float show_off_timer = 2;
    private float current_timer = 0;

    [SerializeField] string[] titles;
    [SerializeField] Sprite[] images;

    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<Achievements>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public static bool UnlockAchievement(int id)
    {
        bool unlocked = PlayerPrefs.GetInt("Ach:"+id, 0) > 0;
        if (!unlocked)
        {
            PlayerPrefs.SetInt("Ach:" + id, 1);
            PlayerPrefs.Save();

            return true;
        }

        return false;
    }

    public bool _UnlockAchievement(int id)
    {
        bool unlocked = PlayerPrefs.GetInt("Ach:" + id, 0) > 0;
        if (!unlocked)
        {
            PlayerPrefs.SetInt("Ach:" + id, 1);
            PlayerPrefs.Save();
            showOffChallenge(id);
            return true;
        }

        return false;
    }

    void showOffChallenge(int id)
    {
        if (inst_panel)
        {
            Destroy(inst_panel.gameObject);
        }

        inst_panel = Instantiate(panel, transform);
        inst_panel.image.sprite = images[id - 1];
        inst_panel.title.text = titles[id - 1];
        inst_panel.gameObject.SetActive(true);

        StopAllCoroutines();
        current_timer = 0;
        StartCoroutine(showOff());

    }

    IEnumerator showOff()
    {
        while (current_timer < show_off_timer)
        {
            current_timer += Time.deltaTime;
            yield return 0;
        }

        current_timer = 0;
        if (inst_panel)
        {
            Destroy(inst_panel.gameObject);
        }
    }

}
