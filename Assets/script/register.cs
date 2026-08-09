using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class register : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField email;
    public TMP_Text notification;
    public panelswtich panelswtich;
    private string filepath;
    private void Start()
    {
        filepath = Path.Combine(Application.persistentDataPath, "user.txt");
        if (panelswtich == null)
        {
            panelswtich = FindFirstObjectByType<panelswtich>();
        }
    }
    public void onclickloginbutton()
    {

        string user = username.text.Trim();
        string pass = password.text.Trim();
        string mail = email.text.Trim();
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(mail))
        {
            message("Please fill in all fields", Color.red);
            return;
        }
        //kiem tra dieu kien username
        if (!Regex.IsMatch(user, @"^[a-zA-Z0-9]+$"))
        {
            message("Username may only contain letters and numbers", Color.red);
            return;
        }
        //kiem tra dieu kien password
        if (pass.Length < 6)
        {
            message("Password must be at least 6 characters", Color.red);
            return;
        }
        //kiem tra dieu kien email
        if (!mail.Contains("@") || !mail.Contains("."))
        {
            message("Invalid email", Color.red);
            return;
        }
        //kiem tra tai khoan da ton tai chua
        if (File.Exists(filepath))
        {
            string[] lines = File.ReadAllLines(filepath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    string savedUser = parts[0].Trim();
                    if (savedUser.Equals(user, System.StringComparison.OrdinalIgnoreCase))
                    {
                        message("This username already exists", Color.red);
                        return;
                    }
                }
            }
        }
        if (panelswtich != null)
        {
            panelswtich.openloginpanel();
        }

        string newline = user + "," + pass + "," + mail + System.Environment.NewLine;
        File.AppendAllText(filepath, newline);
        message("Registration successful", Color.green);
        username.text = "";
        email.text = "";
        password.text = "";
    }
    private void message(string msg, Color color)
    {
        if (notification != null)
        {
            notification.text = msg;
            notification.color = color;
        }
    }
    public void gotologinbutton()
    {
        if (panelswtich != null)
        {
            panelswtich.openloginpanel();
        }
    }
}
