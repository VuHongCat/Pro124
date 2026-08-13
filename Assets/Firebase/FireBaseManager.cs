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

    private bool _reportedInstanceMismatch;

    private void Awake()
    {
        // Singleton pattern - keeps FirebaseManager across scenes
        if (Instance != null)
        {
            Debug.LogWarning(
                $"[FirebaseManager] Duplicate Awake (id={GetInstanceID()}, go={gameObject.name}). " +
                $"Existing Instance id={Instance.GetInstanceID()}, go={Instance.gameObject.name}. Disabling this duplicate component only."
            );
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log(
            $"[FirebaseManager] Awake: Instance set (id={GetInstanceID()}, go={gameObject.name}). " +
            $"Scene: {gameObject.scene.name}"
        );

        InitializeFirebase();
    }

    private void Update()
    {
        // Watchdog: fires once if Instance is ever lost or replaced during play,
        // which is exactly the state that makes Register/Login report "not ready".
        if (!_reportedInstanceMismatch && (Instance == null || Instance != this))
        {
            _reportedInstanceMismatch = true;
            Debug.LogWarning(
                $"[FirebaseManager] WATCHDOG: Instance mismatch detected. " +
                $"Instance==null: {Instance == null}, Instance!=this: {Instance != this}, " +
                $"this destroyed: {this == null}, go destroyed: {gameObject == null}. Stack:\n" +
                System.Environment.StackTrace
            );
        }
    }

    private void OnDestroy()
    {
        bool wasInstance = Instance == this;
        Debug.LogWarning(
            $"[FirebaseManager] OnDestroy (id={GetInstanceID()}, go={gameObject.name}). " +
            $"Instance still points here: {wasInstance}. Stack:\n" +
            System.Environment.StackTrace
        );
        if (wasInstance)
            Instance = null;
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"[FirebaseManager] Failed to initialize Firebase: {task.Exception}");
                return;
            }

            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                Firestore = FirebaseFirestore.DefaultInstance;
                IsFirebaseReady = true;
                Debug.Log("[FirebaseManager] Firebase initialized successfully.");
                CheckCurrentUser();
            }
            else
            {
                Debug.LogError($"[FirebaseManager] Dependency error: {dependencyStatus}");
            }
        });
    }

    private void CheckCurrentUser()
    {
        FirebaseUser currentUser = Auth.CurrentUser;
        if (currentUser != null)
        {
            Debug.Log($"[FirebaseManager] User already signed in: {currentUser.Email}");
            // SceneLoader.TransitionTo("MainMenu");
        }
    }

    public void SignOut()
    {
        Auth.SignOut();
        Debug.Log("[FirebaseManager] Signed out.");
        SceneLoader.TransitionTo("Login");
    }
}