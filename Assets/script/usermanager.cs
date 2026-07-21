
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
public class usermanager : MonoBehaviour
{
    public bool Login(string username, string password, out string message)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            message = "vui lòng nhập đủ thông tin";
            return false;
        }
        message = "đăng nhập thành công";
        return true;
    }
    public bool Register(string username, string email, string password, out string message)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            message = "vui lòng nhập đủ thông tin";
            return false;
        }
        if (!email.Contains("@"))
        {
            message = "email không hợp lệ";
            return false;
        }
        if (password.Length < 6)
        {
            message = "mật khẩu phải có ít nhất 6 ký tự";
            return false;
        }
        message = "đăng ký thành công";
        return true;
    }
}

