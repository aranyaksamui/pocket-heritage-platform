using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;


/// <summary>
///     This script is attached to the Main Menu buttons for each heritage site. This script is responsible to either download or 
///     get the specific addressable from remote.
/// </summary>
public class SiteUIItem : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The name of the site")]
    [SerializeField] TMP_Text nameText;
    [Tooltip("The site_model_id of the site")]
    [SerializeField] TMP_Text siteModelIdText;
    [Tooltip("Button text switches from download / view")]
    [SerializeField] TMP_Text buttonText;
    [Tooltip("The actual action button (shows 'Download' if site model not downloaded else shows 'View'")]
    [SerializeField] Button actionButton;

    private long downloadSizeInBytes;

    SiteInfo mySiteData;


    /// <summary>
    ///     Setup the Main Menu site button with SiteInfo data.
    /// </summary>
    /// <param name="site">The site data.</param>
    public void Setup(SiteInfo site)
    {
        mySiteData = site;
        Debug.Log($"[SiteUIItem/Setup()] 4. Setting up button for: {mySiteData.siteId} {mySiteData.siteName}");
        // Filling the site data in the UI
        nameText.text = mySiteData.siteName;
        Debug.Log($"[SiteUIItem/Setup()] 5. Button name: {nameText.text}");
        siteModelIdText.text = mySiteData.siteModelId;


        actionButton.onClick.AddListener(OnActionBtnClick);
        // Immediately check if the model is download by getting it's download size
        CheckDownloadStatus();
    }

    /// <summary>
    ///     Check if the site model is downloaded (cached locally) or needs to be downloaded from the Remote host
    ///     by getting it's download size.
    /// </summary>
    private void CheckDownloadStatus()
    {
        Debug.Log($"[SiteUIItem/CheckDownloadStatus()] Checking download status...");
        Addressables.GetDownloadSizeAsync(mySiteData.siteModelId).Completed += (downloadSizeHandle) =>
        {
            if (downloadSizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                // downloadSizeInBytes is 0 if the site model is already downloaded or > 0 if needs to be downloaded
                downloadSizeInBytes = downloadSizeHandle.Result;
                Debug.Log($"[SiteUIItem/CheckDownloadStatus()] Site Addressable size: {downloadSizeInBytes}");
                UpdateUI();
            }
            else
            {
                Debug.LogError($"[SiteUIItem/CheckDownloadStatus()]\n{downloadSizeHandle.OperationException.ToString()}");
            }
            Addressables.Release(downloadSizeHandle);
        };
    }

    /// <summary>
    ///     Update the UI based on download size.
    /// </summary>
    private void UpdateUI()
    {
        if (downloadSizeInBytes > 0)
        {
            float sizeMB = downloadSizeInBytes / (1024f * 1024f);
            buttonText.text = $"Download ({sizeMB:F1})";
        }
        else
        {
            Debug.Log("[SiteUIItem/UpdateUI()] Model is already downloaded.");
            AREvents.OnSitesDownloaded.Invoke();
            buttonText.text = "View Model";
        }
    }

    // Event listener for the action button
    private void OnActionBtnClick()
    {
        // If the download size is > 0 then download the site model else enter the AR view of the site
        if (downloadSizeInBytes > 0) StartDownload();
        else EnterARView();
    }

    /// <summary>
    ///     Start the download of the site model from the Remote host if download size in > 0.
    /// </summary>
    private void StartDownload()
    {
        AREvents.OnLoadingStatusChanged(true, "Downloading site...");
        // Download the model
        Addressables.DownloadDependenciesAsync(mySiteData.siteModelId).Completed += (downloadDependenciesHandle) =>
        {
            if (downloadDependenciesHandle.Status == AsyncOperationStatus.Succeeded)
            {
                // Check download status
                CheckDownloadStatus();
            }
            else if (downloadDependenciesHandle.Status == AsyncOperationStatus.Failed)
                Debug.Log(downloadDependenciesHandle.OperationException.ToString());
            
            // Release handle once done
            Addressables.Release(downloadDependenciesHandle);
        };
    }

    /// <summary>
    ///     Enter the AR view if the site is already downloaded.
    /// </summary>
    private void EnterARView()
    {
        ActiveSiteContext.Instance.SelectSite(mySiteData);
    }
}
