using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldIslandUI : MonoBehaviour
{
    [Header("Island Info")]
    public int islandID;
    public string sceneName;


    [Header("UI")]
    public Button button;
    public Image islandImage;


    [Header("Color")]
    public Color unlockColor = Color.white;
    public Color lockColor = new Color(0.5f, 0.5f, 0.5f, 1f);


    private bool unlocked;


    public void UpdateIsland()
    {
        int unlockedIsland = PlayerPrefs.GetInt("UnlockedIsland", 1);


        if (islandID <= unlockedIsland)
        {
            UnlockIsland();
        }
        else
        {
            LockIsland();
        }
    }


    private void UnlockIsland()
    {
        unlocked = true;


        // đảo sáng
        if (islandImage != null)
            islandImage.color = unlockColor;


        // cho bấm
        if (button != null)
            button.interactable = true;
    }



    private void LockIsland()
    {
        unlocked = false;


        // đảo mờ
        if (islandImage != null)
            islandImage.color = lockColor;


        // không bấm được
        if (button != null)
            button.interactable = false;
    }



    public void ClickIsland()
    {
        if (!unlocked)
        {
            Debug.Log("Đảo chưa mở khóa!");
            return;
        }


        SceneLoader.TransitionTo(sceneName);
    }
}