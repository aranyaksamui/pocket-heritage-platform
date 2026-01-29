using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     This script is attached to the home button to return to the Main Menu UI
/// </summary>
public class ReturnToMenu : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(HandleReturnToSiteMenuClick);
    }

    // Fire the return to site menu button to go back to main menu
    private void HandleReturnToSiteMenuClick()
    {
        AREvents.OnReturnToSiteMenu.Invoke();
    }
}
