
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
            message = "Please fill in all fields";
            return false;
        }
        message = "Login successful";
        return true;
    }
    public bool Register(string username, string email, string password, out string message)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            message = "Please fill in all fields";
            return false;
        }
        if (!email.Contains("@"))
        {
            message = "Invalid email";
            return false;
        }
        if (password.Length < 6)
        {
            message = "Password must be at least 6 characters";
            return false;
        }
        message = "Registration successful";
        return true;
    }
}

