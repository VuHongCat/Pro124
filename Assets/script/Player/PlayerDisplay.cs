using UnityEngine;
using TMPro;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthUI;
    }
    private void UpdateHealthUI(int current, int max)
    {
        hpText.text = $"{current}/{max}";
    }
}
