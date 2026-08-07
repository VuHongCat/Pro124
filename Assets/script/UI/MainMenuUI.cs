using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.TransitionTo("MapLevel1");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
