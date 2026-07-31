using UnityEngine;
using TMPro;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text statusText;
    private int currentBlock;

    private PlayerHealth playerHealth;
    private PlayerBlock playerBlock;
    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerBlock = GetComponent<PlayerBlock>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerBlock.OnBlockChanged += UpdateBlockUI;
        if (playerStatus != null) playerStatus.OnStatusChanged += UpdateStatusUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerBlock.OnBlockChanged -= UpdateBlockUI;
        if (playerStatus != null) playerStatus.OnStatusChanged -= UpdateStatusUI;
    }
    private void UpdateHealthUI(int current, int max)
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        if (currentBlock > 0)
        {
            hpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth} ({currentBlock})";
        }
        else
        {
            hpText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }
    private void UpdateBlockUI(int block)
    {
        currentBlock = block;
        RefreshUI();
    }

    private void UpdateStatusUI()
    {
        if (statusText == null || playerStatus == null) return;
        System.Text.StringBuilder sb = new();
        int s = playerStatus.GetStatus(StatusType.Strength);
        int w = playerStatus.GetStatus(StatusType.Weak);
        int v = playerStatus.GetStatus(StatusType.Vulnerable);
        if (s > 0) sb.Append($"STR+{s} ");
        if (w > 0) sb.Append($"Weak({w}) ");
        if (v > 0) sb.Append($"Vuln({v}) ");
        statusText.text = sb.ToString();
    }
}
