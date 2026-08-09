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

    public void OnRegisterButtonClicked()
    {
        if (emailInput == null || passwordInput == null)
        {
            ShowMessage("Lỗi hệ thống: Thiếu tham chiếu UI InputField.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput != null ? confirmPasswordInput.text : password;

        // Kiểm tra dữ liệu đầu vào cơ bản
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Vui lòng nhập đầy đủ Email và Mật khẩu.");
            return;
        }

        if (confirmPasswordInput != null)
        {
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Vui lòng nhập lại mật khẩu để xác nhận.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowMessage("Mật khẩu xác nhận không khớp.");
                return;
            }
        }

        if (password.Length < 6)
        {
            ShowMessage("Mật khẩu phải có ít nhất 6 ký tự.");
            return;
        }

        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady)
        {
            ShowMessage("Firebase chưa sẵn sàng, vui lòng thử lại.");
            return;
        }

        Register(email, password);
    }

    private void Register(string email, string password)
    {
        SetInteractable(false);
        ShowMessage("Đang tạo tài khoản...");

        FirebaseAuth auth = FirebaseManager.Instance.Auth;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            SetInteractable(true);

            if (task.IsCanceled || task.IsFaulted)
            {
                string errorMessage = FirebaseErrorHelper.GetErrorMessage(task.Exception);
                ShowMessage(errorMessage);
                return;
            }

            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.Log($"[Register] Tạo tài khoản thành công: {newUser.Email}");

            if (usernameInput != null && !string.IsNullOrWhiteSpace(usernameInput.text))
            {
                UserProfile profile = new UserProfile { DisplayName = usernameInput.text.Trim() };
                newUser.UpdateUserProfileAsync(profile);
            }

            ShowMessage("Đăng ký thành công! Đang chuyển sang màn hình đăng nhập...");

            if (panelSwitcher != null)
                panelSwitcher.openloginpanel();
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
        // Thêm logic chuyển Panel hoặc chuyển Scene tại đây
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
        Debug.Log($"[Register] {msg}");
    }
}
