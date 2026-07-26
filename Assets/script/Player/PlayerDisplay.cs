using UnityEngine;
using TMPro;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    private int currentBlock;

    private PlayerHealth playerHealth;
    private PlayerBlock playerBlock;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerBlock = GetComponent<PlayerBlock>();
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerBlock.OnBlockChanged += UpdateBlockUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
        playerBlock.OnBlockChanged -= UpdateBlockUI;
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
}
