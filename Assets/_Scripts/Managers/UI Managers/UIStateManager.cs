using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

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
        AREvents.OnAssetLoadingStarted += HandleAssetLoading;
    }

    private void OnDisable()
    {
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
        AREvents.OnAssetLoadingStarted -= HandleAssetLoading;
    }


    // Initialize all the canvases
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
    // Call this event handler on object placed
    private void HandleObjectPlaced(GameObject placedObject)
    {
        ChangeState(UIState.Interaction);
    }
    
    // Call this event handler when loading asset after "Place" button clicked
    private void HandleAssetLoading()
    {
        ChangeState(UIState.Loading);
    }
}