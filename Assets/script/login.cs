using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class login : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text notification;
    private string filepath;
    public panelswtich panelswtich;
    private void Start()
    {
        filepath = Path.Combine(Application.persistentDataPath, "user.txt");
        if (panelswtich == null)
        {
            panelswtich = FindFirstObjectByType<panelswtich>();
        }
    }
   
    public void onclickbutton()
    {
        string user = username.text.Trim();
        string pass = password.text.Trim();
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            message("Please fill in all fields", Color.red);
            return;
        }

        if (!File.Exists(filepath))
        {
            message("Account does not exist", Color.red);
            return;
        }
        message("Login successful", Color.green);


        string[] lines = File.ReadAllLines(filepath);
        bool userfound = false;
        bool iscorrectpass = false;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(',');
            if (parts.Length >= 2)
            {
                string savedUsername = parts[0].Trim();
                string savedPassword = parts[1].Trim();
               if (savedUsername.Equals(user, System.StringComparison.OrdinalIgnoreCase))
                {
                    userfound = true;
                    if (savedPassword == pass)
                    {
                        iscorrectpass = true;
                    }
                    break;
                }
            }
        }
        if (!userfound)
        {
            message("Incorrect username or password", Color.red);
            return;
        }
        if (!iscorrectpass)
        {
            message("Incorrect username or password", Color.red);
            return;
        }
        SceneLoader.TransitionTo("MainMenu");
    }
    public void message(string msg, Color color)
    {
        if (notification != null)
        {
            notification.text = msg;
            notification.color = color;
        }
    }
    public void gotoregisterbuttton()
    {
        if (panelswtich != null)
        {
            panelswtich.openregisterpanel();
        }
    }
}