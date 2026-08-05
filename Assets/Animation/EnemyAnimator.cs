using System.Collections;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public void OnDieAnimationFinished()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        BattleManager battleManager = FindAnyObjectByType<BattleManager>();

        if (battleManager != null)
        {
            battleManager.OnEnemyAnimationFinished(gameObject);
        }
    }
}