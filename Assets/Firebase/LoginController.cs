using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    [Header("Scene Settings")]
    public string gameSceneName = "MainMenu";

    public void OnLoginButtonClicked()
    {
        if (emailInput == null || passwordInput == null)
        {
            ShowMessage("System error: Missing UI InputField reference.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter both email and password.");
            return;
        }

        if (!LocalAuthManager.IsValidEmail(email))
        {
            ShowMessage("Invalid email format.");
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("Password must be at least 6 characters.");
            return;
        }

        Login(email, password);
    }

    private void Login(string email, string password)
    {
        if (!LocalAuthManager.CheckLogin(email, password))
        {
            ShowMessage("Invalid email or password.");
            return;
        }

        Debug.Log($"[Login] Login successful: {email}");
        ShowMessage("Login successful!");

        SignInFirebaseAndLoad(email, password);
    }

    private void SignInFirebaseAndLoad(string email, string password)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady)
        {
            SceneLoader.TransitionTo(gameSceneName);
            return;
        }

        FirebaseManager.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogWarning($"[Login] Firebase sign-in skipped: {task.Exception}");
                SceneLoader.TransitionTo(gameSceneName);
                return;
            }

            CloudSave.Load(hasSave =>
            {
                if (hasSave)
                    ShowMessage("Loaded saved progress!");
                SceneLoader.TransitionTo(gameSceneName);
            });
        });
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
        Debug.Log($"[Login] {msg}");
    }
}
