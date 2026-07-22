using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
public class userauth : MonoBehaviour
{
    [Header("register")]
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_Text message;
    public GameObject messagePanel;
    private string filepath;
    private void Awake()
    {
        message = messagePanel.GetComponent<TMP_Text>();
        messagePanel.SetActive(false);
    }
    public void register()
    {
        string user = username.text.Trim();
        string pass = password.text.Trim();
        // string mail = email.text.Trim();
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            notification("vui long nhap du thong tin", Color.red);
            return;
        }
        //kiem tra tai khoan da dang ky chua
        if (PlayerPrefs.HasKey("user" + user))
        {
            notification("tai khoan da ton tai", Color.red);
            return;
        }
        if (PlayerPrefs.HasKey("user" + user))
        {
            notification("tai khoan da ton tai", Color.red);
            return;
        }
        else
        {
            PlayerPrefs.SetString("user" + user, pass);
            notification("dang ky thanh cong", Color.green);
        }

    }
    public void login()
    {
        string user = username.text.Trim();
        string pass = password.text.Trim();
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            notification("vui long nhap du thong tin", Color.red);
            return;
        }
        if (!PlayerPrefs.HasKey("user" + user))
        {
            notification("tai khoan khong ton tai", Color.red);
            return;
        }
        if (pass == PlayerPrefs.GetString("user" + user))
        {
            notification("dang nhap thanh cong", Color.green);
        }
        else
        {
            notification("sai mat khau", Color.red);
            message.color = Color.red;
        }
    }
    private void notification(string msg, Color color)
    {
        if (messagePanel != null && message != null)
        {
            message.text = msg;
            message.color = color;
            messagePanel.SetActive(true);
        }

    }
}