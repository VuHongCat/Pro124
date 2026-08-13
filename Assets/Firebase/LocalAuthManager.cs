using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LocalAuthManager
{
    private const string FileName = "users.txt";

    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Please enter an email address.";
        if (!IsValidEmail(email))
            return "Invalid email format.";
        return null;
    }

    public static string ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "Please enter a password.";
        if (password.Length < 6)
            return "Password must be at least 6 characters.";
        return null;
    }

    public static string RegisterUser(string email, string password, out bool success)
    {
        success = false;

        string emailError = ValidateEmail(email);
        if (emailError != null) return emailError;

        string passError = ValidatePassword(password);
        if (passError != null) return passError;

        if (EmailExists(email))
            return "This email address is already registered.";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.AppendAllText(FilePath, $"{email.Trim()}|{password}\n");
            success = true;
            return "Registration successful!";
        }
        catch (Exception e)
        {
            Debug.LogError($"[LocalAuth] Failed to save user: {e}");
            return "Failed to save account. Please try again.";
        }
    }

    public static bool CheckLogin(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return false;

        string entry = FindUser(email);
        if (entry == null)
            return false;

        string[] parts = entry.Split('|');
        return parts.Length >= 2 && parts[1] == password;
    }

    private static bool EmailExists(string email)
    {
        return FindUser(email) != null;
    }

    private static string FindUser(string email)
    {
        if (!File.Exists(FilePath))
            return null;

        string[] lines = File.ReadAllLines(FilePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length >= 2 && string.Equals(parts[0].Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase))
                return line;
        }
        return null;
    }
}
