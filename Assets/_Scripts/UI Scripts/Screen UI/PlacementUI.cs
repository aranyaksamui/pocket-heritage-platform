using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacementUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] TMP_Text placementText;
    [SerializeField] Button placeButton;
    [SerializeField] Image crosshairImage;

    private Canvas placementUICanvas;

    private void Awake()
    {
        placementUICanvas = GetComponent<Canvas>();
        placementUICanvas.enabled = true;
    }

    private void OnEnable()
    {
        placeButton.onClick.AddListener(OnPlaceButtonClick);
        // Subscribe to AREvents
        AREvents.OnPlanesDetectionChanged += UpdatePlacementHint;
    }

    private void OnDisable()
    {
        placeButton.onClick.RemoveAllListeners();
        // Unsubscribe from AREvents
        AREvents.OnPlanesDetectionChanged -= UpdatePlacementHint;
    }

    // Call this to update UI when planes are detected
    public void UpdatePlacementHint(bool planesDetected)
    {
        if (planesDetected)
        {
            placementText.text = "Flat surface detected. Click the place button!";
            placeButton.interactable = true;
        }
        else
        {
            placementText.text = "Point your camera to a flat surface.";
            placeButton.interactable = false;
        }
    }

    // Place button function
    private void OnPlaceButtonClick()
    {
        AREvents.OnPlaceObjectRequested.Invoke();
    }
}
