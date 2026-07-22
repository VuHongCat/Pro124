using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.Instance.LoadScene("Map");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
