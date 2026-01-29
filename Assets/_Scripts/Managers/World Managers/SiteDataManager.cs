using UnityEngine;
using System.Collections;


/// <summary>
///     This manager class loads the site data from the site_data.json (heritage site data for labels) after the heritate site object has been placed.
/// </summary>
public class SiteDataManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The smart label prefab pointing to the specific heritage feature")]
    [SerializeField] GameObject smartLabelPrefab;

    private string activeSiteId;

    private void OnEnable()
    {
        AREvents.OnObjectPlaced += HandleObjectPlaced;
        AREvents.OnSiteSelection += HandleSiteSelected;
    }

    private void OnDisable()
    {
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
        AREvents.OnSiteSelection -= HandleSiteSelected;
    }

    private void HandleSiteSelected(SiteInfo site)
    {
        activeSiteId = site.siteId;
    }

    /// <summary>
    ///     Load site data on object placed.
    /// </summary>
    private void HandleObjectPlaced(GameObject siteObject)
    {
        Debug.Log($"[SiteDataManager/HandleObjectPlaced()] Object placed. Generating labels...");
        // Find empty child named Features to store the feature labels
        Transform featuresContainer = siteObject.transform.Find("Features");
        // If the Features child not found then create one
        if (featuresContainer == null)
        {
            Debug.LogWarning($"[SiteDataManager/HandleObjectPlaced()] Features object not found! Create an empty features object...");
            GameObject newContainer = new GameObject("Features");
            // Set the parent to the placed site object
            newContainer.transform.SetParent(siteObject.transform, false);
            featuresContainer = newContainer.transform;
        }
        StartCoroutine(WaitForFirebaseAndFetch(featuresContainer));
        //FetchSiteDataFromCloud(featuresContainer);
    }

    /// <summary>
    ///     Coroutine to wait for Firebase to be ready (when FirebaseInit sets IsReady = true) and only then fetch data from cloud.
    /// </summary>
    /// <param name="featureContainer">Features Gameobject container to store the spawned amart labels.</param>
    private IEnumerator WaitForFirebaseAndFetch(Transform featureContainer)
    {
        while (!FirebaseInit.IsReady)
        {
            Debug.Log("[SiteDataManager/WaitForFirebaseAndFetch()] Waiting for firebase to be ready...");
            // Wait 1 frame and check again
            yield return null;
        }
        Debug.Log("[SiteDataManager/WaitForFirebaseAndFetch()] Firebase is ready! Fetching data from cloud...");
        FetchSiteDataFromCloud(featureContainer);
    }

    /// <summary>
    ///     Load the site data from Firestore database using the GetFeaturesForSite API.
    /// </summary>
    /// <param name="container">Features Gameobject container to store the spawned amart labels.</param>
    private void FetchSiteDataFromCloud(Transform container)
    {
        // Turn on the loading screen while the app fetches site data from Firestore
        AREvents.OnLoadingStatusChanged.Invoke(true, "Fetching data from Firestore...");
        // Call the GetFeaturesForSite API
        CloudDataManager.Instance.GetFeaturesForSite(activeSiteId, 
            (features) => 
            {
                ActiveSiteContext.Instance.SetSiteFeatures(features);
                foreach (FeatureData feature in features) SpawnFeatureLabel(feature, container);
                AREvents.OnFeatureLabelsSpawned.Invoke();
            }, 
            (error) => 
            {
                Debug.LogError($"[SiteDataManager/FetchSiteDataFromCloud]\n{error}");
            }
        );
    }

    /// <summary>
    ///     Spawn the feature in it's respective position inside the parent object.
    /// </summary>
    /// <param name="feature">Data of the feature</param>
    /// <param name="parent">Transform of the parent object</param>
    private void SpawnFeatureLabel(FeatureData feature, Transform parent)
    {
        // Instantiate the smart label in the scene
        if (smartLabelPrefab == null) { Debug.LogError("[SiteDataManager/SpawnFeatureLabel()] Smart label prefab is null!"); return; }
        GameObject spawnedLabel = Instantiate(smartLabelPrefab, parent);
        // Set the label anchor position
        Vector3 pos = new Vector3(feature.featurePos.x, feature.featurePos.y, feature.featurePos.z);
        // Initialize and set the label and it's data
        spawnedLabel.GetComponent<ARSmartLabel>().InitilizeSmartLabel(feature, pos);
    }
}
