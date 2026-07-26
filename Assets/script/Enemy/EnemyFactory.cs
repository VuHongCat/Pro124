using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    public GameObject CreateEnemy(EnemyData data, Transform parent)
    {
        Debug.Log(enemyPrefab);
        Debug.Log(parent);
        GameObject enemy = Instantiate(enemyPrefab, parent, false);

        EnemyDisplay display = enemy.GetComponent<EnemyDisplay>();
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        EnemyCombat combat = enemy.GetComponent<EnemyCombat>();

        display.Setup(data);
        health.Initialize(data);
        combat.Initialize(data);
        return enemy;
    }
}