using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    Canvas interactionUICanvas;

    private void Awake()
    {
        interactionUICanvas = GetComponent<Canvas>();
        interactionUICanvas.enabled = false;
    }
}
