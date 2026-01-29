using UnityEngine;
using System.Collections.Generic;


public class ActiveSiteContext : MonoBehaviour
{
    public static ActiveSiteContext Instance { get; private set; }

    public SiteInfo CurrentSiteData { get; private set; }                           // Site metadata
    public List<FeatureData> CurrentSiteFeatures { get; private set; }          // Site feature data
    public GameObject CurrentSiteObject { get; private set; }                   // Site model object


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

    /// <summary>
    ///     Set the metadata for selected site (except the featuers and model object).
    /// </summary>
    /// <param name="site">Site metadata</param>
    public void SelectSite(SiteInfo site)
    {
        CurrentSiteData = site;

        CurrentSiteFeatures = null;
        CurrentSiteObject = null;

        AREvents.OnSiteSelection.Invoke(CurrentSiteData);
    }

    /// <summary>
    ///     Set the site model object (Called by ARAssetLoader).
    /// </summary>
    /// <param name="siteObj">Site model object</param>
    public void SetSiteObject(GameObject siteObj)
    {
        CurrentSiteObject = siteObj;
    }

    public void SetSiteFeatures(List<FeatureData> features)
    {
        CurrentSiteFeatures = features;
    }

    /// <summary>
    ///     Clear the global active site context data.
    /// </summary>
    public void ClearContext()
    {
        // Notify the listeners
        AREvents.OnContextCleared.Invoke();
        // Wipe the data
        CurrentSiteData = null;
        CurrentSiteFeatures = null;
        CurrentSiteObject = null;
    }
}
