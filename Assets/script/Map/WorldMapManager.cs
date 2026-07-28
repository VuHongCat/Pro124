using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance;

    public WorldIsland island1;
    public WorldIsland island2;
    public WorldIsland island3;
    public WorldIsland island4;

    public int unlockedLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
    }

    private void Start()
    {
        UpdateMap();
    }

    public void UpdateMap()
    {
        island1.SetUnlocked(unlockedLevel >= 1);
        island2.SetUnlocked(unlockedLevel >= 2);
        island3.SetUnlocked(unlockedLevel >= 3);
        island4.SetUnlocked(unlockedLevel >= 4);
    }

    public void UnlockLevel(int level)
    {
        if (level > unlockedLevel)
        {
            unlockedLevel = level;

            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
            PlayerPrefs.Save();
        }
    }
}