using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }
    public FirebaseUser CurrentUser => Auth != null ? Auth.CurrentUser : null;
    public bool IsFirebaseReady { get; private set; } = false;

    private void Awake()
    {
        // Singleton pattern - giữ FirebaseManager xuyên suốt các scene
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"[FirebaseManager] Không thể khởi tạo Firebase: {task.Exception}");
                return;
            }

            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                Firestore = FirebaseFirestore.DefaultInstance;
                IsFirebaseReady = true;
                Debug.Log("[FirebaseManager] Firebase khởi tạo thành công.");
                CheckCurrentUser();
            }
            else
            {
                Debug.LogError($"[FirebaseManager] Lỗi dependency: {dependencyStatus}");
            }
        });
    }

    private void CheckCurrentUser()
    {
        FirebaseUser currentUser = Auth.CurrentUser;
        if (currentUser != null)
        {
            Debug.Log($"[FirebaseManager] Người dùng đã đăng nhập sẵn: {currentUser.Email}");
            // SceneLoader.TransitionTo("MainMenu");
        }
    }

    public void SignOut()
    {
        Auth.SignOut();
        Debug.Log("[FirebaseManager] Đã đăng xuất.");
        SceneLoader.TransitionTo("Login");
    }
}