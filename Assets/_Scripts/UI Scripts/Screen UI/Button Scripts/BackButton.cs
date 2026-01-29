using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBackButtonClick);
    }

    // Fire the back button
    private void OnBackButtonClick()
    {
        AREvents.OnBackButtonClicked.Invoke();
    }
}
