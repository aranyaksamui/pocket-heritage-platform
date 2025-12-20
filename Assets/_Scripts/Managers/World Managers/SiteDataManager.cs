using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System;


/// <summary>
///     This manager class loads the site data from the site_data.json (heritage site data for labels) after the heritate site object has been placed.
/// </summary>
public class SiteDataManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Heritage site Firestore document id")]
    [SerializeField] string firestoreSiteId = "site_tajmahal";
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
        //StartCoroutine(WaitForFirebaseAndFetch(featuresContainer));
        FetchSiteDataFromCloud(featuresContainer);
    }

    /// <summary>
    ///     Coroutine to wait for Firebase to be ready (when FirebaseInit sets IsReady = true) and only then fetch data from cloud.
    /// </summary>
    /// <param name="featureContainer">Features Gameobject container to store the spawned amart labels.</param>
    /// <returns></returns>
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
    ///     Load the site data from Firestore database.
    /// </summary>
    /// <param name="container">Features Gameobject container to store the spawned amart labels.</param>
    private async void FetchSiteDataFromCloud(Transform container)
    {
        // Turn on the loading screen while the app fetches site data from Firestore
        AREvents.OnLoadingStatusChanged.Invoke(true, "Fetching data from Firestore...");
        // Get the cached (pre-warmed) Firestore instance we did in FirebaseInit
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        // Query structure to get the site_features collection of a heritage site
        CollectionReference featuresRef = db.Collection("heritage_sites").Document(firestoreSiteId).Collection("site_features");
        // Send request to get the features collection
        QuerySnapshot featuresCollection = await featuresRef.GetSnapshotAsync().ContinueWithOnMainThread(GetQuerySnapshot);
        // Process and span each feature document in the collection
        foreach (DocumentSnapshot document in featuresCollection)
        {
            ProcessDocument(document, container);
        }
        // Change state from loading to interaction after labels are spawned
        AREvents.OnFeatureLabelsSpawned.Invoke();
    }

    /// <summary>
    ///     The callback function that gets the snapshots from query result.
    /// </summary>
    /// <param name="snapshotTask">The snapshots task.</param>
    private QuerySnapshot GetQuerySnapshot(Task<QuerySnapshot> snapshotTask)
    {
        AREvents.OnLoadingStatusChanged(false, "");

        if (snapshotTask.IsFaulted)
        {
            Debug.Log($"[SiteDataManager/WaitForFirebaseAndFetch()] \n {snapshotTask.Exception}");
            return null;
        }

        QuerySnapshot snapshot = snapshotTask.Result;
        Debug.Log($"[SiteDataManager/WaitForFirebaseAndFetch()] Downloaded {snapshot.Count} labels");
        return snapshot;
    }

    /// <summary>
    ///     Process each feature document and convert it to Firestore to C# format for spawning over the parent heritage site model.
    /// </summary>
    /// <param name="document">The feature document.</param>
    /// <param name="container">Feature container.</param>
    private void ProcessDocument(DocumentSnapshot document, Transform container)
    {
        // Converting Firebase document to C# dictionary
        Dictionary<string, object> documentData = document.ToDictionary();

        FeatureData featureData = new FeatureData();
        // Safe mapping of name and description of the site data
        featureData.featureName = documentData.ContainsKey("feature_name") 
            ? documentData["feature_name"].ToString() 
            : "Unknown name";
        featureData.featureDesc = documentData.ContainsKey("feature_description") 
            ? documentData["feature_description"].ToString() 
            : "Unknown description";
        featureData.triggerVisibilityDist = documentData.ContainsKey("feature_trigger_visibility_distance")
            ? Convert.ToSingle(documentData["feature_trigger_visibility_distance"]) 
            : 1.5f;

        Debug.Log($"[SiteDataManager/ProcessDocument()] \n Feature name: {featureData.featureName} \n Feature desc: {featureData.featureDesc}");

        // Safe mapping of position
        if (documentData.ContainsKey("feature_position"))
        {
            // Firestore stores Maps as Dictionaries
            Dictionary<string, object> posMap = (Dictionary<string, object>) documentData["feature_position"];

            float posX = Convert.ToSingle(posMap["position_x"]);
            float posY = Convert.ToSingle(posMap["position_y"]);
            float posZ = Convert.ToSingle(posMap["position_z"]);

            featureData.featurePos = new PositionData { x = posX, y = posY, z = posZ };
        }
        else featureData.featurePos = new PositionData { x = 0f, y = 0f, z = 0f };
        // Spawn the feature label
        SpawnFeatureLabel(featureData, container);
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
