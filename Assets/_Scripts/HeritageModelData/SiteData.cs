using System;
using System.Collections.Generic;


/// <summary>
///     Summary:
///     The primary site data with keys: "siteName", "siteDescription" and "features".
///     Stores all the historical data about a heritage site. 
/// </summary>
[Serializable]
public class SiteData
{
    public string siteName;
    public string siteDesc;
    public List<FeatureData> features;
}

/// <summary>
///     The feature data with keys: "featureTitle", "featureDesc" and "featurePos".
///     Stores the title, description and the position (relative to the parent gameobject) of a disctinct feature in the heritage site.
///     One heritage site can have multiple features. For example: A fountain and a gate.
/// </summary>
[Serializable]
public class FeatureData
{
    public string featureName;
    public string featureDesc;
    public PositionData featurePos;
    public float triggerVisibilityDist;
}

/// <summary>
///     Stores positions of each of the disctinct features in the heritage site model relative to the parent gameobject".
///     With keys: x, y and z.
/// </summary>
[Serializable]
public class PositionData
{
    public float x;
    public float y;
    public float z;
}
