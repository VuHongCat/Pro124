using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 4;

    private int currentEnergy;
    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    private void Awake()
    {
        ResetEnergy();
    }
    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
    }

    public bool HasEnoughEnergy(int cost)
    {
        return currentEnergy >= cost;
    }

    public bool SpendEnergy(int cost)
    {
        if (!HasEnoughEnergy(cost)) return false;
        currentEnergy -= cost;
        Debug.Log($"{currentEnergy}/{maxEnergy}");
        return true;
    }

    public void GainEnergy(int amount)
    {
        currentEnergy += amount;
    }
}
