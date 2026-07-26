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
            message("vui lòng nhập đủ thông tin", Color.red);
            return;
        }

        if (!File.Exists(filepath))
        {
            message("Tài khoản không tồn tại", Color.red);
            return;
        }
        message("đăng nhập thành công", Color.green);


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
            message("tài khoản hoặc mật khẩu không đúng", Color.red);
            return;
        }
        if (!iscorrectpass)
        {
            message("tài khoản hoặc mật khẩu không đúng", Color.red);
            return;
        }
        SceneLoader.Instance.LoadScene("MainMenu");
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