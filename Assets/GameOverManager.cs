using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    private bool gameOver = false;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth =
                FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError(
                "[GameOverManager] Không tìm thấy PlayerHealth!"
            );

            return;
        }

        // Đăng ký event
        playerHealth.OnPlayerDeath += HandlePlayerDeath;

        Debug.Log(
            "[GameOverManager] Đã kết nối PlayerHealth"
        );
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    // =========================================================
    // PLAYER DEATH
    // =========================================================

    private void HandlePlayerDeath()
    {
        if (gameOver)
            return;

        gameOver = true;

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[GameOverManager] HANDLE PLAYER DEATH"
        );

        Debug.Log(
            "================================"
        );

        if (MapManager.instance != null)
        {
            MapManager.instance.PlayerDied();
        }
        else
        {
            Debug.LogError(
                "[GameOverManager] MapManager.instance = NULL!"
            );

            // Backup nếu MapManager không tồn tại
            PlayerHealth.ResetRunHealth();

            UnityEngine.SceneManagement.SceneManager
                .LoadScene("MapLevel1");
        }
    }
}