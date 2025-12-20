using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
///     This manager class is responsible to load and instantiate the heritage site model asset on demand using Unity Addressables.
/// </summary>
public class ARAssetLoader : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The address of the asset in Addressables group (Default Local Group)")]
    [SerializeField] string assetAddress = "site_model_tajmahal";

    private GameObject loadedAssetObject;

    /// <summary>
    ///     Loads and places the site model prefab asset asynchronously using the asset's location address.
    /// </summary>
    /// <param name="position">Position of the site model</param>
    /// <param name="rotation">Rotation of the site model</param>
    public void LoadAndPlaceAsset(Vector3 position, Quaternion rotation)
    {
        // Invoke the loading screen
        AREvents.OnLoadingStatusChanged.Invoke(true, "Loading asset...");
        Debug.Log($"[ARAssetLoad/LoadAndPlaceAsset()] Loading asset: {assetAddress}");
        // Load the model using the asset's adderss and instantiate it
        Addressables.InstantiateAsync(assetAddress, position, rotation).Completed += OnAssetLoaded;
    }

    /// <summary>
    ///     Callback handler for when the asset is finished loading.
    /// </summary>
    /// <param name="handle">The internal async operations snapshot after the asset loading is resolved or failed</param>
    public void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[ARAssetLoader/OnAssetLoaded()] Asset successfully loaded");
            // Get the loaded asset object and store it for future references
            loadedAssetObject = handle.Result;
            // Notify the subscribers in event bus (AREvents)
            AREvents.OnObjectPlaced.Invoke(loadedAssetObject);
        }
        else
        {
            Debug.LogError("[ARAssetLoader/OnAssetLoaded()] Asset loading failed");
        }
    }

    /// <summary>
    ///     Unloads the asset and it's references from memory.
    /// </summary>
    public void CleanUpLoadedAsset()
    {
        if (loadedAssetObject != null)
        {
            Addressables.ReleaseInstance(loadedAssetObject);
        }
    }
}
