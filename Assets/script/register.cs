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
            message("vui lòng nhập đủ thông tin", Color.red);
            return;
        }
        //kiem tra dieu kien username
        if (!Regex.IsMatch(user, @"^[a-zA-Z0-9]+$"))
        {
            message("Tên tài khoản chỉ được chứa chữ cái và số", Color.red);
            return;
        }
        //kiem tra dieu kien password
        if (pass.Length < 6)
        {
            message("Mật khẩu phải có ít nhất 6 ký tự", Color.red);
            return;
        }
        //kiem tra dieu kien email
        if (!mail.Contains("@") || !mail.Contains("."))
        {
            message("email không hợp lệ", Color.red);
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
                        message("Tên tài khoản này đã tồn tại", Color.red);
                        return;
                    }
                }
            }
        }
        if (PlayerPrefs.HasKey("User_" + username + "_Pass"))
        {
            message("Tên tài khoản này đã tồn tại", Color.red);
            return;
        }
        PlayerPrefs.SetString("User" + user + "Pass", pass);
        PlayerPrefs.SetString("User" + user + "Email", mail);
        PlayerPrefs.Save(); // Lưu ngay lập tức vào máy

        message("đăng ký thành công", Color.green);
        if (panelswtich != null)
        {
            panelswtich.loginpanel.SetActive(true);
            panelswtich.registerpanel.SetActive(false);
        }
        username.text = "";
        email.text = "";
        password.text = "";
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
                    string savedPass = parts[1].Trim();
                    string savedEmail = parts[2].Trim();
                    if (savedUser == user && savedPass == pass && savedEmail == mail)
                    {
                        message("Tài khoản đã tồn tại trong file", Color.red);
                        return;
                    }
                }
            }
           
        }
        //ghi vao file
        string newline = user + "," + pass + "," + mail + System.Environment.NewLine;
        File.AppendAllText(filepath, newline);
        message("Đăng ký thành công", Color.green);
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
