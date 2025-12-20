using UnityEngine;
using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;

public class FirebaseInit : MonoBehaviour
{
    /// <summary>
    ///     Check if the Firebase setup is ready for connection.
    /// </summary>
    public static bool IsReady { get; private set; } = false;

    private void Start()
    {
        VerifyDependencies();
    }

    /// <summary>
    ///     Verification for the phone to have necessary Google Play Services
    /// </summary>
    private void VerifyDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(VerifyDependencyStatus);
    }

    /// <summary>
    ///     Callback function to process the task result of the dependency verification
    /// </summary>
    /// <param name="task"></param>
    private void VerifyDependencyStatus(Task<DependencyStatus> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError($"[FirebaseInit/VerifyDependencyStatus()] Firebase verify dependency failed!\n {task.Exception}");
            return;
        }
        // Get the status from the task result
        DependencyStatus status = task.Result;
        // Verify the status
        if (status == DependencyStatus.Available)
        {
            Debug.Log("[FirebaseInit/VerifyDependencyStatus()] Firebase app is Ready!!");
            // Initializing singletons for caching (pre-warming getters into memory so that whenever another scripts references the default instance it get it instantly)
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            var db = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
            IsReady = true;
        }
        else
        {
            Debug.LogError($"[FirebaseInit/VerifyDependencyStatus()] Could not connect to Firebase: {status}");
        }
    }
}
