using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Starting Relics")]
    [SerializeField] private List<RelicData> startingRelics = new();

    [Header("Current Relics")]
    [SerializeField] private List<RelicData> ownedRelics = new();

    public IReadOnlyList<RelicData> OwnedRelics => ownedRelics;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (RelicData relic in startingRelics)
        {
            AddRelic(relic);
        }
    }

    //=========================================
    // Add Relic
    //=========================================

    public void AddRelic(RelicData relic)
    {
        if (relic == null)
            return;

        if (!relic.stackable && ownedRelics.Contains(relic))
            return;

        ownedRelics.Add(relic);

        Debug.Log("Obtained Relic : " + relic.relicName);
    }

    //=========================================
    // Remove Relic
    //=========================================

    public void RemoveRelic(RelicData relic)
    {
        if (relic == null)
            return;

        ownedRelics.Remove(relic);
    }

    //=========================================
    // Check Relic
    //=========================================

    public bool HasRelic(string relicName)
    {
        foreach (RelicData relic in ownedRelics)
        {
            if (relic.relicName == relicName)
                return true;
        }

        return false;
    }

    public RelicData GetRelic(string relicName)
    {
        foreach (RelicData relic in ownedRelics)
        {
            if (relic.relicName == relicName)
                return relic;
        }

        return null;
    }

    //=========================================
    // Battle
    //=========================================

    public void OnBattleStart()
    {
        Debug.Log("Relic Battle Start");

        foreach (RelicData relic in ownedRelics)
        {
            Debug.Log(relic.relicName);
        }
    }

    public void OnBattleWon()
    {
        Debug.Log("Relic Battle Won");
    }

    //=========================================
    // Turn
    //=========================================

    public void OnTurnStart()
    {

    }

    public void OnTurnEnd()
    {

    }

    //=========================================
    // Rest
    //=========================================

    public void OnRest()
    {

    }

    //=========================================
    // Card
    //=========================================

    public void OnGainCard()
    {

    }

    //=========================================
    // Gold
    //=========================================

    public bool CanGainGold()
    {
        return !HasRelic("Ectoplasm");
    }

    //=========================================
    // Debug
    //=========================================

    [ContextMenu("Print Relics")]
    public void PrintRelics()
    {
        Debug.Log("===== CURRENT RELICS =====");

        foreach (RelicData relic in ownedRelics)
        {
            Debug.Log(relic.relicName);
        }
    }
}