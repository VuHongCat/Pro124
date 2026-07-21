using UnityEngine;

public class MapUI : MonoBehaviour
{
    public void StartBattle()
    {
        SceneLoader.Instance.LoadScene("Battle");
    }

    public void OpenShop()
    {
        SceneLoader.Instance.LoadScene("Shop");
    }

    public void ReturnMainMenu()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }
}
