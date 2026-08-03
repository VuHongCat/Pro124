using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    public static EnemyTargetManager Instance { get; private set; }

    public EnemyHealth CurrentTarget { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectTarget(EnemyHealth target)
    {
        if (target == null)
            return;

        CurrentTarget = target;

        Debug.Log("Target: " + target.name);
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }

    public bool HasTarget()
    {
        return CurrentTarget != null;
    }
}