using UnityEngine;

public class panelswtich : MonoBehaviour
{
    public GameObject loginpanel;
    public GameObject registerpanel;
    void Start()
    {
        openloginpanel();
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
