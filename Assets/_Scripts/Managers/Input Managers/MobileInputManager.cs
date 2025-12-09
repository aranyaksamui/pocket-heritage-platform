using UnityEngine;


/*
 * Summary:
 *      This manager class handles the mobile inputs.
 *      It provides a static singleton instance that initializes the generated C# class for the Unity new Input System.
*/
public class MobileInputManager : MonoBehaviour
{
    // The InputActions reference for global access
    public ARInputActions InputActions { get; private set; }
    // The input manager singleton
    public static MobileInputManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        InputActions = new ARInputActions();
    }
}
