using UnityEngine;


/*
 * Summary:
 *      This manager class loads the site data from the site_data.json (heritage site data for labels) after the heritate site object has been placed.
*/
public class SiteDataManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Site data json file")]
    [SerializeField] string siteDataJsonFile = "site_data";
    [Tooltip("The smart label prefab pointing to the specific heritage feature")]
    [SerializeField] GameObject smartLabelPrefab;


    private void OnEnable()
    {
        AREvents.OnObjectPlaced += HandleObjectPlaced;
    }

    private void OnDisable()
    {
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
    }

    // Load site data on object placed
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
        LoadSiteData(featuresContainer);
    }

    // Load the specific site data from its json file
    private void LoadSiteData(Transform parentObject)
    {
        // Find the file
        TextAsset jsonFile = Resources.Load<TextAsset>(siteDataJsonFile);
        // Decode the file
        SiteData jsonData = JsonUtility.FromJson<SiteData>(jsonFile.text);
        // Loop through the data
        foreach (FeatureData feature in jsonData.features)
        {
            SpawnFeatureLabel(feature, parentObject);
        }
    }

    // Spawn the feature in it's respective position inside the parent object
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
