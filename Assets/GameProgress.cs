using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    [Header("Run Progress")]
    public int currentLevel = 1;
    public string currentNodeName = "";

    [Header("Player HP")]
    public int playerMaxHealth = 100;
    public int playerCurrentHealth = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // SET CURRENT NODE
    // =========================================================

    public void SetCurrentNode(string nodeName)
    {
        currentNodeName = nodeName;

        Debug.Log(
            "[GameProgress] Current Node = " +
            currentNodeName
        );
    }

    // =========================================================
    // SAVE PLAYER HP
    // =========================================================

    public void SavePlayerHealth(
        int currentHealth,
        int maxHealth)
    {
        playerCurrentHealth = currentHealth;
        playerMaxHealth = maxHealth;

        Debug.Log(
            "[GameProgress] Save HP = " +
            playerCurrentHealth +
            "/" +
            playerMaxHealth
        );
    }

    // =========================================================
    // GET PLAYER HP
    // =========================================================

    public int GetPlayerHealth()
    {
        return playerCurrentHealth;
    }

    public int GetPlayerMaxHealth()
    {
        return playerMaxHealth;
    }

    // =========================================================
    // START NEW RUN
    // =========================================================

    public void StartNewRun()
    {
        currentLevel = 1;
        currentNodeName = "";

        playerMaxHealth = 100;
        playerCurrentHealth = 100;

        Debug.Log(
            "[GameProgress] START NEW RUN"
        );
    }

    // =========================================================
    // RESET RUN WHEN PLAYER DIES
    // =========================================================

    public void ResetRun()
    {
        currentLevel = 1;
        currentNodeName = "";

        // Chết thì hồi đầy máu cho run mới
        playerMaxHealth = 100;
        playerCurrentHealth = 100;

        Debug.Log(
            "[GameProgress] RESET RUN"
        );

        Debug.Log(
            "[GameProgress] HP = " +
            playerCurrentHealth +
            "/" +
            playerMaxHealth
        );
    }
}