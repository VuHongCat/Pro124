using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Owned Relics")]
    [SerializeField] private List<RelicData> ownedRelics = new();

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

    #region Relic

    public void AddRelic(RelicData relic)
    {
        if (relic == null)
            return;

        if (!relic.stackable && HasRelic(relic.relicName))
            return;

        ownedRelics.Add(relic);

        ApplyInstantEffect(relic);
    }

    public void RemoveRelic(RelicData relic)
    {
        if (ownedRelics.Contains(relic))
            ownedRelics.Remove(relic);
    }

    public bool HasRelic(string relicName)
    {
        foreach (RelicData relic in ownedRelics)
        {
            if (relic.relicName == relicName)
                return true;
        }

        return false;
    }

    public List<RelicData> GetOwnedRelics()
    {
        return ownedRelics;
    }

    #endregion

    #region Instant Effect

    private void ApplyInstantEffect(RelicData relic)
    {
        switch (relic.relicName)
        {
            case "Ancient Core":
                // TODO
                break;

            case "Dragon Soul":
                // TODO
                break;

            case "Mango":
                // TODO
                break;

            case "Strawberry":
                // TODO
                break;

            case "Vajra":
                // TODO
                break;
        }
    }

    #endregion

    #region Battle

    public void OnBattleStart(List<EnemyHealth> enemies)
    {
        // Anchor

        // Bag of Marbles

        // Tea Set
    }

    public void OnBattleEnd()
    {
        // Blood Vial

        // Burning Blood
    }

    #endregion

    #region Turn

    public void OnPlayerTurnStart()
    {
        // Coffee Dripper

        // Happy Flower

        // Orichalcum

        // Red Skull
    }

    public void OnPlayerTurnEnd()
    {
        // Ice Cream
    }

    #endregion

    #region Events

    public void OnGainGold(int amount)
    {
        // Ectoplasm
    }

    public void OnObtainCard(CardData card)
    {
        // Ceramic Fish
    }

    public void OnRestSite()
    {
        // Girya

        // Tea Set

        // Coffee Dripper
    }

    #endregion

    #region Save

    public void SaveRelics()
    {
        // TODO
    }

    public void LoadRelics()
    {
        // TODO
    }

    #endregion
}