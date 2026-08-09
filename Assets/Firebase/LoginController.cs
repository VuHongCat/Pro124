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
            ShowMessage("Lỗi hệ thống: Thiếu tham chiếu UI InputField.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter both email and password.");
            return;
        }

        if (!FirebaseManager.Instance.IsFirebaseReady)
        {
            ShowMessage("Firebase is not ready. Please try again.");
            return;
        }

        Login(email, password);
    }

    private void Login(string email, string password)
    {
        ShowMessage("Logging in...");

        FirebaseAuth auth = FirebaseManager.Instance.Auth;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = FirebaseErrorHelper.GetErrorMessage(task.Exception);
                ShowMessage(errorMessage);
                return;
            }

            AuthResult result = task.Result;
            FirebaseUser user = result.User;
            Debug.Log($"[Login] Login successful: {user.Email}");

            ShowMessage("Login successful!");

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