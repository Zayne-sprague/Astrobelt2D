using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
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
}
