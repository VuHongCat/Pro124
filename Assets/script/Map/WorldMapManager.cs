using UnityEngine;


public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance;


    [Header("Island")]
    public WorldIslandUI island1;
    public WorldIslandUI island2;
    public WorldIslandUI island3;
    public WorldIslandUI island4;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        // Mặc định mở đảo 1
        if (!PlayerPrefs.HasKey("UnlockedIsland"))
        {
            PlayerPrefs.SetInt("UnlockedIsland", 1);
            PlayerPrefs.Save();
        }
    }



    private void Start()
    {
        UpdateMap();
    }



    public void UpdateMap()
    {
        island1.UpdateIsland();
        island2.UpdateIsland();
        island3.UpdateIsland();
        island4.UpdateIsland();
    }



    // Gọi khi thắng boss
    public void UnlockIsland(int id)
    {
        int current =
            PlayerPrefs.GetInt("UnlockedIsland", 1);


        if (id > current)
        {
            PlayerPrefs.SetInt("UnlockedIsland", id);
            PlayerPrefs.Save();
        }
    }
}