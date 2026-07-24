using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.Instance.LoadScene("MapLevel1");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
