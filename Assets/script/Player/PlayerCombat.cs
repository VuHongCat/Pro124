using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private PlayerHealth playerHealth;

    public void Attack(EnemyHealth target, int damage)
    {
        if (target == null) return;
        if (playerStatus == null) playerStatus = GetComponent<PlayerStatus>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        damage += playerStatus.GetStatus(StatusType.Strength);
        if (playerStatus.GetStatus(StatusType.Weak) > 0)
            damage = Mathf.RoundToInt(damage * 0.75f);
        int prev = target.CurrentHealth;
        target.TakeDamage(damage);
        int dealt = prev - target.CurrentHealth;

        int lifesteal = playerStatus.GetStatus(StatusType.Lifesteal);
        if (lifesteal > 0 && dealt > 0)
        {
            playerHealth?.Heal(Mathf.RoundToInt(dealt * 0.5f));
            playerStatus.AddStatus(StatusType.Lifesteal, -1);
        }
    }
}
