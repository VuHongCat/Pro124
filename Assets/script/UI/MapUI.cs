using UnityEngine;

public class MapUI : MonoBehaviour
{
    public void StartBattle()
    {
        SceneLoader.TransitionTo("Battle");
    }

    public void OpenShop()
    {
        SceneLoader.TransitionTo("Shop");
    }

    public void ReturnMainMenu()
    {
        SceneLoader.TransitionTo("MainMenu");
    }
}
