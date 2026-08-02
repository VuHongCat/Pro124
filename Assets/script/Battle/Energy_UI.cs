using TMPro;
using UnityEngine;

public class Energy_UI : MonoBehaviour
{
    public EnergyManager EnergyManager;
    public TMP_Text energyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (EnergyManager == null)
        {
            EnergyManager = FindAnyObjectByType<EnergyManager>();
        }
        UpdateEnergyUI();
    }
    private void Update()
    {
        UpdateEnergyUI();

    }

    public void UpdateEnergyUI()
    {
        if (EnergyManager == null) return;
        //cap nhat so mana
        if (energyText != null)
        {
            energyText.text = $"{EnergyManager.CurrentEnergy} / {EnergyManager.MaxEnergy}";
        }
    
    }
}
