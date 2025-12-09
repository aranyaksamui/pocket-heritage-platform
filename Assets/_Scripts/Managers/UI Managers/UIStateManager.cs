using UnityEngine;
using System.Collections.Generic;

/*
 * Summary:
 *      This manager class handles the UI change and updatation based on user interaction with the UI.
*/
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

    Dictionary<UIState, Canvas> stateCanvases;


    // Singleton instance of the UIStateManager
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
    }

    private void OnDisable()
    {
        AREvents.OnObjectPlaced -= HandleObjectPlaced;
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
        };
    }

    // Change state to a new state
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

    // Update the canvas visibility on canvas state change
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
}
