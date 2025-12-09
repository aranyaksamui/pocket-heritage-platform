using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

/*
 * Summary:
 *      This class handles the heritage objects.
 *      It has helper functions for the heritage objects like placement in the AR world.
*/
public class WorldObjectsManager : MonoBehaviour
{
    [Tooltip("The heritage object to be placed and viewed in AR")]
    [SerializeField] GameObject heritageObjectPrefab;

    private ARRaycastManager raycastManager;
    private GameObject spawnedObject = null;
    //private Vector2 touchPosition;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector2 screenCenter;


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

    // Place a heritage site object into the AR world
    private void PlaceObject()
    {
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (spawnedObject == null && heritageObjectPrefab != null)
            {
                spawnedObject = Instantiate(heritageObjectPrefab, hitPose.position, hitPose.rotation);
                if (spawnedObject != null) AREvents.OnObjectPlaced?.Invoke(spawnedObject);
            }
            else Debug.LogError("[WorldObjectsManager/PlaceObject()] Hertiage object prefab is null or spawnedObject is not null");
        }
        else Debug.LogError("[WorldObjectsManager/PlaceObject()] Placement Failed! Point you camera to a flat detected surface");
    }
}
