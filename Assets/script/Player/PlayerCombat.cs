using JetBrains.Annotations;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public void Attack(EnemyHealth target, int damage)
    {
        if(target == null) return;
        target.TakeDamage(damage);
    }
}
