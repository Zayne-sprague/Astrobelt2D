using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorProfile : MonoBehaviour
{
    [SerializeField] public Sprite image;
    [SerializeField] public Image profileImage;
    [SerializeField] public Button button;
    
    // Start is called before the first frame update
    void Start()
    {
        if (image)
        {
            profileImage.sprite = image;
            profileImage.enabled = true;

        }
        else
        {
            profileImage.enabled = false;
        }
    }

    public void buildOut(Sprite img)
    {
        image = img;
        profileImage.sprite = img;

        profileImage.enabled = true;
    }

}
