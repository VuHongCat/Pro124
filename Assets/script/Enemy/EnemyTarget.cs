using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyTarget : MonoBehaviour, IPointerClickHandler
{
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (enemyHealth == null)
            return;

        EnemyTargetManager.Instance.SelectTarget(enemyHealth);
    }
}