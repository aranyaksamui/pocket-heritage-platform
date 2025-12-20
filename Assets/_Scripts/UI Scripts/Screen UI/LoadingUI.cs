using UnityEngine;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] TMP_Text loadingMessage;

    Canvas loadingCanvas;

    private void Awake()
    {
        loadingCanvas = GetComponent<Canvas>();
        loadingCanvas.enabled = false;
    }

    /// <summary>
    ///     Set the loading text message in the loading screen canvas.
    /// </summary>
    /// <param name="message">Text message</param>
    public void SetLoadingMessage(string message)
    {
        loadingMessage.text = message;
    }
}
