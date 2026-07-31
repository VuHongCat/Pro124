using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerStatus == null) playerStatus = GetComponent<PlayerStatus>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void Attack(EnemyHealth target, int damage)
    {
        if (target == null) return;
        damage += playerStatus.GetStatus(StatusType.Strength);
        int prev = target.CurrentHealth;
        target.TakeDamage(damage);
        int dealt = prev - target.CurrentHealth;
        if (dealt > 0 && playerStatus.GetStatus(StatusType.Lifesteal) > 0)
        {
            playerHealth?.Heal(Mathf.RoundToInt(dealt * 0.5f));
            playerStatus.ConsumeStatus(StatusType.Lifesteal);
        }
    }
}
