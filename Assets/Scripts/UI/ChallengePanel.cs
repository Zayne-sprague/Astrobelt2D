using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePanel : MonoBehaviour
{
    [SerializeField] Image ChallengeImage;
    [SerializeField] Sprite LockedIcon;
    [SerializeField] Sprite UnlockedIcon;
    [SerializeField] Text ChallengeTitle;
    [SerializeField] Text ChallengeDescription;
    [SerializeField] string LockedDescription;
    [SerializeField] string UnlockedDescription;
    [SerializeField] int acheivementID;

    // Start is called before the first frame update
    void Start()
    {
        bool unlocked = PlayerPrefs.GetInt("Ach:" + acheivementID, 0) > 0;

        if (unlocked)
        {
            ChallengeImage.sprite = UnlockedIcon;
            ChallengeDescription.text = UnlockedDescription;
        }
        else
        {
            ChallengeImage.sprite = LockedIcon;
            ChallengeDescription.text = LockedDescription;
        }
    }
}
