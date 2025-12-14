using UnityEngine;
using UnityEngine.XR.ARFoundation;


/*
 * Summary:
 *      This class handles the AR session state and data.
 *      AR session is like the brain of the AR engine.
*/      
public class ARSessionManager : MonoBehaviour
{
    [Header("Gameobject References")]
    [Tooltip("The plane manager on AR Origin that enables plane detection")]
    [SerializeField] ARPlaneManager arPlaneManager;

    private bool arePlanesDetected = false;

    private void OnEnable()
    {
        ARSession.stateChanged += OnSessionStateChange;
        if (arPlaneManager != null)
            arPlaneManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        ARSession.stateChanged -= OnSessionStateChange;
        if (arPlaneManager != null)
            arPlaneManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    // Call this event handler on state change in AR Session
    private void OnSessionStateChange(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking)
            Debug.Log("[ARSessionManager/OnSessionStateChange()] AR session is now tracking environment");
        else if (args.state == ARSessionState.SessionInitializing)
            Debug.Log("AR session is initializing");
    }

    // Call this event handler on plane detection
    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        if (!arePlanesDetected && changes.added.Count > 0)
        {
            Debug.Log("[ARSessionManager/OnTrackablesChanged()] First planes detected!");
            arePlanesDetected = true;
            // Update the placenment hints in placement UI if planes are detected
            AREvents.OnPlanesDetectionChanged.Invoke(arePlanesDetected);
        }
    }
}
