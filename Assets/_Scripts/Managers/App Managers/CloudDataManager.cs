using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Extensions;


/// <summary>
///     This manager class talks to Google to get site data on demand.
/// </summary>
public class CloudDataManager : MonoBehaviour
{
    private List<SiteInfo> cachedSites = null;

    /// <summary>
    ///     CloudDataManager singleton instance
    /// </summary>
    public static CloudDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    // API #1: Get all sites for Main Menu rendering
    /// <summary>
    ///     API: Get list of all sites.
    /// </summary>
    /// <param name="onSuccess">Pass sites if success.</param>
    /// <param name="onError">Pass error if failure.</param>
    public void GetAllSites(Action<List<SiteInfo>> onSuccess, Action<string> onError)
    {
        // Return cached sites if available
        if (cachedSites != null) { onSuccess?.Invoke(cachedSites); return; }
        // Get the cached (pre-warmed) Firestore instance we did in FirebaseInit
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        // Fetch sites from cloud
        db.Collection("heritage_sites").GetSnapshotAsync().ContinueWithOnMainThread(task => ProcessSitesSnapshot(task, onSuccess, onError));
    }

    /// <summary>
    ///     Process the sites we get from Firestore db to use it in the app.
    /// </summary>
    /// <param name="snapshotTask">The actuall snapshot task.</param>
    /// <param name="onSuccess">Callback if task success.</param>
    /// <param name="onError">Callback if task failure.</param>
    private void ProcessSitesSnapshot(Task<QuerySnapshot> snapshotTask, Action<List<SiteInfo>> onSuccess, Action<string> onError)
    {
        // Return the exception as string if failure
        if (snapshotTask.IsFaulted)
        {
            onError?.Invoke(snapshotTask.Exception.ToString());
            return;
        }

        // Create the sites list
        List<SiteInfo> sites = new List<SiteInfo>();
        // Loop through the Firestore Documents (sites)
        foreach (DocumentSnapshot document in snapshotTask.Result.Documents)
        {
            // Converting Firebase document to C# dictionary
            Dictionary<string, object> siteData = document.ToDictionary();
            // Create a site info object with the site data
            SiteInfo siteInfo = new SiteInfo
            {
                siteId = document.Id,

                siteName = siteData.ContainsKey("site_name")
                ? siteData["site_name"].ToString()
                : "Unknow name",

                siteDesc = siteData.ContainsKey("site_description")
                ? siteData["site_description"].ToString()
                : "Unknown description",

                siteModelId = siteData.ContainsKey("site_model_id")
                ? siteData["site_model_id"].ToString() 
                : "",
            };
            // Add to the site list
            sites.Add(siteInfo);
        }
        // Copy the sites list to cachedSites
        cachedSites = sites;

        onSuccess.Invoke(sites);
    }

    // API #2: Get features for a single site to render them in smart labels
    /// <summary>
    ///     API: Get all the features of a site.
    /// </summary>
    /// <param name="siteId">The site id.</param>
    /// <param name="onSuccess">Pass site features if success.</param>
    /// <param name="onError">Pass error if failure.</param>
    public void GetFeaturesForSite(string siteId, Action<List<FeatureData>> onSuccess, Action<string> onError)
    {
        // Get the cached (pre-warmed) Firestore instance we did in FirebaseInit
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        // Get the features collection in a site
        CollectionReference siteFeaturesCollection = db.Collection("heritage_sites").Document(siteId).Collection("site_features");
        siteFeaturesCollection.GetSnapshotAsync().ContinueWithOnMainThread(task => ProcessFeaturesSnapshot(task, onSuccess, onError));
    }

    /// <summary>
    ///     Process the site features to use the in the app.
    /// </summary>
    /// <param name="snapshotTask">The actual snapshot task.</param>
    /// <param name="onSuccess">Callback if task success.</param>
    /// <param name="onError">Callback if task failure.</param>
    /// <exception cref="NotImplementedException"></exception>
    private void ProcessFeaturesSnapshot(Task<QuerySnapshot> snapshotTask, Action<List<FeatureData>> onSuccess, Action<string> onError)
    {
        if (snapshotTask.IsFaulted)
        {
            onError(snapshotTask.Exception.ToString());
            return;
        }

        // Create the site features list
        List<FeatureData> siteFeatures = new List<FeatureData>();
        // Loop through the Firestore Documents (sites)
        foreach (DocumentSnapshot document in snapshotTask.Result.Documents)
        {
            // Converting Firestore object to C# dictionary
            Dictionary<string, object> featureData = document.ToDictionary();
            // Creating a new feature object to store each site feature document
            FeatureData siteFeature = new FeatureData();

            siteFeature.featureName = featureData.ContainsKey("feature_name")
            ? featureData["feature_name"].ToString()
            : "Unknown name";

            siteFeature.featureDesc = featureData.ContainsKey("feature_description")
            ? featureData["feature_description"].ToString()
            : "Unknown name";

            siteFeature.triggerVisibilityDist = featureData.ContainsKey("feature_trigger_visibility_distance")
            ? Convert.ToSingle(featureData["feature_trigger_visibility_distance"])
            : 1.5f;

            if (featureData.ContainsKey("feature_position"))
            {
                Dictionary<string, object> posMap = (Dictionary<string, object>)featureData["feature_position"];
                siteFeature.featurePos = new PositionData
                {
                    x = Convert.ToSingle(posMap["position_x"]),
                    y = Convert.ToSingle(posMap["position_y"]),
                    z = Convert.ToSingle(posMap["position_z"]),

                };
            }
            else siteFeature.featurePos = new PositionData { x = 0.0f, y = 0.0f, z = 0.0f };

            // Add to the site features list
            siteFeatures.Add(siteFeature);
        }

        onSuccess(siteFeatures);
    }
}

