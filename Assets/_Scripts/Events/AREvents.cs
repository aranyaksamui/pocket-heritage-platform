using UnityEngine;


/// <summary>
///     This static class is the central event bus of the application.
///     It stores all the events and event handlers in the application.
/// </summary>
public static class AREvents
{
    // Placement phase events
    /// <summary>
    ///     This raises an event when the AR planes are detected.
    /// </summary>
    public static System.Action<bool> OnPlanesDetectionChanged;

    /// <summary>
    ///     This raises an event when the user clicks the place button to place the model in the AR world.
    /// </summary>
    public static System.Action OnPlaceObjectRequested;

    /// <summary>
    ///     This raises an event when the user clicks the place button to place the model in the AR world.
    /// </summary>
    public static System.Action<GameObject> OnObjectPlaced;


    // Interaction phase events
    /// <summary>
    ///     This raises an event when the user clicks a feature label icon.
    /// </summary>
    public static System.Action<FeatureData> OnFeatureClicked; //TODO: Implement feature label icon click
    /// <summary>
    ///     This raises an event when the feature data is present and the feature labels are successfully spawned in the AR world.
    /// </summary>
    public static System.Action OnFeatureLabelsSpawned;

    // Navigation phase events

    // UI State management
    ///// <summary>
    /////     This raises an event when the user clicks a feature label icon.
    ///// </summary>
    //public static System.Action<UIState> OnUIStateChange;

    // Hotspots & interaction events

    // Loading events
    /// <summary>
    ///     This raises an event when the asset gets loaded.
    /// </summary>
    public static System.Action<bool, string> OnLoadingStatusChanged;

    // Misc
    public static System.Action OnSitesDownloaded;
    /// <summary>
    ///     This raises an event when the heritage sites are loaded from the GetAllSites API and the Main Menu site list is populated.
    /// </summary>
    public static System.Action OnSitesLoadedAndListPopulated;
    /// <summary>
    ///     This raises an event when the user presses the back button in the UI.
    /// </summary>
    public static System.Action OnBackButtonClicked;
    /// <summary>
    ///     This raises an event when the Return to Main Menu or Home button is clicked
    /// </summary>
    public static System.Action OnReturnToSiteMenu;

    // Active site context
    /// <summary>
    ///     This raises and event when a site is selected in the Main Menu using the SiteUIItem buttons.
    /// </summary>
    public static System.Action<SiteInfo> OnSiteSelection;
    /// <summary>
    ///     This raises an event when the data for current active site is cleared.
    /// </summary>
    public static System.Action OnContextCleared;

}
