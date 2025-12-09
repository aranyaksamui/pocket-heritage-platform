using UnityEngine;

/*
 * Summary:
 *      This static class stores all the events and event handlers in the application
*/
public static class AREvents
{
    // Placement phase events
    public static System.Action<bool> OnPlanesDetectionChanged;
    public static System.Action OnPlaceObjectRequested;
    public static System.Action<GameObject> OnObjectPlaced;

    // Interaction phase events
    public static System.Action OnObjectZoomRequested;
    public static System.Action OnObjectRotationRequested;


    // Navigation phase events

    // UI State management
    public static System.Action<UIState> OnUIStateChange;

    // Hotspots & interaction events
}
