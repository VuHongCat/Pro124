using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FireBaseAuth : MonoBehaviour
{
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase đã sẵn sàng!");
            }
            else
            {
                Debug.LogError($"Không thể tải Firebase: {dependencyStatus}");
            }
        });
    }
}
