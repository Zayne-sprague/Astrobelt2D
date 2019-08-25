using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdScript : MonoBehaviour
{
    public string ios_id = "3261781";
    public string android_id = "3261780";
    public string gameId;
    [SerializeField] public string placementId = "bannerPlacement";
    [SerializeField] public bool testMode = true;
    [SerializeField] public bool topMode = true;

    void Start()
    {
        gameId = ios_id;
        Advertisement.Initialize(gameId, testMode);
        show_banner();
    }

    public void show_banner()
    {
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(placementId);

        if (topMode)
        {
            Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        }
        else
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        }
    }
    private void OnDestroy()
    {
        hide_banner();
    }
    public void hide_banner()
    {
        Advertisement.Banner.Hide(true);
    }
}