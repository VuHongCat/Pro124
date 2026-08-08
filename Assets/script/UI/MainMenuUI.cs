using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        if (MonsterIndexUI.Instance == null)
        {
            GameObject indexGo = new GameObject("MonsterIndexUI");
            indexGo.AddComponent<MonsterIndexUI>();
        }
    }

    public void PlayGame()
    {
        SceneLoader.TransitionTo("MapLevel1");
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
