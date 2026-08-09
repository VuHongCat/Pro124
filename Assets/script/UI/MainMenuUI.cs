using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.TransitionTo("WorldMap");
    }

    public void ContinueGame()
    {
        if (RunSession.RunActive &&
            !string.IsNullOrEmpty(RunSession.MapSceneName))
        {
            SceneLoader.TransitionTo(RunSession.MapSceneName);
        }
        else
        {
            SceneLoader.TransitionTo("WorldMap");
        }
    }

    public void NewRun()
    {
        RunSession.StartNewRun();
        SceneLoader.TransitionTo("WorldMap");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
