using UnityEngine;

public class panelswtich : MonoBehaviour
{
    public GameObject loginpanel;
    public GameObject registerpanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        openregisterpanel();
    }
    public void openloginpanel()
    {
        if(loginpanel != null)
        {
            loginpanel.SetActive(true);
        }
        if (registerpanel != null)
        {
            registerpanel.SetActive(false);
        }
    }
    public void openregisterpanel()
    {
        if (registerpanel != null)
        {
            registerpanel.SetActive(true);
        }
        if (loginpanel != null)
        {
            loginpanel.SetActive(false);
        }
    }
    
}
