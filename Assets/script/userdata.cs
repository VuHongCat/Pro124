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
    [Serializable]
    public class account
    {
        public string username;
        public string password;
        public string email;
    }
    [Serializable]
    public class accountdata
    {
        public List<account > accounts = new List<account>();
    }
}


