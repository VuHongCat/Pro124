using JetBrains.Annotations;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    public void Attack(EnemyHealth target, int damage)
    {
        if(target == null) return;
        damage += playerStatus.GetStatus(StatusType.Strength);
        target.TakeDamage(damage);
    }
}
