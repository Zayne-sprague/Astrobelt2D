using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SelectorSlider : MonoBehaviour
{


    [SerializeField] Sprite[] imageProfiles;
    [SerializeField] string[] titles;
    [SerializeField] string[] descriptions;
    [SerializeField] string[] locked_descriptions;
    [SerializeField] bool isPlayerIds;

    [SerializeField] SelectorProfile profile;
    [SerializeField] Text TitleLabel;
    [SerializeField] Text DescriptionLabel;

    private Vector3 mid_tile_location = new Vector3(0, 0, 0);
    private Vector2 mid_tile_size = new Vector2(197.4f, 157.1f);

    private Vector3 left_tile_location = new Vector3(-210.15f, 21, 0);
    private Vector2 left_tile_size = new Vector2(197.4f, 157.1f);

    private Vector3 right_tile_location = new Vector3(210.15f, 21, 0);
    private Vector2 right_tile_size = new Vector2(197.4f, 157.1f);

    private Vector3 mid_tile_image_location = new Vector3(0, 0, 0);
    //private Vector2 mid_tile_image_size = new Vector2(156.6f, 128.9f);
    private Vector2 mid_tile_image_size = new Vector2(20.7f, 19.55f);
    private Vector3 mid_tile_image_rotation = new Vector3(0, 0, 0);

    private Vector3 left_tile_image_location = new Vector3(50.325f, 0f, 0);
    //private Vector2 left_tile_image_size = new Vector2(89.3f, 128.9f);
    private Vector2 left_tile_image_size = new Vector2(50.325f, 25.00001f);
    private Vector3 left_tile_image_rotation = new Vector3(0, 0, -23.066f);

    private Vector3 right_tile_image_location = new Vector3(50.325f, 0f, 0);
    //private Vector2 right_tile_image_size = new Vector2(89.3f, 128.9f);
    private Vector2 right_tile_image_size = new Vector2(50.325f, 25f);
    private Vector3 right_tile_image_rotation = new Vector3(0, 0, 23.066f);

    private SelectorProfile left_tile;
    private SelectorProfile mid_tile;
    private SelectorProfile right_tile;

    private int prev_index;
    private int curr_index;
    private int next_index;

    // Start is called before the first frame update
    void Start()
    {
        curr_index = 0;

        if (isPlayerIds)
        {
            curr_index = PlayerPrefs.GetInt("PlayerID", 1) - 1;
        }

        prev_index = curr_index - 1 < 0 ? imageProfiles.Length - 1 : curr_index - 1;
        next_index = curr_index + 1 > imageProfiles.Length - 1 ? 0 : curr_index + 1;

        BuildOutPanels(prev_index, curr_index, next_index);
    }

    void BuildOutPanels(int left_index, int mid_index, int right_index)
    {
        prev_index = left_index;
        curr_index = mid_index;
        next_index = right_index;

        // Clean
        if (left_tile)
        {
            Destroy(left_tile.gameObject);
            Destroy(mid_tile.gameObject);
            Destroy(right_tile.gameObject);
        }

        // LEFT TILE
        left_tile = Instantiate(profile, transform);
        left_tile.transform.parent = transform;
        left_tile.profileImage.preserveAspect = false;

        left_tile.transform.localPosition = left_tile_location;
        //left_tile.button.GetComponent<RectTransform>().position = left_tile_location;
        left_tile.button.GetComponent<RectTransform>().sizeDelta = left_tile_size;

        left_tile.profileImage.rectTransform.SetLeft(left_tile_image_location.x);
        left_tile.profileImage.rectTransform.SetTop(left_tile_image_location.y);
        left_tile.profileImage.rectTransform.SetRight(left_tile_image_size.x);
        left_tile.profileImage.rectTransform.SetBottom(left_tile_image_size.y);


        left_tile.profileImage.rectTransform.eulerAngles = left_tile_image_rotation;


        left_tile.button.onClick.AddListener(delegate { BuildOutPanels(prev_index - 1 < 0 ? imageProfiles.Length - 1 : prev_index - 1,prev_index, curr_index); });


        left_tile.buildOut(imageProfiles[left_index]);

        // MID TILE
        mid_tile = Instantiate(profile);
        mid_tile.transform.parent = transform;

        mid_tile.transform.localPosition = mid_tile_location;
        //mid_tile.button.GetComponent<RectTransform>().position = mid_tile_location;
        mid_tile.button.GetComponent<RectTransform>().sizeDelta = mid_tile_size;

        mid_tile.profileImage.rectTransform.transform.localPosition = mid_tile_image_location;
        mid_tile.profileImage.rectTransform.SetRight(mid_tile_image_size.x);
        mid_tile.profileImage.rectTransform.SetBottom(mid_tile_image_size.y);
        mid_tile.profileImage.rectTransform.eulerAngles = mid_tile_image_rotation;

        mid_tile.profileImage.preserveAspect = true;

        mid_tile.button.onClick.AddListener(delegate { playgame(); });

        mid_tile.buildOut(imageProfiles[mid_index]);

        // RIGHT TILE
        right_tile = Instantiate(profile);
        right_tile.transform.parent = transform;
        right_tile.profileImage.preserveAspect = false;

        right_tile.transform.localPosition = right_tile_location;
        //right_tile.button.GetComponent<RectTransform>().position = right_tile_location;
        right_tile.button.GetComponent<RectTransform>().sizeDelta = right_tile_size;

        right_tile.profileImage.rectTransform.SetLeft(right_tile_image_location.x);
        right_tile.profileImage.rectTransform.SetTop(right_tile_image_location.y);
        right_tile.profileImage.rectTransform.SetRight(right_tile_image_size.x);
        right_tile.profileImage.rectTransform.SetBottom(right_tile_image_size.y);
        right_tile.profileImage.rectTransform.eulerAngles = right_tile_image_rotation;


        right_tile.button.onClick.AddListener(delegate { BuildOutPanels(curr_index, next_index, next_index + 1 > imageProfiles.Length - 1 ? 0 : next_index + 1); });

        right_tile.buildOut(imageProfiles[right_index]);

        // SET TITLE AND DESCRIPTION

        TitleLabel.text = titles[mid_index];
        bool unlocked = false;

        if (isPlayerIds && mid_index > 4)
        {
            unlocked = PlayerPrefs.GetInt("pID:" + mid_index, 0) > 0;
        }
        else if (!isPlayerIds)
        {
            unlocked = PlayerPrefs.GetInt("Ach:" + mid_index, 0) > 0;
        }

        if (unlocked)
        {
            DescriptionLabel.text = descriptions[mid_index];
        }
        else
        {
            DescriptionLabel.text = locked_descriptions[mid_index];
        }
    }

    void playgame()
    {
        if (isPlayerIds)
        {
            bool unlocked = PlayerPrefs.GetInt("pID:" + curr_index, 0) > 0 || (curr_index <= 4); 

            if (unlocked)
            {
                PlayerPrefs.SetInt("PlayerID", curr_index + 1);
                SceneManager.LoadScene(1);
            }
        }
    }
}


public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}