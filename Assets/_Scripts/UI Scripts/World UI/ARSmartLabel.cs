using TMPro;
using UnityEngine;

/* 
 * Summary:
 *      This class manages the Smart Labels feature.
 *      Smart labels anchor/points to each of the distinct features of the heritage site (3D Model).
 *      It gives the user with description and back stories of the feature it is pointing to.
 *      For example a antique fountain in the heritage site.
*/
public class ARSmartLabel : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The UI label box")]
    [SerializeField] GameObject labelContentHolder;
    [Tooltip("The line that points to the features")]
    [SerializeField] LineRenderer labelLine;
    [Tooltip("Distance at which the label will be visible")]
    [SerializeField] float triggerVisibilityDistance = 1.5f;

    [Header("Visuals")]
    [Tooltip("")]
    [SerializeField] bool scaleWithDistance = false;
    [Tooltip("Width of the label line at the start")]
    [SerializeField] float lineStartWidth = 0.01f;
    [Tooltip("Width of the label line at the end")]
    [SerializeField] float lineEndWidth = 0.01f;

    [Header("Label texts")]
    [Tooltip("Feature name")]
    [SerializeField] TMP_Text featureName;
    [Tooltip("Feature description")]
    [SerializeField] TMP_Text featureDescription;


    private Transform camTransform;
    private FeatureData featureData;

    private void Awake()
    {
        labelLine = GetComponent<LineRenderer>();
    }

    void Start()
    {
        camTransform = Camera.main.transform;
        // Hide the UI label initialy
        labelContentHolder.SetActive(false);
        // Set the no. of vertices of the label line
        labelLine.positionCount = 2;
        // Set the start and end widht of the label line
        labelLine.startWidth = lineStartWidth;
        labelLine.endWidth = lineEndWidth;
    }

    void Update()
    {
        HandleLabelVisibility();
    }

    // Initialize the smart label with the site feature data and set the position of the label
    public void InitilizeSmartLabel(FeatureData siteFeatureData, Vector3 labelPosition)
    {
        featureData = siteFeatureData;
        // Set the feature data
        triggerVisibilityDistance = featureData.triggerVisibilityDist;
        if (featureName != null && featureDescription != null)
        {
            featureName.text = featureData.featureName;
            featureDescription.text = featureData.featureDesc;
        }
        else Debug.LogError("[ARSmartLabel/InitializeSmartLabel()] feature name or description is null!");
        // Set the local position of the label relative to the parent object
        transform.localPosition = labelPosition;
    }

    // Handle the feature label's visibility based on distance from feature to camera
    void HandleLabelVisibility()
    {
        if (camTransform == null) return;
        // Get the camera distance from the camera's current postion to the target feature's position
        float camDistance = Vector3.Distance(transform.position, camTransform.position);
        if (camDistance <= triggerVisibilityDistance)
        {
            labelLine.enabled = true;
            if (!labelContentHolder.activeSelf) labelContentHolder.SetActive(true);
            UpdateLabelVisuals();
        }
        else
        {
            labelContentHolder.SetActive(false);
            labelLine.enabled = false;
        }
    }

    // Update the label's visuals like billboarding and the label pointer line
    void UpdateLabelVisuals()
    {
        // Label billboarding
        // Argument 1: Calculates a point parallel to those laser beams but starting from the UI label. It forces the label to look in the exact same direction the camera is looking
        // Argument 2: We force the UI to match the tilt of the camera
        labelContentHolder.transform.LookAt(labelContentHolder.transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);

        // Set the two vertices that would point to the target feature from the UI Label
        if (labelLine.enabled)
        {
            labelLine.SetPosition(0, transform.position);
            labelLine.SetPosition(1, labelContentHolder.transform.position);
        }
    }
}
