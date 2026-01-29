using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This manager class handles the UI change and updatation based on user interaction with the UI.
/// </summary>
public class UIStateManager : MonoBehaviour
{
    [Tooltip("Select the starting UI state (UI to show when the app starts")]
    [SerializeField] UIState currentState = UIState.MainMenu;

    [Header("State canvases")]
    [Tooltip("The heritage main menu UI")]
    [SerializeField] Canvas mainMenuCanvas;
    [Tooltip("The heritage placement UI")]
    [SerializeField] Canvas placementCanvas;
    [Tooltip("The heritage object interaction UI")]
    [SerializeField] Canvas interactionCanvas;
    [Tooltip("The heritage object navigation UI")]
    [SerializeField] Canvas navigationCanvas;
    [Tooltip("The heritage object information UI")]
    [SerializeField] Canvas infoCanvas;
    [Tooltip("Loading UI")]
    [SerializeField] Canvas loadingCanvas;

    private Dictionary<UIState, Canvas> stateCanvases;
    private Stack<UIState> uiHistory;


    /// <summary>
    ///     Singleton instance of the UIStateManager
    /// </summary>
    public static UIStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        InitializeCanvases();
    }

    private void OnEnable()
    {
        // Button listeners subscribe
        AREvents.OnBackButtonClicked += GoBack;
        AREvents.OnReturnToSiteMenu += ReturnToSiteMenu;
        // State change listeners subscribe
        AREvents.OnSitesDownloaded += HandleSitesDownloaded;
        AREvents.OnSitesLoadedAndListPopulated += HandleSitesPopulated;
        AREvents.OnSiteSelection += HandleSiteSelected;
        AREvents.OnObjectPlaced += HandleObjectPlaced;
        AREvents.OnLoadingStatusChanged += HandleLoadingStatus;
        AREvents.OnFeatureLabelsSpawned += HandleFeatureLabelSpawned;
    }

    private void OnDisable()
    { 
        // Button listeners unsubscribe
        AREvents.OnBackButtonClicked -= GoBack;
        AREvents.OnReturnToSiteMenu -= ReturnToSiteMenu;
        // State change listeners unsubscribe
        AREvents.OnSitesDownloaded -= HandleSitesDownloaded;
        AREvents.OnSitesLoadedAndListPopulated -= HandleSitesPopulated;
        AREvents.OnSiteSelection -= HandleSiteSelected;
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
        AREvents.OnLoadingStatusChanged -= HandleLoadingStatus;
        AREvents.OnFeatureLabelsSpawned -= HandleFeatureLabelSpawned;
    }

    /// <summary>
    ///     Initializes all the app canvases.
    /// </summary>
    private void InitializeCanvases()
    {
        if (loadingCanvas == null || mainMenuCanvas == null || placementCanvas == null || interactionCanvas == null) return;
        stateCanvases = new Dictionary<UIState, Canvas>
        {
            { UIState.MainMenu, mainMenuCanvas },
            { UIState.Placement, placementCanvas },
            { UIState.Interaction, interactionCanvas },
            { UIState.Navigation, navigationCanvas },
            { UIState.Info, infoCanvas },
            { UIState.Loading, loadingCanvas },
        };

        uiHistory = new Stack<UIState>();
    }

    /// <summary>
    ///     Change the UI state to a new state
    /// </summary>
    /// <param name="newState">The next UI state</param>
    /// <param name="addToHistory">Going forward add the current UI to history. Going backward do not add the current UI to history</param>
    private void ChangeState(UIState newState, bool addToHistory = true)
    {
        // Gurad clause: prevent state change if new state is current state itself
        if (newState == currentState) return;
        if (addToHistory && currentState != UIState.Loading)
        {
            uiHistory.Push(currentState);
            foreach (UIState state in uiHistory) Debug.Log($"[UIStateManager/ChangeState()] {state.ToString()}");
        }
        // Store previous state for transition logic (back button)
        UIState previousState = currentState;
        currentState = newState;
        // Update the canvas visibility
        UpdateCanvasVisibility(previousState, newState);

        Debug.Log($"[UIStateManager/ChangeState()] state changed from {previousState} to {currentState}");
    }

    /// <summary>
    ///     Update the canvas visibility on canvas state change.
    /// </summary>
    /// <param name="previousState">Previous or current UI state to be updated.</param>
    /// <param name="newState">Next UI state.</param>
    private void UpdateCanvasVisibility(UIState previousState, UIState newState)
    {
        if (stateCanvases.ContainsKey(previousState) && stateCanvases[previousState] != null)
            stateCanvases[previousState].enabled = false;
        if (stateCanvases.ContainsKey(newState) && stateCanvases[newState])
            stateCanvases[newState].enabled = true;
    }

    /// <summary>
    ///     Go back to the previous UI state.
    /// </summary>
    private void GoBack()
    {
        // If current state is loading, do not add it to the stack (interact with it because it might mess up downloads)
        if (currentState == UIState.Loading) return;
        // If there is a history: go back else: There is no UI history
        if (uiHistory.Count > 0)
        {
            UIState previousState = uiHistory.Pop();
            ChangeState(previousState, false);
        }
        else
        {
            Debug.Log("[UIStateManager/GoBack()] You are already at the root. There is no UI History.");
        }
    }

    /// <summary>
    ///     Call this event handler when Return to Main Menu / Home button is pressed.
    /// </summary>
    private void ReturnToSiteMenu()
    {
        // Clear the UI stack
        uiHistory.Clear();
        // Clear the active context
        ActiveSiteContext.Instance.ClearContext();
        // Change the UI State to main menu
        ChangeState(UIState.MainMenu, false);
    }

    // State change methods
    /// <summary>
    ///     Changes state to Main Menu when sites are downloaded.d
    /// </summary>
    private void HandleSitesDownloaded()
    {
        ChangeState(UIState.MainMenu);
    }

    /// <summary>
    ///     Changes state to Main Menu when sites list is ready.
    /// </summary>
    private void HandleSitesPopulated()
    {
        ChangeState(UIState.MainMenu);
    }

    /// <summary>
    ///     Handles site selection and changes UI state from mainmenu to placement.
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="siteModelId"></param>
    private void HandleSiteSelected(SiteInfo site)
    {
        Debug.Log($"[UIStateManager/HandleSiteSelected] Selected site: {site.siteId}");
        ChangeState(UIState.Placement);
    }

    /// <summary>
    ///     Change UI state from placement to interaction on object placed.
    /// </summary>
    private void HandleObjectPlaced(GameObject placedObject)
    {
        ChangeState(UIState.Interaction);
    }

    /// <summary>
    ///     Call this event handler when loading state is invoked.
    /// </summary>
    /// <param name="status">If true set the loading message.</param>
    /// <param name="message">The loading message.</param>
    private void HandleLoadingStatus(bool status, string message)
    {
        if (status)
        {
            loadingCanvas.gameObject.GetComponent<LoadingUI>().SetLoadingMessage(message);
            ChangeState(UIState.Loading);
        }
    }
    /// <summary>
    ///     Call this event handler when feature labels are successfully spawned.
    /// </summary>
    private void HandleFeatureLabelSpawned()
    {
        ChangeState(UIState.Interaction);
    }
}