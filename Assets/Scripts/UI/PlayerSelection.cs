using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{


    public void ChoosePlayer(int playerID)
    {
        PlayerPrefs.SetInt("PlayerID", playerID);
        PlayerPrefs.Save();
    }
}
