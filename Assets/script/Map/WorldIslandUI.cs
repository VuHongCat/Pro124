using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldIslandUI : MonoBehaviour
{
    public int islandID;
    public string sceneName;

    private bool unlocked;


    public void UpdateIsland()
    {
        int unlockedIsland = PlayerPrefs.GetInt("UnlockedIsland", 1);

        unlocked = islandID <= unlockedIsland;
    }


    public void ClickIsland()
    {
        if (!unlocked)
            return;

        SceneManager.LoadScene(sceneName);
    }
}