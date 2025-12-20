using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This manager class handles the UI change and updatation based on user interaction with the UI.
/// </summary>
public class UIStateManager : MonoBehaviour
{
    [SerializeField] UIState currentState = UIState.Placement;

    [Header("State canvases")]
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

    Dictionary<UIState, Canvas> stateCanvases;


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
        AREvents.OnObjectPlaced += HandleObjectPlaced;
        AREvents.OnLoadingStatusChanged += HandleLoadingStatus;
        AREvents.OnFeatureLabelsSpawned += HandleFeatureLabelSpawned;
    }

    private void OnDisable()
    {
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
        AREvents.OnLoadingStatusChanged -= HandleLoadingStatus;
        AREvents.OnFeatureLabelsSpawned -= HandleFeatureLabelSpawned;
    }

    /// <summary>
    ///     Initializes all the app canvases.
    /// </summary>
    private void InitializeCanvases()
    {
        if (placementCanvas == null || interactionCanvas == null) return;
        stateCanvases = new Dictionary<UIState, Canvas>
        {
            { UIState.Placement, placementCanvas },
            { UIState.Interaction, interactionCanvas },
            { UIState.Navigation, navigationCanvas },
            { UIState.Info, infoCanvas },
            { UIState.Loading, loadingCanvas },
        };
    }

    /// <summary>
    ///     Change the UI state to a new state
    /// </summary>
    /// <param name="newState">The next UI state</param>
    public void ChangeState(UIState newState)
    {
        // Gurad clause: prevent state change if new state is current state itself
        if (newState == currentState) return;
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
    public void UpdateCanvasVisibility(UIState previousState, UIState newState)
    {
        if (stateCanvases.ContainsKey(previousState) && stateCanvases[previousState] != null)
            stateCanvases[previousState].enabled = false;
        if (stateCanvases.ContainsKey(newState) && stateCanvases[newState])
            stateCanvases[newState].enabled = true;
    }

    // State change methods
    /// <summary>
    ///     Change UI state on object placed.
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