using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace Utilities.Notification
{
    public class FirebaseValidator : MonoBehaviour
    {
        private FirebaseApp _app;
        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = FirebaseApp.DefaultInstance;
                    Debug.Log($"Firebase init : {_app.Options.ProjectId}");
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                } else {
                    Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }
}
