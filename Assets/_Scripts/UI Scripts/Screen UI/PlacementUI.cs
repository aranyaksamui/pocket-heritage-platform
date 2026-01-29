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
    private string selectedSite;

    private void Awake()
    {
        placementUICanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        placeButton.onClick.AddListener(OnPlaceButtonClick);
        // Subscribe to AREvents
        AREvents.OnPlanesDetectionChanged += UpdatePlacementHint;
        AREvents.OnSiteSelection += HandleSiteSelected;
    }

    private void OnDisable()
    {
        placeButton.onClick.RemoveAllListeners();
        // Unsubscribe from AREvents
        AREvents.OnPlanesDetectionChanged -= UpdatePlacementHint;
        AREvents.OnSiteSelection -= HandleSiteSelected;
    }

    // Call this to update UI when planes are detected
    public void UpdatePlacementHint(bool planesDetected)
    {
        Debug.Log("[PlacementUI/UpdatePlacementHint()] plane detection working");
        if (placementUICanvas != null && !placementUICanvas.enabled) return;
        if (planesDetected && selectedSite.Length > 0)
        {
            placementText.text = $"Site: {selectedSite} - Flat surface detected. Click the place button!";
            placeButton.interactable = true;
        }
        else
        {
            placementText.text = $"Site: {selectedSite} - Point your camera to a flat surface.";
            placeButton.interactable = false;
        }
    }

    public void HandleSiteSelected(SiteInfo site)
    {
        selectedSite = site.siteId;
    }

    // Place button function
    private void OnPlaceButtonClick()
    {
        AREvents.OnPlaceObjectRequested.Invoke();
    }
}
