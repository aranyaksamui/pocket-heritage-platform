using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

/// <summary>
///     Provides helper functions for the heritage object.
///     For example: Method to place the heritage model object.
/// </summary>
public class WorldObjectsManager : MonoBehaviour
{
    [Tooltip("AR asset manager that loads the assets using Unity addressables")]
    [SerializeField] ARAssetLoader assetLoader;

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector2 screenCenter;
    private bool isObjectSpawned = false;


    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    private void OnEnable()
    {
        // Subscribe to AREvents
        AREvents.OnPlaceObjectRequested += PlaceObject;

    }
    
    private void OnDisable()
    {
        // Unsubscribe to AREvents
        AREvents.OnPlaceObjectRequested -= PlaceObject;
    }

    private void Start()
    {
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    /// <summary>
    ///     Place a heritage site object into the AR world. Fires when user clicks the "Place" button.
    /// </summary>
    private void PlaceObject()
    {
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (!isObjectSpawned)
            {
                assetLoader.LoadAndPlaceAsset(hitPose.position, hitPose.rotation);
                isObjectSpawned = true;
            }
            else Debug.LogError("[WorldObjectsManager/PlaceObject()] Model object already spawned");
        }
        else Debug.LogError("[WorldObjectsManager/PlaceObject()] Placement Failed! Point you camera to a flat detected surface");
    }
}
