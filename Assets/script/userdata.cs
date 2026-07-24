using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using System;

public class userdata : MonoBehaviour
{
    private const string FilePath = "user.txt";
    //doc file text 
    public static Dictionary<string, string> ReadUserData()
    {
        var users = new Dictionary<string, string>();
        if (!File.Exists(FilePath))
        {
            return users;
        }
        foreach (string line in File.ReadAllLines(FilePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split('|');
            if (parts.Length >= 2)
            {
                string user = parts[0].Trim();
                string pass = parts[1].Trim();
                if (!users.ContainsKey(user)) users[user] = pass;
            }
        }
        return users;
    }
    public static void saveuserdata(string name, string password)
    {
        File.AppendAllText(FilePath, $"{name}|{password}\n");
    }
}   


