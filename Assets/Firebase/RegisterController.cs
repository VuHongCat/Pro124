using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField usernameInput;
    public TMP_Text messageText;
    public Button registerButton;

    [Header("Panel Switching")]
    public panelswtich panelSwitcher;
    private void Start()
    {
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    public void OnRegisterButtonClicked()
    {
        if (emailInput == null || passwordInput == null)
        {
            ShowMessage("System error: Missing UI InputField reference.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput != null ? confirmPasswordInput.text : password;

        string emailError = LocalAuthManager.ValidateEmail(email);
        if (emailError != null)
        {
            ShowMessage(emailError);
            return;
        }

        string passError = LocalAuthManager.ValidatePassword(password);
        if (passError != null)
        {
            ShowMessage(passError);
            return;
        }

        if (confirmPasswordInput != null)
        {
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Please re-enter your password to confirm.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("Passwords do not match.");
                return;
            }
        }

        Register(email, password);
    }

    private void Register(string email, string password)
    {
        SetInteractable(false);
        ShowMessage("Creating account...");

        string result = LocalAuthManager.RegisterUser(email, password, out bool success);
        SetInteractable(true);

        if (!success)
        {
            ShowMessage(result);
            return;
        }

        Debug.Log($"[Register] Account created successfully: {email}");
        ShowMessage("Registration successful! Redirecting to login...");

        SyncFirebaseAccount(email, password);

        if (panelSwitcher != null)
            panelSwitcher.openloginpanel();
    }

    private void SyncFirebaseAccount(string email, string password)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady)
            return;

        FirebaseManager.Instance.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
                Debug.Log($"[Register] Firebase sync skipped (account may already exist): {task.Exception.Message}");
            else
                Debug.Log("[Register] Firebase account synced.");
        });
    }

    private void SetInteractable(bool state)
    {
        if (registerButton != null) registerButton.interactable = state;
        if (emailInput != null) emailInput.interactable = state;
        if (passwordInput != null) passwordInput.interactable = state;
        if (confirmPasswordInput != null) confirmPasswordInput.interactable = state;
    }

    private void GoToLogin()
    {
        // Add panel switching or scene switching logic here
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = msg;
        }
        Debug.Log($"[Register] {msg}");
    }
}
