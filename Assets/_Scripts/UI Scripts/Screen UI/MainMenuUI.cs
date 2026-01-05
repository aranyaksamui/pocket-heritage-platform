using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Tooltip("The transform of the content that holds the site item button")]
    [SerializeField] Transform contentContainer;
    [Tooltip("The site UI item template button prefab")]
    [SerializeField] GameObject siteItemBtnPrefab;

    private bool hasLoaded = false;


    private void Start()
    {
        StartCoroutine(InitSequence());
    }

    /// <summary>
    ///     Coroutine to wait for Firebase to be ready (when FirebaseInit sets IsReady = true) and only then fetch data from cloud.
    /// </summary>
    IEnumerator InitSequence()
    {
        // Wait for firebase to load
        while (!FirebaseInit.IsReady) yield return null;
        FetchListFromCloud();
    }

    /// <summary>
    ///     Fetch the site list from the cloud by calling the GetAllSites API we defined in CloudDataManager.
    /// </summary>
    private void FetchListFromCloud()
    {
        // If the site list is already loaded do not refetch it
        if (hasLoaded) return;

        Debug.Log("[MainMenuUI/FetchListFromCloud()] 1. Fetching site list from cloud...");

        AREvents.OnLoadingStatusChanged?.Invoke(true, "Fetching site list from the cloud...");
        // Call the GetAllSite API
        CloudDataManager.Instance.GetAllSites
        (
            // Fetch list success callback
            (sites) => 
            {
                AREvents.OnLoadingStatusChanged?.Invoke(false, "");
                Debug.Log($"[MainMenuUI/FetchListFromCloud()] 2.1. No. of sites fetched: {sites.Count}");
                Debug.Log($"[MainMenuUI/FetchListFromCloud()] 2.2. {sites[0].siteName}");
                Debug.Log($"[MainMenuUI/FetchListFromCloud()] 2.3. {sites[0].siteDesc}");
                GenerateSiteList(sites);
                hasLoaded = true;
                AREvents.OnSitesLoadedAndListPopulated.Invoke();
            },
            // Fetch list error callback
            (error) => 
            {
                AREvents.OnLoadingStatusChanged?.Invoke(false, "");
                Debug.Log(error);
                hasLoaded = false;
            }
        );
    }

    /// <summary>
    ///     Generates and sets up action (view or download) button for each sites in the Main Menu.
    /// </summary>
    /// <param name="sites">The sites list.</param>
    private void GenerateSiteList(List<SiteInfo> sites)
    {
        // Cleanup (test buttons or tempalte button from prefab)
        foreach (Transform child in contentContainer) Destroy(child.gameObject);
        // Loop through all the sites
        foreach (SiteInfo site in sites)
        {
            Debug.Log($"[MainMenuUI/GenerateSiteList()] 3. Instantiating site: {site.siteName}");
            // Instantiate the site button
            GameObject siteButton = Instantiate(siteItemBtnPrefab, contentContainer);
            siteButton.GetComponent<Image>().enabled = true;
            siteButton.GetComponent<Button>().enabled = true;
            // Get the SiteUIItem component in that button gameobject
            SiteUIItem siteUIItem = siteButton.GetComponent<SiteUIItem>();
            // Call the Setup function in that component
            siteUIItem.Setup(site);
        }
    }
}
